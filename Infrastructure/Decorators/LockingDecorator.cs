using System;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;

namespace PrioQ.Infrastructure.Decorators
{
    public class LockingDecorator : IPriorityQueue
    {
        private readonly IPriorityQueue _innerQueue;
        private readonly object _lockObj = new object();

        public LockingDecorator(IPriorityQueue innerQueue)
        {
            _innerQueue = innerQueue;
        }

        public void Enqueue(PriorityQueueItem item)
        {
            lock (_lockObj)
            {
                _innerQueue.Enqueue(item);
            }
        }

        public PriorityQueueItem Dequeue()
        {
            lock (_lockObj)
            {
                return _innerQueue.Dequeue();
            }
        }
    }
}
