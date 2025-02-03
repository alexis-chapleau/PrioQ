using System.Collections.Generic;
using PrioQ.Domain.Entities;
using PrioQ.Application.Interfaces;
using PrioQ.Infrastructure.PriorityQueues;

namespace PrioQ.Infrastructure.Factories
{
    public class PriorityQueueFactory : IPriorityQueueFactory
    {
        private readonly IEnumerable<IPriorityQueueDecoratorFactory> _decoratorFactories;

        public PriorityQueueFactory(IEnumerable<IPriorityQueueDecoratorFactory> decoratorFactories)
        {
            _decoratorFactories = decoratorFactories;
        }

        public BasePriorityQueue CreatePriorityQueue(QueueConfig config)
        {
            BasePriorityQueue queue;

            // For unbounded priority, fall back to heap (which always uses locking).
            if (config.UnboundedPriority)
            {
                queue = new HeapPriorityQueue();
            }
            else
            {
                // Choose based on algorithm and maximum priority.
                switch (config.Algorithm)
                {
                    case PriorityQueueAlgorithm.Bitmask:
                        if (config.MaxPriority <= 64)
                        {
                            if (config.UseLocking)
                                queue = new BitmaskPriorityQueue();  // locked version
                            else
                                queue = new ConcurrentBitmaskPriorityQueue(); // concurrent version
                        }
                        else
                        {
                            // Fall back if range is too high.
                            if (config.UseLocking)
                                queue = new BucketPriorityQueue();
                            else
                                queue = new ConcurrentBucketPriorityQueue();
                        }
                        break;

                    case PriorityQueueAlgorithm.DoubleBitmask:
                        if (config.MaxPriority <= 256)
                        {
                            if (config.UseLocking)
                                queue = new DoubleLevelBitmaskPriorityQueue(); // locked version
                            else
                                queue = new ConcurrentDoubleLevelBitmaskPriorityQueue(); // concurrent version
                        }
                        else
                        {
                            if (config.UseLocking)
                                queue = new BucketPriorityQueue();
                            else
                                queue = new ConcurrentBucketPriorityQueue();
                        }
                        break;

                    case PriorityQueueAlgorithm.NormalBuckets:
                        if (config.UseLocking)
                            queue = new BucketPriorityQueue();
                        else
                            queue = new ConcurrentBucketPriorityQueue();
                        break;

                    case PriorityQueueAlgorithm.Heap:
                    default:
                        // Heap always uses locking.
                        queue = new HeapPriorityQueue();
                        break;
                }
            }

            if (queue is HeapPriorityQueue)
            {
                Console.Write("HeapPriorityQueue always uses locking; overriding config.UseLocking.");
                config.UseLocking = true;
            }

            // Apply any decorators.
            foreach (var decoratorFactory in _decoratorFactories)
            {
                if (decoratorFactory.ShouldApply(config))
                    queue = decoratorFactory.Apply(queue);
            }

            // Map all config attributes to the queue.
            queue.UseLogging = config.UseLogging;
            queue.UseLocking = config.UseLocking;
            queue.UseLazyDelete = config.UseLazyDelete;
            queue.UseAnalytics = config.UseAnalytics;
            queue.MaxPriority = config.MaxPriority;
            queue.Algorithm = config.Algorithm;
            queue.UnboundedPriority = config.UnboundedPriority;

            return queue;
        }
    }
}
