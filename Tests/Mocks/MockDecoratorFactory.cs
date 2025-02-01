using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;
using PrioQ.Infrastructure.Factories;
using PrioQ.Tests.Mocks;

namespace PrioQ.Tests.Mocks
{
    /// <summary>
    /// A mock decorator factory that conditionally applies the MockDecorator.
    /// </summary>
    public class MockDecoratorFactory : IPriorityQueueDecoratorFactory
    {
        private readonly bool _shouldApply;

        /// <summary>
        /// Initializes a new instance of the factory.
        /// </summary>
        /// <param name="shouldApply">Determines whether the decorator should be applied.</param>
        public MockDecoratorFactory(bool shouldApply)
        {
            _shouldApply = shouldApply;
        }

        public bool ShouldApply(QueueConfig config)
        {
            return _shouldApply;
        }

        public IPriorityQueue Apply(IPriorityQueue queue, QueueConfig config)
        {
            return new MockDecorator(queue);
        }
    }
}
