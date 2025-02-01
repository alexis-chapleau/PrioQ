using System;
using System.Collections.Generic;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;

namespace PrioQ.Infrastructure.Decorators
{
    public class LazyDeleteDecorator : IPriorityQueue
    {
        private readonly IPriorityQueue _innerQueue;
        private readonly HashSet<Guid> _toDelete;

        public LazyDeleteDecorator(IPriorityQueue innerQueue)
        {
            _innerQueue = innerQueue;
            _toDelete = new HashSet<Guid>();
        }

        public void Enqueue(PriorityQueueItem item)
        {
            _innerQueue.Enqueue(item);
        }

        public PriorityQueueItem Dequeue()
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
