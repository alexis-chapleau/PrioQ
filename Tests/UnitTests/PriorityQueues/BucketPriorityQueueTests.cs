using System;
using System.Collections.Generic;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;
using PrioQ.Infrastructure.PriorityQueues;

namespace Tests.UnitTests.PriorityQueues
{
    public class BucketPriorityQueueTests
    {
        /// <summary>
        /// Returns a default QueueConfig instance.
        /// </summary>
        private QueueConfig GetTestConfig() => new QueueConfig();

        [Fact]
        public void Dequeue_ReturnsNull_WhenQueueIsEmpty()
        {
            // Arrange
            var config = GetTestConfig();
            IPriorityQueue queue = new BucketPriorityQueue(config);

            // Act
            var result = queue.Dequeue();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Enqueue_Dequeue_SingleItem_ReturnsSameItem()
        {
            // Arrange
            var config = GetTestConfig();
            IPriorityQueue queue = new BucketPriorityQueue(config);
            var item = new PriorityQueueItem(2, "TestCommand");

            // Act
            queue.Enqueue(item);
            var result = queue.Dequeue();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(item.Id, result.Id);
        }

        [Fact]
        public void Enqueue_Dequeue_MultiplePriorities_ReturnsItemsInCorrectOrder()
        {
            // Arrange
            var config = GetTestConfig();
            IPriorityQueue queue = new BucketPriorityQueue(config);
            // Lower numeric value means higher priority.
            var itemHigh = new PriorityQueueItem(1, "HighPriority");
            var itemMedium = new PriorityQueueItem(3, "MediumPriority");
            var itemLow = new PriorityQueueItem(5, "LowPriority");

            // Act
            queue.Enqueue(itemLow);
            queue.Enqueue(itemHigh);
            queue.Enqueue(itemMedium);

            // Assert
            // Expecting: HighPriority (1) -> MediumPriority (3) -> LowPriority (5)
            var result1 = queue.Dequeue();
            var result2 = queue.Dequeue();
            var result3 = queue.Dequeue();

            Assert.Equal(itemHigh.Id, result1.Id);
            Assert.Equal(itemMedium.Id, result2.Id);
            Assert.Equal(itemLow.Id, result3.Id);
        }

        [Fact]
        public void Enqueue_MultipleItems_SamePriority_ReturnsInFIFOOrder()
        {
            // Arrange
            var config = GetTestConfig();
            IPriorityQueue queue = new BucketPriorityQueue(config);
            var item1 = new PriorityQueueItem(2, "Command1");
            var item2 = new PriorityQueueItem(2, "Command2");
            var item3 = new PriorityQueueItem(2, "Command3");

            // Act
            queue.Enqueue(item1);
            queue.Enqueue(item2);
            queue.Enqueue(item3);

            // Assert: Items should come out in the same order they were enqueued (FIFO).
            var result1 = queue.Dequeue();
            var result2 = queue.Dequeue();
            var result3 = queue.Dequeue();

            Assert.Equal(item1.Id, result1.Id);
            Assert.Equal(item2.Id, result2.Id);
            Assert.Equal(item3.Id, result3.Id);
        }

        [Fact]
        public void Dequeue_SkipsEmptyBuckets_AndProcessesRemainingItems()
        {
            // Arrange
            var config = GetTestConfig();
            IPriorityQueue queue = new BucketPriorityQueue(config);
            var itemPriority1 = new PriorityQueueItem(1, "Priority1");
            var itemPriority3 = new PriorityQueueItem(3, "Priority3");

            // Act
            queue.Enqueue(itemPriority1);
            // Intentionally skip adding any item for priority 2.
            queue.Enqueue(itemPriority3);

            // Assert
            var result1 = queue.Dequeue();
            var result2 = queue.Dequeue();

            Assert.Equal(itemPriority1.Id, result1.Id);
            Assert.Equal(itemPriority3.Id, result2.Id);
        }
    }
}
