using System;
using System.Collections.Generic;
using System.Numerics;
using PrioQ.Domain.Entities;


namespace PrioQ.Infrastructure.PriorityQueues
{
    public class BitmaskPriorityQueue : BasePriorityQueue
    {
        // Supports priorities 1 to 64 (offset: index 0 to 63).
        private readonly Queue<PriorityQueueItem>[] _buckets;
        private long _activeBuckets; // Each bit represents whether that bucket is non-empty.

        public BitmaskPriorityQueue()
        {
            _buckets = new Queue<PriorityQueueItem>[64];
            _activeBuckets = 0;
        }

        public override void Enqueue(PriorityQueueItem item)
        {
            // Adjust for offset: valid priorities are 1...64, so index = priority - 1.
            int index = item.Priority - 1;
            if (index < 0 || index >= _buckets.Length)
                throw new ArgumentOutOfRangeException(nameof(item.Priority), "Priority is out of range for BitmaskPriorityQueue (must be between 1 and 64).");

            if (_buckets[index] == null)
                _buckets[index] = new Queue<PriorityQueueItem>();

            _buckets[index].Enqueue(item);
            // Set the bit for this bucket.
            _activeBuckets |= 1L << index;
        }

        public override PriorityQueueItem Dequeue()
        {
            if (_activeBuckets == 0)
                return null;

            // Find the lowest set bit.
            int index = BitOperations.TrailingZeroCount((ulong)_activeBuckets);
            var bucket = _buckets[index];
            var item = bucket.Dequeue();
            if (bucket.Count == 0)
            {
                // Clear the bit.
                _activeBuckets &= ~(1L << index);
            }
            return item;
        }
    }
}
