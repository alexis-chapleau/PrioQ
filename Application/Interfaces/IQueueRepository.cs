using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;

namespace PrioQ.Application.Interfaces
{
    public interface IQueueRepository
    {
        IPriorityQueue GetQueue();
        void SetQueue(IPriorityQueue queue);
    }
}
