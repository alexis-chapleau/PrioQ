using System.Collections.Generic;
using System.Linq;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;

namespace PrioQ.Infrastructure.PriorityQueues
{
    public class BucketPriorityQueue : IPriorityQueue
    {
        private readonly Dictionary<int, Queue<PriorityQueueItem>> _buckets;

        public BucketPriorityQueue(QueueConfig config)
        {
            _buckets = new Dictionary<int, Queue<PriorityQueueItem>>();
        }

        public void Enqueue(PriorityQueueItem item)
        {
            if (!_buckets.ContainsKey(item.Priority))
                _buckets[item.Priority] = new Queue<PriorityQueueItem>();

            _buckets[item.Priority].Enqueue(item);
        }

        public PriorityQueueItem Dequeue()
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
