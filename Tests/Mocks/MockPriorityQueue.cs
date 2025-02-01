using System.Collections.Generic;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;

namespace PrioQ.Tests.Mocks
{
    /// <summary>
    /// A simple mock implementation of IPriorityQueue for testing purposes.
    /// </summary>
    public class MockPriorityQueue : IPriorityQueue
    {
        private readonly Queue<PriorityQueueItem> _queue = new Queue<PriorityQueueItem>();

        public void Enqueue(PriorityQueueItem item)
        {
            _queue.Enqueue(item);
        }

        public PriorityQueueItem Dequeue()
        {
            return _queue.Count > 0 ? _queue.Dequeue() : null;
        }

        /// <summary>
        /// Exposes the current count (for debugging or additional assertions).
        /// </summary>
        public int Count => _queue.Count;
    }
}
