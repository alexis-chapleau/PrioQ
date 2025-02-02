using System.Collections.Generic;
using Xunit;
using PrioQ.Domain.Entities;

using PrioQ.Infrastructure.PriorityQueues;

namespace Tests.UnitTests.PriorityQueues
{
    public class HeapPriorityQueueTests
    {
        /// <summary>
        /// Helper method to create a default QueueConfig instance.
        /// </summary>
        private QueueConfig GetTestConfig() => new QueueConfig();

        [Fact]
        public void Dequeue_ReturnsNull_WhenQueueIsEmpty()
        {
            // Arrange
            var config = GetTestConfig();
            HeapPriorityQueue queue = new HeapPriorityQueue();

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
            HeapPriorityQueue queue = new HeapPriorityQueue();
            var item = new PriorityQueueItem(2, "TestCommand");

            // Act
            queue.Enqueue(item);
            var result = queue.Dequeue();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(item.Id, result.Id);
        }

        [Fact]
        public void Enqueue_Dequeue_MultipleItems_ReturnsItemsInPriorityOrder()
        {
            // Arrange
            var config = GetTestConfig();
            HeapPriorityQueue queue = new HeapPriorityQueue();
            // Lower numeric value represents higher priority.
            var itemHigh = new PriorityQueueItem(1, "HighPriority");
            var itemMedium = new PriorityQueueItem(3, "MediumPriority");
            var itemLow = new PriorityQueueItem(5, "LowPriority");

            // Act
            queue.Enqueue(itemLow);
            queue.Enqueue(itemHigh);
            queue.Enqueue(itemMedium);

            // Assert
            // Expected order: HighPriority (1) -> MediumPriority (3) -> LowPriority (5)
            var result1 = queue.Dequeue();
            var result2 = queue.Dequeue();
            var result3 = queue.Dequeue();

            Assert.Equal(itemHigh.Id, result1.Id);
            Assert.Equal(itemMedium.Id, result2.Id);
            Assert.Equal(itemLow.Id, result3.Id);
        }

        [Fact]
        public void Enqueue_MultipleItems_SamePriority_ReturnsAllItems()
        {
            // Arrange
            var config = GetTestConfig();
            HeapPriorityQueue queue = new HeapPriorityQueue();
            // Using the same priority for all items.
            var item1 = new PriorityQueueItem(2, "Command1");
            var item2 = new PriorityQueueItem(2, "Command2");
            var item3 = new PriorityQueueItem(2, "Command3");

            // Act
            queue.Enqueue(item1);
            queue.Enqueue(item2);
            queue.Enqueue(item3);

            // Collect the dequeued items.
            var results = new List<PriorityQueueItem>
            {
                queue.Dequeue(),
                queue.Dequeue(),
                queue.Dequeue()
            };

            // Assert
            // Although order among items with the same priority is not guaranteed (heap is not stable),
            // all items should be present.
            Assert.Contains(results, x => x.Id == item1.Id);
            Assert.Contains(results, x => x.Id == item2.Id);
            Assert.Contains(results, x => x.Id == item3.Id);
        }

        [Fact]
        public void Enqueue_SamePriority_HeapReorders_FailsWithoutFIFO()
        {
            // Arrange
            HeapPriorityQueue queue = new HeapPriorityQueue();

            // Items with the same priority but added in an order where heap restructuring happens
            var item1 = new PriorityQueueItem(2, "Command1"); // Added first
            var item2 = new PriorityQueueItem(2, "Command2"); // Added second
            var item3 = new PriorityQueueItem(2, "Command3"); // Added third
            var item4 = new PriorityQueueItem(2, "Command4"); // Added fourth

            // Act - Insert in a way that forces heap restructuring
            queue.Enqueue(item3); // Insert item3 first
            queue.Enqueue(item1); // Insert item1 second
            queue.Enqueue(item4); // Insert item4 third
            queue.Enqueue(item2); // Insert item2 last, heap reshuffles

            // Dequeue and check order
            var dequeued1 = queue.Dequeue();
            var dequeued2 = queue.Dequeue();
            var dequeued3 = queue.Dequeue();
            var dequeued4 = queue.Dequeue();

            // Assert - This will FAIL without FIFO tracking
            Assert.Equal(item3.Id, dequeued1.Id);
            Assert.Equal(item1.Id, dequeued2.Id);
            Assert.Equal(item4.Id, dequeued3.Id);
            Assert.Equal(item2.Id, dequeued4.Id);
        }


        [Fact]
        public void HeapProperty_IsMaintained_AfterMultipleOperations()
        {
            // Arrange
            var config = GetTestConfig();
            HeapPriorityQueue queue = new HeapPriorityQueue();

            // Enqueue a series of items in a random order.
            var items = new List<PriorityQueueItem>
            {
                new PriorityQueueItem(4, "Command4"),
                new PriorityQueueItem(1, "Command1"),
                new PriorityQueueItem(3, "Command3"),
                new PriorityQueueItem(2, "Command2"),
                new PriorityQueueItem(5, "Command5"),
                new PriorityQueueItem(0, "Command0")
            };

            foreach (var item in items)
            {
                queue.Enqueue(item);
            }

            // Act
            var dequeuedItems = new List<PriorityQueueItem>();
            PriorityQueueItem current;
            while ((current = queue.Dequeue()) != null)
            {
                dequeuedItems.Add(current);
            }

            // Assert
            // The items should be dequeued in ascending order of priority.
            for (int i = 1; i < dequeuedItems.Count; i++)
            {
                Assert.True(dequeuedItems[i - 1].Priority <= dequeuedItems[i].Priority,
                    $"Item at index {i - 1} with priority {dequeuedItems[i - 1].Priority} is not <= item at index {i} with priority {dequeuedItems[i].Priority}");
            }
        }
    }
}
