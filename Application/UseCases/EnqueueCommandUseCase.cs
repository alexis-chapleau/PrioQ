using PrioQ.Domain.Entities;
using PrioQ.Application.Interfaces;
using PrioQ.Domain.Exceptions;

namespace PrioQ.Application.UseCases
{
    public class EnqueueCommandUseCase : IEnqueueCommandUseCase
    {
        private readonly IQueueRepository _queueRepository;
        private readonly QueueConfig _config;

        public EnqueueCommandUseCase(QueueConfig config, IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
            _config = config;
        }

        public void Execute(PriorityQueueItem item)
        {
            if (!_config.UnboundedPriority)
            {
                if (item.Priority < 1 || item.Priority > _config.MaxPriority)
                {
                    throw new PriorityOutOfRangeException(item.Priority, _config.MaxPriority);
                }
            }
            var queue = _queueRepository.GetQueue();
            queue.Enqueue(item);
        }
    }
}
