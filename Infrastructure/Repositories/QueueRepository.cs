using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;


namespace PrioQ.Infrastructure.Repository
{
    public class QueueRepository : IQueueRepository
    {
        private BasePriorityQueue _queue;
        private bool _hasQueue = false;

        public BasePriorityQueue GetQueue()
        {            
            return _queue;
        }

        public void SetQueue(BasePriorityQueue queue)
        {
            _queue = queue;
            _hasQueue = true;
        }

        public bool HasQueue()
        {
            return _hasQueue;
        }
    }
}
