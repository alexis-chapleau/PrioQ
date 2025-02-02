using System;
using PrioQ.Domain.Entities;


namespace PrioQ.Infrastructure.Decorators
{
    public class LockingDecorator : BasePriorityQueue
    {
        private readonly BasePriorityQueue _innerQueue;
        private readonly object _lockObj = new object();

        public LockingDecorator(BasePriorityQueue innerQueue)
        {
            _innerQueue = innerQueue;
        }

        public override void Enqueue(PriorityQueueItem item)
        {
            lock (_lockObj)
            {
                _innerQueue.Enqueue(item);
            }
        }

        public override PriorityQueueItem Dequeue()
        {
            lock (_lockObj)
            {
                return _innerQueue.Dequeue();
            }
        }
    }
}
