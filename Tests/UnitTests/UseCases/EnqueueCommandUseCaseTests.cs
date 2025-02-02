using System;
using Xunit;
using Moq;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Exceptions;
using PrioQ.Application.UseCases;
using PrioQ.Application.Interfaces; // For IQueueRepository
using PrioQ.Tests.Mocks; // For FakePriorityQueue

namespace PrioQ.Tests.UnitTests
{
    public class EnqueueCommandUseCaseTests
    {
        // Helper method to create a bounded config.
        private QueueConfig CreateBoundedConfig(int maxPriority = 10)
        {
            return new QueueConfig
            {
                UnboundedPriority = false,
                MaxPriority = maxPriority,
                Algorithm = PriorityQueueAlgorithm.NormalBuckets,
                UseLogging = false,
                UseLocking = false,
                UseLazyDelete = false,
                UseAnalytics = false
            };
        }

        // Helper method to create an unbounded config.
        private QueueConfig CreateUnboundedConfig()
        {
            return new QueueConfig
            {
                UnboundedPriority = true,
                // MaxPriority is ignored when unbounded.
                Algorithm = PriorityQueueAlgorithm.NormalBuckets,
                UseLogging = false,
                UseLocking = false,
                UseLazyDelete = false,
                UseAnalytics = false
            };
        }

        [Fact]
        public void Execute_BoundedConfig_WithPriorityAboveMax_ThrowsException()
        {
            // Arrange
            var config = CreateBoundedConfig(maxPriority: 10);
            var mockRepo = new Mock<IQueueRepository>();
            // Return a fake queue so that Enqueue is called on a valid instance.
            var mockQueue = new MockPriorityQueue();
            mockQueue.MaxPriority = 10;
            mockRepo.Setup(r => r.GetQueue()).Returns(mockQueue);
            var useCase = new EnqueueCommandUseCase(config, mockRepo.Object);
            var item = new PriorityQueueItem(11, "Test Command");

            // Act & Assert
            var ex = Assert.Throws<PriorityOutOfRangeException>(() => useCase.Execute(item));
            Assert.Equal(11, ex.ProvidedPriority);
            Assert.Equal(10, ex.MaxAllowed);
        }

        [Fact]
        public void Execute_BoundedConfig_WithPriorityBelowMin_ThrowsException()
        {
            // Arrange
            var config = CreateBoundedConfig(maxPriority: 10);
            var mockRepo = new Mock<IQueueRepository>();
            mockRepo.Setup(r => r.GetQueue()).Returns(new MockPriorityQueue());
            var useCase = new EnqueueCommandUseCase(config, mockRepo.Object);
            var item = new PriorityQueueItem(0, "Test Command");

            // Act & Assert
            var ex = Assert.Throws<PriorityOutOfRangeException>(() => useCase.Execute(item));
            Assert.Equal(0, ex.ProvidedPriority);
        }

        [Fact]
        public void Execute_BoundedConfig_WithValidPriority_DoesNotThrow()
        {
            // Arrange
            var config = CreateBoundedConfig(maxPriority: 10);
            var mockQueue = new MockPriorityQueue();
            mockQueue.MaxPriority = 10;
            var mockRepo = new Mock<IQueueRepository>();
            mockRepo.Setup(r => r.GetQueue()).Returns(mockQueue);
            var useCase = new EnqueueCommandUseCase(config, mockRepo.Object);
            var item = new PriorityQueueItem(5, "Valid Command");

            // Act & Assert
            var exception = Record.Exception(() => useCase.Execute(item));
            Assert.Null(exception);
        }

        [Fact]
        public void Execute_UnboundedConfig_AllowsAnyPriority()
        {
            // Arrange
            var config = CreateUnboundedConfig();
            var mockQueue = new MockPriorityQueue();
            mockQueue.UnboundedPriority = true;
            var mockRepo = new Mock<IQueueRepository>();
            mockRepo.Setup(r => r.GetQueue()).Returns(mockQueue);
            var useCase = new EnqueueCommandUseCase(config, mockRepo.Object);
            var highPriorityItem = new PriorityQueueItem(100, "High Priority");
            var lowPriorityItem = new PriorityQueueItem(-5, "Low Priority");

            // Act & Assert
            Assert.Null(Record.Exception(() => useCase.Execute(highPriorityItem)));
            Assert.Null(Record.Exception(() => useCase.Execute(lowPriorityItem)));

        }
    }
}
