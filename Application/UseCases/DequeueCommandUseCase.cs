using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;

namespace PrioQ.Application.UseCases
{
    public class DequeueCommandUseCase : IDequeueCommandUseCase
    {
        private readonly IQueueRepository _queueRepository;

        public DequeueCommandUseCase(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public PriorityQueueItem Execute()
        {
            var queue = _queueRepository.GetQueue();
            return queue.Dequeue();
        }
    }
}
