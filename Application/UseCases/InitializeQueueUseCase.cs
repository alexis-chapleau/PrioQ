using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;


namespace PrioQ.Application.UseCases
{
    public class InitializeQueueUseCase : IInitializeQueueUseCase
    {
        private readonly IQueueRepository _queueRepository;
        private readonly BasePriorityQueueFactory _queueFactory;
        private readonly QueueConfig _config;

        public InitializeQueueUseCase(
            IQueueRepository queueRepository,
            BasePriorityQueueFactory queueFactory,
            QueueConfig config)
        {
            _queueRepository = queueRepository;
            _queueFactory = queueFactory;
            _config = config;
        }

        public void Execute()
        {
            var newQueue = _queueFactory.CreatePriorityQueue(_config);
            _queueRepository.SetQueue(newQueue);
        }
        public void Execute(QueueConfig config)
        {
            var newQueue = _queueFactory.CreatePriorityQueue(config);
            _queueRepository.SetQueue(newQueue);
        }
    }
}
