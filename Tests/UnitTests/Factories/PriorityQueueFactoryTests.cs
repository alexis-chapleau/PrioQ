using System.Collections.Generic;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.Factories;
using PrioQ.Infrastructure.PriorityQueues;

namespace PrioQ.Tests.Factories
{
    public class PriorityQueueFactoryTests
    {
        // A dummy decorator factory that never applies (for testing).
        private class DummyDecoratorFactory : IPriorityQueueDecoratorFactory
        {
            public bool ShouldApply(QueueConfig config) => false;
            public BasePriorityQueue Apply(BasePriorityQueue queue) => queue;
        }

        private PriorityQueueFactory GetFactory()
        {
            return new PriorityQueueFactory(new List<IPriorityQueueDecoratorFactory> { new DummyDecoratorFactory() });
        }

        [Fact]
        public void CreatePriorityQueue_HeapAlgorithm_AlwaysLocked()
        {
            // Arrange
            var config = new QueueConfig
            {
                UnboundedPriority = false,
                Algorithm = PriorityQueueAlgorithm.Heap,
                UseLocking = false, // even if false, heap must be locked
                MaxPriority = 100
            };

            var factory = GetFactory();

            // Act
            BasePriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert
            Assert.IsType<HeapPriorityQueue>(queue);
            Assert.True(queue.UseLocking); // HeapPriorityQueue always forces locking.
        }

        [Fact]
        public void CreatePriorityQueue_BitmaskAlgorithm_ConcurrentVersionWhenNotLocking()
        {
            // Arrange
            var config = new QueueConfig
            {
                UnboundedPriority = false,
                Algorithm = PriorityQueueAlgorithm.Bitmask,
                UseLocking = false,
                MaxPriority = 64
            };

            var factory = GetFactory();

            // Act
            BasePriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert
            Assert.IsType<ConcurrentBitmaskPriorityQueue>(queue);
        }

        [Fact]
        public void CreatePriorityQueue_BitmaskAlgorithm_LockedVersionWhenLocking()
        {
            // Arrange
            var config = new QueueConfig
            {
                UnboundedPriority = false,
                Algorithm = PriorityQueueAlgorithm.Bitmask,
                UseLocking = true,
                MaxPriority = 64
            };

            var factory = GetFactory();

            // Act
            BasePriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert
            Assert.IsType<BitmaskPriorityQueue>(queue);
        }

        [Fact]
        public void CreatePriorityQueue_DoubleBitmaskAlgorithm_ConcurrentVersionWhenNotLocking()
        {
            // Arrange
            var config = new QueueConfig
            {
                UnboundedPriority = false,
                Algorithm = PriorityQueueAlgorithm.DoubleBitmask,
                UseLocking = false,
                MaxPriority = 256
            };

            var factory = GetFactory();

            // Act
            BasePriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert
            Assert.IsType<ConcurrentDoubleLevelBitmaskPriorityQueue>(queue);
        }

        [Fact]
        public void CreatePriorityQueue_DoubleBitmaskAlgorithm_LockedVersionWhenLocking()
        {
            // Arrange
            var config = new QueueConfig
            {
                UnboundedPriority = false,
                Algorithm = PriorityQueueAlgorithm.DoubleBitmask,
                UseLocking = true,
                MaxPriority = 256
            };

            var factory = GetFactory();

            // Act
            BasePriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert
            Assert.IsType<DoubleLevelBitmaskPriorityQueue>(queue);
        }

        [Fact]
        public void CreatePriorityQueue_NormalBuckets_ConcurrentVersionWhenNotLocking()
        {
            // Arrange
            var config = new QueueConfig
            {
                UnboundedPriority = false,
                Algorithm = PriorityQueueAlgorithm.NormalBuckets,
                UseLocking = false,
                MaxPriority = 100
            };

            var factory = GetFactory();

            // Act
            BasePriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert
            Assert.IsType<ConcurrentBucketPriorityQueue>(queue);
        }

        [Fact]
        public void CreatePriorityQueue_NormalBuckets_LockedVersionWhenLocking()
        {
            // Arrange
            var config = new QueueConfig
            {
                UnboundedPriority = false,
                Algorithm = PriorityQueueAlgorithm.NormalBuckets,
                UseLocking = true,
                MaxPriority = 100
            };

            var factory = GetFactory();

            // Act
            BasePriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert
            Assert.IsType<BucketPriorityQueue>(queue);
        }

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
            BasePriorityQueue queue = factory.CreatePriorityQueue(config);

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
                UseLocking = true,
                UseLazyDelete = false,
                UseAnalytics = false
            };

            var decoratorFactories = new List<IPriorityQueueDecoratorFactory>();
            var factory = new PriorityQueueFactory(decoratorFactories);

            // Act
            BasePriorityQueue queue = factory.CreatePriorityQueue(config);

            // Assert
            Assert.IsType<BitmaskPriorityQueue>(queue);
        }
    }
}
