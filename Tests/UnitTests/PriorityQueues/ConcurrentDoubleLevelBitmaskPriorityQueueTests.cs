using System.Collections.Generic;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.PriorityQueues;

namespace PrioQ.Tests.PriorityQueues
{
    public class ConcurrentDoubleLevelBitmaskPriorityQueueTests
    {
        [Fact]
        public void Enqueue_Dequeue_ItemsInPriorityOrder()
        {
            // Arrange
            var queue = new ConcurrentDoubleLevelBitmaskPriorityQueue();
            var items = new List<PriorityQueueItem>
            {
                new PriorityQueueItem(200, "LowPriority"),   // lower priority value means higher importance
                new PriorityQueueItem(1, "HighPriority"),
                new PriorityQueueItem(100, "MediumPriority")
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

            Assert.Equal(1, first.Priority);
            Assert.Equal(100, second.Priority);
            Assert.Equal(200, third.Priority);
        }
    }
}
