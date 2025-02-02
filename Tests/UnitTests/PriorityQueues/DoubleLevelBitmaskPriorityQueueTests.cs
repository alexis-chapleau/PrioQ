using System;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.PriorityQueues;

namespace PrioQ.Tests.UnitTests
{
    public class DoubleLevelBitmaskPriorityQueueTests
    {
        [Fact]
        public void Enqueue_And_Dequeue_Works_For_DoubleLevel()
        {
            var queue = new DoubleLevelBitmaskPriorityQueue();

            // Enqueue items with different priorities.
            var item1 = new PriorityQueueItem(1, "Item1");     // highest priority
            var item2 = new PriorityQueueItem(100, "Item2");
            var item3 = new PriorityQueueItem(50, "Item3");
            var item4 = new PriorityQueueItem(200, "Item4");

            queue.Enqueue(item1);
            queue.Enqueue(item2);
            queue.Enqueue(item3);
            queue.Enqueue(item4);

            // Expected order by numeric priority: item1, item3, item2, item4.
            Assert.Equal(item1.Id, queue.Dequeue().Id);
            Assert.Equal(item3.Id, queue.Dequeue().Id);
            Assert.Equal(item2.Id, queue.Dequeue().Id);
            Assert.Equal(item4.Id, queue.Dequeue().Id);
            Assert.Null(queue.Dequeue());
        }
    }
}
