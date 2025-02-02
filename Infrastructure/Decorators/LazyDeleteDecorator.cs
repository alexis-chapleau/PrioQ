using System;
using System.Collections.Generic;
using PrioQ.Domain.Entities;


namespace PrioQ.Infrastructure.Decorators
{
    public class LazyDeleteDecorator : BasePriorityQueue
    {
        private readonly BasePriorityQueue _innerQueue;
        private readonly HashSet<Guid> _toDelete;

        public LazyDeleteDecorator(BasePriorityQueue innerQueue)
        {
            _innerQueue = innerQueue;
            _toDelete = new HashSet<Guid>();
        }

        public override void Enqueue(PriorityQueueItem item)
        {
            _innerQueue.Enqueue(item);
        }

        public override PriorityQueueItem Dequeue()
        {
            while (true)
            {
                var item = _innerQueue.Dequeue();
                if (item == null)
                    return null;
                if (_toDelete.Contains(item.Id))
                {
                    _toDelete.Remove(item.Id);
                    continue;
                }
                return item;
            }
        }

        public void MarkForDeletion(Guid id)
        {
            _toDelete.Add(id);
        }
    }
}
