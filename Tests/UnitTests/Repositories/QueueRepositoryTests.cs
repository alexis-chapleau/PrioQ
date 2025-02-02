using System.Threading.Tasks;
using Xunit;
using PrioQ.Application.Interfaces;

using PrioQ.Infrastructure.Repository;
using PrioQ.Domain.Entities;

namespace Tests.UnitTests.Repositories
{
    public class QueueRepositoryTests
    {
        /// <summary>
        /// A simple dummy implementation of BasePriorityQueue for testing purposes.
        /// </summary>
        private class DummyQueue : BasePriorityQueue
        {
            public override void Enqueue(PriorityQueueItem item) { }
            public override PriorityQueueItem Dequeue() => null;
        }

        [Fact]
        public void GetQueue_ReturnsNull_WhenQueueNotSet()
        {
            // Arrange
            var repository = new QueueRepository();

            // Act
            var queue = repository.GetQueue();

            // Assert
            Assert.Null(queue);
        }

        [Fact]
        public void SetQueue_GetQueue_ReturnsSameInstance()
        {
            // Arrange
            var repository = new QueueRepository();
            BasePriorityQueue dummyQueue = new DummyQueue();

            // Act
            repository.SetQueue(dummyQueue);
            var retrievedQueue = repository.GetQueue();

            // Assert
            Assert.NotNull(retrievedQueue);
            Assert.Same(dummyQueue, retrievedQueue);
        }

        [Fact]
        public async Task ConcurrentAccess_DoesNotThrowExceptions_AndReturnsAValidInstance()
        {
            // Arrange
            var repository = new QueueRepository();
            BasePriorityQueue dummyQueue1 = new DummyQueue();
            BasePriorityQueue dummyQueue2 = new DummyQueue();

            // Act: Perform multiple concurrent operations.
            var tasks = new Task[100];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    // Alternate setting and getting the queue.
                    repository.SetQueue(dummyQueue1);
                    var q1 = repository.GetQueue();
                    repository.SetQueue(dummyQueue2);
                    var q2 = repository.GetQueue();
                });
            }

            await Task.WhenAll(tasks);

            // Assert: After concurrent operations, GetQueue should return one of the dummy queues.
            var finalQueue = repository.GetQueue();
            Assert.True(finalQueue == dummyQueue1 || finalQueue == dummyQueue2,
                "Final queue instance should be either dummyQueue1 or dummyQueue2.");
        }
    }
}
