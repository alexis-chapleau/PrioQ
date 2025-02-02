using PrioQ.Domain.Entities;

using PrioQ.Infrastructure.Decorators;

namespace PrioQ.Infrastructure.Factories
{
    public class LazyDeleteDecoratorFactory : IPriorityQueueDecoratorFactory
    {
        public bool ShouldApply(QueueConfig config) => config.UseLazyDelete;

        public BasePriorityQueue Apply(BasePriorityQueue queue)
        {
            return new LazyDeleteDecorator(queue);
        }
    }
}
