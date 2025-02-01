using PrioQ.Domain.Entities;

namespace PrioQ.Domain.Interfaces
{
    public interface IPriorityQueue
    {
        void Enqueue(PriorityQueueItem item);
        PriorityQueueItem Dequeue();
    }
}
