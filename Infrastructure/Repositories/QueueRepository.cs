using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;


namespace PrioQ.Infrastructure.Repository
{
    public class QueueRepository : IQueueRepository
    {
        private BasePriorityQueue _queue;
        private readonly object _lockObj = new object();

        public BasePriorityQueue GetQueue()
        {
            lock (_lockObj)
            {
                return _queue;
            }
        }

        public void SetQueue(BasePriorityQueue queue)
        {
            lock (_lockObj)
            {
                _queue = queue;
            }
        }
    }
}
