using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;
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

        public IPriorityQueue Apply(IPriorityQueue queue)
        {
            return new AnalyticsDecorator(queue, _collector);
        }
    }
}
