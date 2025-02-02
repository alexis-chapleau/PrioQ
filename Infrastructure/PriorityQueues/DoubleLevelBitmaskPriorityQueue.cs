using System;
using System.Collections.Generic;
using System.Numerics;
using PrioQ.Domain.Entities;


namespace PrioQ.Infrastructure.PriorityQueues
{
    public class DoubleLevelBitmaskPriorityQueue : BasePriorityQueue
    {
        private readonly int _groupSize = 64;
        private readonly int _numGroups = 4; // Total capacity = 256 priorities.
        private readonly Queue<PriorityQueueItem>[] _buckets;
        private readonly long[] _groupBitmasks; // One long per group.
        private long _groups; // Overall bitmask indicating which groups have items.

        public DoubleLevelBitmaskPriorityQueue()
        {
            int totalBuckets = _groupSize * _numGroups; // 256
            _buckets = new Queue<PriorityQueueItem>[totalBuckets];
            _groupBitmasks = new long[_numGroups];
            _groups = 0;
        }

        public override void Enqueue(PriorityQueueItem item)
        {
            // Adjust for offset: valid priorities are 1..256, so index = priority - 1.
            int priority = item.Priority - 1;
            
            if (_buckets[priority] == null)
                _buckets[priority] = new Queue<PriorityQueueItem>();

            _buckets[priority].Enqueue(item);
            int groupIndex = priority / _groupSize;
            int localIndex = priority % _groupSize;
            _groupBitmasks[groupIndex] |= 1L << localIndex;
            _groups |= 1L << groupIndex;
        }

        public override PriorityQueueItem Dequeue()
        {
            if (_groups == 0)
                return null;

            // Find the first group that has items.
            int groupIndex = BitOperations.TrailingZeroCount((ulong)_groups);
            long groupMask = _groupBitmasks[groupIndex];
            int localIndex = BitOperations.TrailingZeroCount((ulong)groupMask);
            int bucketIndex = groupIndex * _groupSize + localIndex;
            var bucket = _buckets[bucketIndex];
            var item = bucket.Dequeue();
            if (bucket.Count == 0)
            {
                // Clear the bit in the group mask.
                _groupBitmasks[groupIndex] &= ~(1L << localIndex);
                if (_groupBitmasks[groupIndex] == 0)
                {
                    // Clear the group bit.
                    _groups &= ~(1L << groupIndex);
                }
            }
            return item;
        }
    }
}
