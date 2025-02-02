using System.Collections.Generic;
using PrioQ.Domain.Entities;


namespace PrioQ.Infrastructure.PriorityQueues
{
    public class BucketPriorityQueue : BasePriorityQueue
    {
        private readonly Dictionary<int, Queue<PriorityQueueItem>> _buckets;

        public BucketPriorityQueue()
        {
            _buckets = new Dictionary<int, Queue<PriorityQueueItem>>();
        }

        public override void Enqueue(PriorityQueueItem item)
        {
            if (!_buckets.ContainsKey(item.Priority))
                _buckets[item.Priority] = new Queue<PriorityQueueItem>();

            _buckets[item.Priority].Enqueue(item);
        }

        public override PriorityQueueItem Dequeue()
        {
            // Assuming a lower numeric value represents a higher priority.
            foreach (var bucket in _buckets.OrderBy(kvp => kvp.Key))
            {
                if (bucket.Value.Count > 0)
                    return bucket.Value.Dequeue();
            }
            return null;
        }
    }
}
