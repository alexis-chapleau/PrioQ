using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;

public interface IPriorityQueueFactory
{
    IPriorityQueue CreatePriorityQueue(QueueConfig config);
}