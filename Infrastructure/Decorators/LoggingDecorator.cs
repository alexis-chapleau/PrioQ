using System;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace PrioQ.Infrastructure.Decorators
{
    public class LoggingDecorator : IPriorityQueue
    {
        private readonly IPriorityQueue _innerQueue;
        private readonly ILogger<LoggingDecorator> _logger;

        public LoggingDecorator(IPriorityQueue innerQueue, ILogger<LoggingDecorator> logger)
        {
            _innerQueue = innerQueue ?? throw new ArgumentNullException(nameof(innerQueue));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Enqueue(PriorityQueueItem item)
        {
            _logger.LogInformation("Enqueueing item with Id {ItemId} at {Time}", item.Id, DateTime.UtcNow);
            _innerQueue.Enqueue(item);
        }

        public PriorityQueueItem Dequeue()
        {
            var item = _innerQueue.Dequeue();
            if (item != null)
            {
                _logger.LogInformation("Dequeued item with Id {ItemId} at {Time}", item.Id, DateTime.UtcNow);
            }
            else
            {
                _logger.LogInformation("Dequeued null item at {Time}", DateTime.UtcNow);
            }
            return item;
        }
    }
}
