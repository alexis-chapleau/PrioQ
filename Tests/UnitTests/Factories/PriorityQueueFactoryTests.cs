using System.Collections.Generic;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;
using PrioQ.Infrastructure.Factories;
using PrioQ.Infrastructure.PriorityQueues;

namespace PrioQ.Tests.UnitTests
{
    public class PriorityQueueFactoryTests
    {
        // Test that if priority is unbounded, the factory always creates a HeapPriorityQueue.
        [Fact]
        public void CreatePriorityQueue_UnboundedConfig_AlwaysReturnsHeapQueue()
        {
            // Arrange
            var config = new QueueConfig
            {
                UnboundedPriority = true,
                // Other config values do not matter here.
                Algorithm = PriorityQueueAlgorithm.NormalBuckets, // Even if set to Buckets, unbounded should force heap.
                UseLogging = false,
                UseLocking = false,
                UseLazyDelete = false,
                UseAnalytics = false
            };

            // No decorator factories for simplicity.
            var decoratorFactories = new List<IPriorityQueueDecoratorFactory>();
            var factory = new PriorityQueueFactory(decoratorFactories);

            // Act
            IPriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert
            Assert.IsType<HeapPriorityQueue>(queue);
        }

        // Test that for bounded config, the factory respects the Algorithm flag.
        [Fact]
        public void CreatePriorityQueue_BoundedConfig_RespectsBucketAlgorithm()
        {
            // Arrange
            var config = new QueueConfig
            {
                UnboundedPriority = false,
                MaxPriority = 10,
                Algorithm = PriorityQueueAlgorithm.Bitmask,
                UseLogging = false,
                UseLocking = false,
                UseLazyDelete = false,
                UseAnalytics = false
            };

            var decoratorFactories = new List<IPriorityQueueDecoratorFactory>();
            var factory = new PriorityQueueFactory(decoratorFactories);

            // Act
            IPriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert
            Assert.IsType<BitmaskPriorityQueue>(queue);
        }
    }
}
