using System;
using System.Collections.Concurrent;
using System.Numerics;
using System.Threading;
using PrioQ.Domain.Entities;

namespace PrioQ.Infrastructure.PriorityQueues
{
    public class ConcurrentDoubleLevelBitmaskPriorityQueue : BasePriorityQueue
    {
        private readonly int _groupSize = 64;           // Number of buckets per group.
        private readonly int _numGroups = 4;              // Total groups (4 * 64 = 256 priorities).
        private readonly ConcurrentQueue<PriorityQueueItem>[] _buckets;
        private readonly long[] _groupBitmasks;           // One 64-bit bitmask per group.
        private long _groups;                             // Overall bitmask indicating which groups have items.

        public ConcurrentDoubleLevelBitmaskPriorityQueue()
        {
            int totalBuckets = _groupSize * _numGroups;     // Total of 256 buckets.
            _buckets = new ConcurrentQueue<PriorityQueueItem>[totalBuckets];
            _groupBitmasks = new long[_numGroups];
            _groups = 0;
            // Buckets are lazily initialized in Enqueue.
        }

        public override void Enqueue(PriorityQueueItem item)
        {
            // Adjust for offset: valid priorities are 1..256, so bucketIndex = Priority - 1.
            int bucketIndex = item.Priority - 1;
            if (bucketIndex < 0 || bucketIndex >= _buckets.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(item.Priority), 
                    "Priority is out of range for DoubleLevelBitmaskPriorityQueue (must be between 1 and 256).");
            }

            // Lazy initialization of the bucket.
            var bucket = _buckets[bucketIndex];
            if (bucket == null)
            {
                var newBucket = new ConcurrentQueue<PriorityQueueItem>();
                bucket = Interlocked.CompareExchange(ref _buckets[bucketIndex], newBucket, null) ?? newBucket;
            }
            bucket.Enqueue(item);

            // Determine group and local index within the group.
            int groupIndex = bucketIndex / _groupSize;
            int localIndex = bucketIndex % _groupSize;

            // Mark the bucket as active.
            SetBucketBit(groupIndex, localIndex);
            // Mark the group as active.
            SetGroupBit(groupIndex);
        }

        public override PriorityQueueItem Dequeue()
        {
            while (true)
            {
                long groupsSnapshot = Interlocked.Read(ref _groups);
                if (groupsSnapshot == 0)
                {
                    // No groups have active buckets.
                    return null;
                }

                // Find the lowest-numbered active group.
                int groupIndex = BitOperations.TrailingZeroCount((ulong)groupsSnapshot);
                long groupMask = Interlocked.Read(ref _groupBitmasks[groupIndex]);
                if (groupMask == 0)
                {
                    // Somehow the group's bitmask is empty. Clear the group bit and retry.
                    ClearGroupBit(groupIndex);
                    continue;
                }

                // Within the group, find the lowest-numbered active bucket.
                int localIndex = BitOperations.TrailingZeroCount((ulong)groupMask);
                int bucketIndex = groupIndex * _groupSize + localIndex;
                var bucket = _buckets[bucketIndex];

                if (bucket == null)
                {
                    // Shouldn't happen if bit is set, but if so, clear the bucket bit and try again.
                    ClearBucketBit(groupIndex, localIndex);
                    continue;
                }

                if (bucket.TryDequeue(out PriorityQueueItem item))
                {
                    // If the bucket becomes empty, clear its bit.
                    if (bucket.IsEmpty)
                    {
                        ClearBucketBit(groupIndex, localIndex);
                        // If this clears the entire group, clear the group bit.
                        if (Interlocked.Read(ref _groupBitmasks[groupIndex]) == 0)
                        {
                            ClearGroupBit(groupIndex);
                        }
                    }
                    return item;
                }
                else
                {
                    // No item could be dequeued from this bucket; clear its bit and retry.
                    ClearBucketBit(groupIndex, localIndex);
                }
            }
        }

        #region Atomic Bitmask Helpers

        /// <summary>
        /// Atomically sets the bit corresponding to the specified bucket (within its group).
        /// </summary>
        private void SetBucketBit(int groupIndex, int localIndex)
        {
            long mask = 1L << localIndex;
            long oldVal, newVal;
            do
            {
                oldVal = _groupBitmasks[groupIndex];
                newVal = oldVal | mask;
            }
            while (Interlocked.CompareExchange(ref _groupBitmasks[groupIndex], newVal, oldVal) != oldVal);
        }

        /// <summary>
        /// Atomically clears the bit corresponding to the specified bucket (within its group).
        /// </summary>
        private void ClearBucketBit(int groupIndex, int localIndex)
        {
            long mask = ~(1L << localIndex);
            long oldVal, newVal;
            do
            {
                oldVal = _groupBitmasks[groupIndex];
                newVal = oldVal & mask;
            }
            while (Interlocked.CompareExchange(ref _groupBitmasks[groupIndex], newVal, oldVal) != oldVal);
        }

        /// <summary>
        /// Atomically sets the bit in _groups for the specified group index.
        /// </summary>
        private void SetGroupBit(int groupIndex)
        {
            long mask = 1L << groupIndex;
            long oldVal, newVal;
            do
            {
                oldVal = _groups;
                newVal = oldVal | mask;
            }
            while (Interlocked.CompareExchange(ref _groups, newVal, oldVal) != oldVal);
        }

        /// <summary>
        /// Atomically clears the bit in _groups for the specified group index.
        /// </summary>
        private void ClearGroupBit(int groupIndex)
        {
            long mask = ~(1L << groupIndex);
            long oldVal, newVal;
            do
            {
                oldVal = _groups;
                newVal = oldVal & mask;
            }
            while (Interlocked.CompareExchange(ref _groups, newVal, oldVal) != oldVal);
        }

        #endregion
    }
}
