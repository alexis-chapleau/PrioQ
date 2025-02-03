using System.Collections.Generic;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.PriorityQueues;

namespace PrioQ.Tests.PriorityQueues
{
    public class ConcurrentBitmaskPriorityQueueTests
    {
        [Fact]
        public void Enqueue_Dequeue_ItemsInPriorityOrder()
        {
            // Arrange
            var queue = new ConcurrentBitmaskPriorityQueue();
            var items = new List<PriorityQueueItem>
            {
                new PriorityQueueItem(10, "LowPriority"),
                new PriorityQueueItem(1, "HighPriority"),
                new PriorityQueueItem(5, "MediumPriority")
            };

            // Act
            foreach (var item in items)
            {
                queue.Enqueue(item);
            }

            // Assert: Since lower numeric priority means higher priority.
            PriorityQueueItem first = queue.Dequeue();
            PriorityQueueItem second = queue.Dequeue();
            PriorityQueueItem third = queue.Dequeue();

            Assert.Equal(1, first.Priority);
            Assert.Equal(5, second.Priority);
            Assert.Equal(10, third.Priority);
        }
    }
}
