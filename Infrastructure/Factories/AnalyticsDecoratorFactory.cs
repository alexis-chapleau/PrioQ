using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;

using PrioQ.Infrastructure.Analytics;
using PrioQ.Infrastructure.Decorators;

namespace PrioQ.Infrastructure.Factories
{
    public class AnalyticsDecoratorFactory : IPriorityQueueDecoratorFactory
    {
        private readonly IAnalyticsCollector _collector;

        public AnalyticsDecoratorFactory(IAnalyticsCollector collector)
        {
            _collector = collector;
        }

        public bool ShouldApply(QueueConfig config) => config.UseAnalytics;

        public BasePriorityQueue Apply(BasePriorityQueue queue)
        {
            return new AnalyticsDecorator(queue, _collector);
        }
    }
}
