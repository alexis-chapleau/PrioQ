using System;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.PriorityQueues;

namespace PrioQ.Tests.UnitTests
{
    public class BitmaskPriorityQueueTests
    {
        [Fact]
        public void Enqueue_And_Dequeue_Works_For_SingleLevel()
        {
            var queue = new BitmaskPriorityQueue();

            // Enqueue items with different priorities.
            var item1 = new PriorityQueueItem(1, "Item1"); // highest priority
            var item2 = new PriorityQueueItem(10, "Item2");
            var item3 = new PriorityQueueItem(5, "Item3");
            var item4 = new PriorityQueueItem(1, "Item4");
            var item5 = new PriorityQueueItem(2, "Item5");
            var item6 = new PriorityQueueItem(5, "Item6");

            queue.Enqueue(item1);
            queue.Enqueue(item2);
            queue.Enqueue(item3);
            queue.Enqueue(item4);
            queue.Enqueue(item5);
            queue.Enqueue(item6);

            // Expected order: item1 (priority 1), then item3 (priority 5), then item2 (priority 10)
            Assert.Equal(item1.Id, queue.Dequeue().Id);
            Assert.Equal(item4.Id, queue.Dequeue().Id);
            Assert.Equal(item5.Id, queue.Dequeue().Id);
            Assert.Equal(item3.Id, queue.Dequeue().Id);
            Assert.Equal(item6.Id, queue.Dequeue().Id);
            Assert.Equal(item2.Id, queue.Dequeue().Id);
            Assert.Null(queue.Dequeue());
        }
    }
}
