using PrioQ.Application.Interfaces;
using PrioQ.Domain.Interfaces;

namespace PrioQ.Infrastructure.Repository
{
    public class QueueRepository : IQueueRepository
    {
        private IPriorityQueue _queue;
        private readonly object _lockObj = new object();

        public IPriorityQueue GetQueue()
        {
            lock (_lockObj)
            {
                return _queue;
            }
        }

        public void SetQueue(IPriorityQueue queue)
        {
            lock (_lockObj)
            {
                _queue = queue;
            }
        }
    }
}
