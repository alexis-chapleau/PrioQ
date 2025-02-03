using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PrioQ.Domain.Entities;

namespace PrioQ.Infrastructure.PriorityQueues
{
    public class ConcurrentBucketPriorityQueue : BasePriorityQueue
    {
        private readonly ConcurrentDictionary<int, ConcurrentQueue<PriorityQueueItem>> _buckets;
        private readonly SortedSet<int> _activePriorities; // keys for non-empty buckets
        private readonly object _activePrioritiesLock = new object();

        public ConcurrentBucketPriorityQueue()
        {
            _buckets = new ConcurrentDictionary<int, ConcurrentQueue<PriorityQueueItem>>();
            _activePriorities = new SortedSet<int>();
        }

        public override void Enqueue(PriorityQueueItem item)
        {
            var bucket = _buckets.GetOrAdd(item.Priority, p => new ConcurrentQueue<PriorityQueueItem>());
            bucket.Enqueue(item);

            // Add the key to the sorted set.
            lock (_activePrioritiesLock)
            {
                _activePriorities.Add(item.Priority);
            }
        }

        public override PriorityQueueItem Dequeue()
        {
            int keyToUse = -1;
            lock (_activePrioritiesLock)
            {
                if (_activePriorities.Count > 0)
                {
                    // Get the lowest priority
                    keyToUse = _activePriorities.First();
                }
            }

            if (keyToUse != -1 && _buckets.TryGetValue(keyToUse, out var bucket))
            {
                if (bucket.TryDequeue(out var item))
                {
                    // If the bucket becomes empty, remove it from the sorted set.
                    if (bucket.IsEmpty)
                    {
                        lock (_activePrioritiesLock)
                        {
                            _activePriorities.Remove(keyToUse);
                        }
                    }
                    return item;
                }
            }

            return null;
        }
    }
}
