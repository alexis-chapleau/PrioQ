using System;
using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;
using PrioQ.Infrastructure.Analytics;

namespace PrioQ.Infrastructure.Decorators
{
    public class AnalyticsDecorator : IPriorityQueue
    {
        private readonly IPriorityQueue _innerQueue;
        private readonly IAnalyticsCollector _collector;

        public AnalyticsDecorator(IPriorityQueue innerQueue, IAnalyticsCollector collector)
        {
            _innerQueue = innerQueue ?? throw new ArgumentNullException(nameof(innerQueue));
            _collector = collector ?? throw new ArgumentNullException(nameof(collector));
        }

        public void Enqueue(PriorityQueueItem item)
        {
            _innerQueue.Enqueue(item);
        }

        public PriorityQueueItem Dequeue()
        {
            var item = _innerQueue.Dequeue();
            if (item != null)
            {
                var dataPoint = new AnalyticsDataPoint
                {
                    ItemId = item.Id,
                    Priority = item.Priority,
                    EnqueuedAt = item.EnqueuedAt,
                    DequeuedAt = DateTime.UtcNow
                };

                _collector.RecordItemProcessed(dataPoint);
            }
            return item;
        }
    }
}
