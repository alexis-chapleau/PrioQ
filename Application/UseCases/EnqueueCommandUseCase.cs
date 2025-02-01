using PrioQ.Domain.Entities;
using PrioQ.Application.Interfaces;

namespace PrioQ.Application.UseCases
{
    public class EnqueueCommandUseCase : IEnqueueCommandUseCase
    {
        private readonly IQueueRepository _queueRepository;

        public EnqueueCommandUseCase(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public void Execute(PriorityQueueItem item)
        {
            var queue = _queueRepository.GetQueue();
            queue.Enqueue(item);
        }
    }
}
