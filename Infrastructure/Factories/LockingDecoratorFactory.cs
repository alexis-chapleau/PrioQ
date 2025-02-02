using PrioQ.Domain.Entities;

using PrioQ.Infrastructure.Decorators;

namespace PrioQ.Infrastructure.Factories
{
    public class LockingDecoratorFactory : IPriorityQueueDecoratorFactory
    {
        public bool ShouldApply(QueueConfig config) => config.UseLocking;

        public BasePriorityQueue Apply(BasePriorityQueue queue)
        {
            return new LockingDecorator(queue);
        }
    }
}
