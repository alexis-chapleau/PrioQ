using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.Decorators;
using PrioQ.Tests.Mocks;

namespace Tests.UnitTests.Decorators
{
    public class LockingDecoratorTests
    {
        [Fact]
        public void Enqueue_ForwardsCallToInnerQueue()
        {
            // Arrange
            var mockQueue = new MockPriorityQueue();
            var lockingDecorator = new LockingDecorator(mockQueue);
            var item = new PriorityQueueItem(1, "Test");

            // Act
            lockingDecorator.Enqueue(item);

            // Assert: Dequeue directly from the mockQueue should return the same item.
            var dequeued = mockQueue.Dequeue();
            Assert.NotNull(dequeued);
            Assert.Equal(item.Id, dequeued.Id);
        }

        [Fact]
        public void Dequeue_ForwardsCallToInnerQueue()
        {
            // Arrange
            var mockQueue = new MockPriorityQueue();
            var lockingDecorator = new LockingDecorator(mockQueue);
            var item = new PriorityQueueItem(1, "Test");

            // Preload the inner queue directly.
            mockQueue.Enqueue(item);

            // Act
            var dequeued = lockingDecorator.Dequeue();

            // Assert
            Assert.NotNull(dequeued);
            Assert.Equal(item.Id, dequeued.Id);
        }

        [Fact]
        public async Task LockingDecorator_ConcurrentEnqueue_ShouldContainAllItems()
        {
            // Arrange
            var mockQueue = new MockPriorityQueue();
            var lockingDecorator = new LockingDecorator(mockQueue);
            int numberOfTasks = 1000;
            var tasks = new List<Task>();

            // Act: Enqueue concurrently from multiple tasks.
            for (int i = 0; i < numberOfTasks; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    lockingDecorator.Enqueue(new PriorityQueueItem(1, "Concurrent"));
                }));
            }
            await Task.WhenAll(tasks);

            // Assert: The inner queue (mockQueue) should contain exactly 'numberOfTasks' items.
            Assert.Equal(numberOfTasks, mockQueue.Count);
        }

        [Fact]
        public async Task LockingDecorator_ConcurrentDequeue_ShouldReturnAllItems()
        {
            // Arrange
            int numberOfItems = 1000;
            var mockQueue = new MockPriorityQueue();

            // Preload the inner queue with a known number of items.
            for (int i = 0; i < numberOfItems; i++)
            {
                mockQueue.Enqueue(new PriorityQueueItem(1, $"Item {i}"));
            }

            var lockingDecorator = new LockingDecorator(mockQueue);
            var tasks = new List<Task<PriorityQueueItem>>();

            // Act: Start concurrent dequeue tasks.
            for (int i = 0; i < numberOfItems; i++)
            {
                tasks.Add(Task.Run(() => lockingDecorator.Dequeue()));
            }
            PriorityQueueItem[] results = await Task.WhenAll(tasks);
            var dequeuedItems = results.Where(item => item != null).ToList();

            // Assert: All preloaded items should be dequeued.
            Assert.Equal(numberOfItems, dequeuedItems.Count);
        }
    }
}
