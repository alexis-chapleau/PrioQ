using PrioQ.Domain.Entities;

namespace PrioQ.Application.Interfaces
{
    public interface IQueueRepository
    {
        BasePriorityQueue GetQueue();
        void SetQueue(BasePriorityQueue queue);
    }
}
