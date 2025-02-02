using System.Collections.Generic;
using PrioQ.Domain.Entities;


namespace PrioQ.Tests.Mocks
{
    /// <summary>
    /// A simple mock implementation of BasePriorityQueue for testing purposes.
    /// </summary>
    public class MockPriorityQueue : BasePriorityQueue
    {
        private readonly Queue<PriorityQueueItem> _queue = new Queue<PriorityQueueItem>();

        public override void Enqueue(PriorityQueueItem item)
        {
            _queue.Enqueue(item);
        }

        public override PriorityQueueItem Dequeue()
        {
            return _queue.Count > 0 ? _queue.Dequeue() : null;
        }

        /// <summary>
        /// Exposes the current count (for debugging or additional assertions).
        /// </summary>
        public int Count => _queue.Count;
    }
}
