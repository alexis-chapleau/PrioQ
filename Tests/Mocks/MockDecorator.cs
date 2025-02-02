using PrioQ.Domain.Entities;


namespace PrioQ.Tests.Mocks
{
    /// <summary>
    /// A simple mock decorator that wraps an inner BasePriorityQueue.
    /// Exposes the inner queue via the InnerQueue property for testing purposes.
    /// </summary>
    public class MockDecorator : BasePriorityQueue
    {
        public BasePriorityQueue InnerQueue { get; }

        public MockDecorator(BasePriorityQueue innerQueue)
        {
            InnerQueue = innerQueue;
        }

        public override void Enqueue(PriorityQueueItem item)
        {
            InnerQueue.Enqueue(item);
        }

        public override PriorityQueueItem Dequeue()
        {
            return InnerQueue.Dequeue();
        }
    }
}
