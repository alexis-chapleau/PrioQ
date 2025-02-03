using System;
using System.Collections.Concurrent;
using System.Numerics;
using System.Threading;
using PrioQ.Domain.Entities;

namespace PrioQ.Infrastructure.PriorityQueues
{
    public class ConcurrentBitmaskPriorityQueue : BasePriorityQueue
    {
        // Supports priorities 1 to 64 (offset: index 0 to 63).
        private readonly ConcurrentQueue<PriorityQueueItem>[] _buckets;
        // Each bit in _activeBuckets indicates whether that bucket is non-empty.
        private long _activeBuckets;

        public ConcurrentBitmaskPriorityQueue()
        {
            // Allocate 64 buckets (one for each priority level).
            _buckets = new ConcurrentQueue<PriorityQueueItem>[64];
            for (int i = 0; i < _buckets.Length; i++)
            {
                _buckets[i] = new ConcurrentQueue<PriorityQueueItem>();
            }
            _activeBuckets = 0;
        }

        public override void Enqueue(PriorityQueueItem item)
        {
            // Adjust for offset: valid priorities are 1...64, so index = priority - 1.
            int index = item.Priority - 1;
            if (index < 0 || index >= _buckets.Length)
                throw new ArgumentOutOfRangeException(nameof(item.Priority), 
                    "Priority must be between 1 and 64.");

            // Enqueue the item in the corresponding bucket.
            _buckets[index].Enqueue(item);

            // Mark this bucket as active by setting its bit.
            SetBucketBit(index);
        }

        public override PriorityQueueItem Dequeue()
        {
            // If no bucket is active, return null.
            if (Interlocked.Read(ref _activeBuckets) == 0)
                return null;

            // Try to dequeue from the lowest active bucket.
            return TryDequeueFromActiveBucket();
        }

        /// <summary>
        /// Atomically sets the bit corresponding to the specified bucket index.
        /// </summary>
        private void SetBucketBit(int index)
        {
            long mask = 1L << index;
            long oldVal, newVal;
            do
            {
                oldVal = _activeBuckets;
                newVal = oldVal | mask;
            }
            while (Interlocked.CompareExchange(ref _activeBuckets, newVal, oldVal) != oldVal);
        }

        /// <summary>
        /// Atomically clears the bit corresponding to the specified bucket index.
        /// </summary>
        private void ClearBucketBit(int index)
        {
            long mask = ~(1L << index);
            long oldVal, newVal;
            do
            {
                oldVal = _activeBuckets;
                newVal = oldVal & mask;
            }
            while (Interlocked.CompareExchange(ref _activeBuckets, newVal, oldVal) != oldVal);
        }

        /// <summary>
        /// Attempts to dequeue an item from the lowest active bucket.
        /// If the bucket becomes empty, its bit is cleared.
        /// Returns the dequeued item, or null if no item is available.
        /// </summary>
        private PriorityQueueItem TryDequeueFromActiveBucket()
        {
            // We use a loop here because in lock-free code, the state can change
            // between reading the active bitmask and dequeuing an item.
            while (true)
            {
                // Read the current active buckets.
                long active = Interlocked.Read(ref _activeBuckets);
                if (active == 0)
                {
                    // No active buckets remain.
                    return null;
                }

                // Find the index of the lowest active bucket.
                int index = BitOperations.TrailingZeroCount((ulong)active);

                // Attempt to dequeue from that bucket.
                if (_buckets[index].TryDequeue(out PriorityQueueItem item))
                {
                    // If the bucket is now empty, clear its bit.
                    if (_buckets[index].IsEmpty)
                    {
                        ClearBucketBit(index);
                    }
                    return item;
                }
                else
                {
                    // If no item could be dequeued, clear this bucket's bit and try again.
                    ClearBucketBit(index);
                }
            }
        }
    }
}
