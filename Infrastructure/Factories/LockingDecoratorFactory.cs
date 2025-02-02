using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;
using PrioQ.Infrastructure.Decorators;

namespace PrioQ.Infrastructure.Factories
{
    public class LockingDecoratorFactory : IPriorityQueueDecoratorFactory
    {
        public bool ShouldApply(QueueConfig config) => config.UseLocking;

        public IPriorityQueue Apply(IPriorityQueue queue)
        {
            return new LockingDecorator(queue);
        }
    }
}
