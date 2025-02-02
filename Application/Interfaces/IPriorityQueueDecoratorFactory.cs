using PrioQ.Domain.Entities;

namespace PrioQ.Infrastructure.Factories
{
    public interface IPriorityQueueDecoratorFactory
    {
        bool ShouldApply(QueueConfig config);
        BasePriorityQueue Apply(BasePriorityQueue queue);
    }
}
