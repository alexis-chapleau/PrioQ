using System.Collections.Generic;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.PriorityQueues;


namespace PrioQ.Infrastructure.Factories
{
    public class PriorityQueueFactory : BasePriorityQueueFactory
    {
        private readonly IEnumerable<IPriorityQueueDecoratorFactory> _decoratorFactories;

        public PriorityQueueFactory(IEnumerable<IPriorityQueueDecoratorFactory> decoratorFactories)
        {
            _decoratorFactories = decoratorFactories;
        }

        public BasePriorityQueue CreatePriorityQueue(QueueConfig config)
        {
            BasePriorityQueue queue;

            // If the queue is unbounded, we fall back to the heap.
            if (config.UnboundedPriority)
            {
                queue = new HeapPriorityQueue();
            }
            else
            {
                // Choose based on the algorithm and the maximum priority.
                switch (config.Algorithm)
                {
                    case PriorityQueueAlgorithm.Bitmask:
                        if (config.MaxPriority <= 64)
                            queue = new BitmaskPriorityQueue();
                        else
                            // If user selects bitmask but the range is too high, fall back.
                            queue = new BucketPriorityQueue();
                        break;

                    case PriorityQueueAlgorithm.DoubleBitmask:
                        if (config.MaxPriority <= 256)
                            queue = new DoubleLevelBitmaskPriorityQueue();
                        else
                            queue = new BucketPriorityQueue();
                        break;

                    case PriorityQueueAlgorithm.NormalBuckets:
                        queue = new BucketPriorityQueue();
                        break;

                    case PriorityQueueAlgorithm.Heap:
                    default:
                        queue = new HeapPriorityQueue();
                        break;
                }
            }

            // Apply decorators (if any).
            foreach (var decoratorFactory in _decoratorFactories)
            {
                if (decoratorFactory.ShouldApply(config))
                    queue = decoratorFactory.Apply(queue);
            }

            // Map all attributes of the config object to the queue.
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
