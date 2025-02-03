using PrioQ.Domain.Entities;

public interface IPriorityQueueFactory
{
    BasePriorityQueue CreatePriorityQueue(QueueConfig config);
}