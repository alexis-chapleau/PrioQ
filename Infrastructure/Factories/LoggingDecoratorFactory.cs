using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;
using PrioQ.Infrastructure.Decorators;
using Microsoft.Extensions.Logging;

namespace PrioQ.Infrastructure.Factories
{
    public class LoggingDecoratorFactory : IPriorityQueueDecoratorFactory
    {
        private readonly ILogger<LoggingDecorator> _logger;

        public LoggingDecoratorFactory(ILogger<LoggingDecorator> logger)
        {
            _logger = logger;
        }

        public bool ShouldApply(QueueConfig config) => config.UseLogging;

        public IPriorityQueue Apply(IPriorityQueue queue)
        {
            return new LoggingDecorator(queue, _logger);
        }
    }
}
