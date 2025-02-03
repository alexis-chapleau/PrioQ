using System.Collections.Generic;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.PriorityQueues;

namespace PrioQ.Tests.PriorityQueues
{
    public class ConcurrentBucketsPriorityQueueTests
    {
        [Fact]
        public void Enqueue_Dequeue_ItemsInPriorityOrder()
        {
            // Arrange
            var queue = new ConcurrentBucketPriorityQueue();
            var items = new List<PriorityQueueItem>
            {
                new PriorityQueueItem(50, "LowPriority"),
                new PriorityQueueItem(10, "HighPriority"),
                new PriorityQueueItem(30, "MediumPriority")
            };

            // Act
            foreach (var item in items)
            {
                queue.Enqueue(item);
            }

            // Assert
            PriorityQueueItem first = queue.Dequeue();
            PriorityQueueItem second = queue.Dequeue();
            PriorityQueueItem third = queue.Dequeue();

            Assert.Equal(10, first.Priority);
            Assert.Equal(30, second.Priority);
            Assert.Equal(50, third.Priority);
        }
    }
}
