using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;
using PrioQ.Infrastructure.Decorators;

namespace PrioQ.Infrastructure.Factories
{
    public class LazyDeleteDecoratorFactory : IPriorityQueueDecoratorFactory
    {
        public bool ShouldApply(QueueConfig config) => config.UseLazyDelete;

        public IPriorityQueue Apply(IPriorityQueue queue, QueueConfig config)
        {
            return new LazyDeleteDecorator(queue);
        }
    }
}
