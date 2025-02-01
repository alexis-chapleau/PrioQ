using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;

namespace PrioQ.Tests.Mocks
{
    /// <summary>
    /// A simple mock decorator that wraps an inner IPriorityQueue.
    /// Exposes the inner queue via the InnerQueue property for testing purposes.
    /// </summary>
    public class MockDecorator : IPriorityQueue
    {
        public IPriorityQueue InnerQueue { get; }

        public MockDecorator(IPriorityQueue innerQueue)
        {
            InnerQueue = innerQueue;
        }

        public void Enqueue(PriorityQueueItem item)
        {
            InnerQueue.Enqueue(item);
        }

        public PriorityQueueItem Dequeue()
        {
            return InnerQueue.Dequeue();
        }
    }
}
