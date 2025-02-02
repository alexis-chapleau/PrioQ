using System.Collections.Generic;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;
using PrioQ.Infrastructure.PriorityQueues;
using PrioQ.Infrastructure.Decorators;

namespace PrioQ.Infrastructure.Factories
{
    public class PriorityQueueFactory : IPriorityQueueFactory
    {
        private readonly IEnumerable<IPriorityQueueDecoratorFactory> _decoratorFactories;

        public PriorityQueueFactory(IEnumerable<IPriorityQueueDecoratorFactory> decoratorFactories)
        {
            _decoratorFactories = decoratorFactories;
        }

        public IPriorityQueue CreatePriorityQueue(QueueConfig config)
        {
            IPriorityQueue queue;

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

            return queue;
        }
    }
}
