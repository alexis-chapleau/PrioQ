using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;

namespace PrioQ.Infrastructure.Factories
{
    public interface IPriorityQueueDecoratorFactory
    {
        bool ShouldApply(QueueConfig config);
        IPriorityQueue Apply(IPriorityQueue queue, QueueConfig config);
    }
}
