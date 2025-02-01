using System.Collections.Generic;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;
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

        public IPriorityQueue CreatePriorityQueue(QueueConfig config)
        {
            IPriorityQueue queue = config.UseBucketAlgorithm
                ? (IPriorityQueue)new BucketPriorityQueue(config)
                : new HeapPriorityQueue(config);

            foreach (var decoratorFactory in _decoratorFactories)
            {
                if (decoratorFactory.ShouldApply(config))
                    queue = decoratorFactory.Apply(queue, config);
            }
            return queue;
        }
    }
}
