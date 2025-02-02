using PrioQ.Domain.Entities;

public interface BasePriorityQueueFactory
{
    BasePriorityQueue CreatePriorityQueue(QueueConfig config);
}