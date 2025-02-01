using System.Collections.Generic;
using Xunit;
using PrioQ.Domain.Entities;              // For QueueConfig and PriorityQueueItem
using PrioQ.Domain.Interfaces;             // For IPriorityQueue
using PrioQ.Infrastructure.Factories;      // For PriorityQueueFactory and IPriorityQueueDecoratorFactory
using PrioQ.Infrastructure.PriorityQueues; // For BucketPriorityQueue and HeapPriorityQueue
using PrioQ.Tests.Mocks;

namespace Tests.UnitTests.Factories
{
    public class PriorityQueueFactoryTests
    {
        [Fact]
        public void CreatePriorityQueue_UsesBucketPriorityQueue_WhenUseBucketAlgorithmIsTrue()
        {
            // Arrange
            var config = new QueueConfig
            {
                UseBucketAlgorithm = true,
                UseLogging = false,
                UseLocking = false,
                UseLazyDelete = false,
                UseAnalytics = false
            };

            // No decorators provided.
            var decoratorFactories = new List<IPriorityQueueDecoratorFactory>();
            var factory = new PriorityQueueFactory(decoratorFactories);

            // Act
            IPriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert
            Assert.IsType<BucketPriorityQueue>(queue);
        }

        [Fact]
        public void CreatePriorityQueue_UsesHeapPriorityQueue_WhenUseBucketAlgorithmIsFalse()
        {
            // Arrange
            var config = new QueueConfig
            {
                UseBucketAlgorithm = false,
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
            Assert.IsType<HeapPriorityQueue>(queue);
        }

        [Fact]
        public void CreatePriorityQueue_AppliesDecoratorFactories_WhenConfigured()
        {
            // Arrange:
            // Use a config with UseBucketAlgorithm = false so the base queue is HeapPriorityQueue.
            var config = new QueueConfig
            {
                UseBucketAlgorithm = false,
                UseLogging = true,
                UseLocking = true,
                UseLazyDelete = true,
                UseAnalytics = true
            };

            // Create two MockDecoratorFactory instances that always apply.
            var decoratorFactories = new List<IPriorityQueueDecoratorFactory>
            {
                new MockDecoratorFactory(true),
                new MockDecoratorFactory(true)
            };

            var factory = new PriorityQueueFactory(decoratorFactories);

            // Act
            IPriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert:
            // The final queue should be wrapped by two layers of MockDecorator.
            var outerDecorator = Assert.IsType<MockDecorator>(queue);
            var middleQueue = outerDecorator.InnerQueue;
            var innerDecorator = Assert.IsType<MockDecorator>(middleQueue);
            var baseQueue = innerDecorator.InnerQueue;
            Assert.IsType<HeapPriorityQueue>(baseQueue);
        }

        [Fact]
        public void CreatePriorityQueue_DoesNotApplyDecoratorFactories_WhenShouldApplyIsFalse()
        {
            // Arrange:
            // Create a config with UseBucketAlgorithm = false.
            var config = new QueueConfig
            {
                UseBucketAlgorithm = false,
                UseLogging = false,
                UseLocking = false,
                UseLazyDelete = false,
                UseAnalytics = false
            };

            // Create a MockDecoratorFactory that should NOT apply.
            var decoratorFactories = new List<IPriorityQueueDecoratorFactory>
            {
                new MockDecoratorFactory(false)
            };

            var factory = new PriorityQueueFactory(decoratorFactories);

            // Act
            IPriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert: Since the decorator should not apply, expect the base queue (HeapPriorityQueue).
            Assert.IsType<HeapPriorityQueue>(queue);
        }
    }
}
