using System;
using PrioQ.Domain.Entities;

using Microsoft.Extensions.Logging;

namespace PrioQ.Infrastructure.Decorators
{
    public class LoggingDecorator : BasePriorityQueue
    {
        private readonly BasePriorityQueue _innerQueue;
        private readonly ILogger<LoggingDecorator> _logger;

        public LoggingDecorator(BasePriorityQueue innerQueue, ILogger<LoggingDecorator> logger)
        {
            _innerQueue = innerQueue ?? throw new ArgumentNullException(nameof(innerQueue));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void Enqueue(PriorityQueueItem item)
        {
            _logger.LogInformation("Enqueueing item with Id {ItemId} at {Time}", item.Id, DateTime.UtcNow);
            _innerQueue.Enqueue(item);
        }

        public override PriorityQueueItem Dequeue()
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
