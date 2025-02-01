using System;
using System.Reflection;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.Decorators;
using PrioQ.Tests.Mocks;

namespace Tests.UnitTests.Decorators
{
    public class LazyDeleteDecoratorTests
    {
        /// <summary>
        /// Helper to create a PriorityQueueItem.
        /// </summary>
        private PriorityQueueItem CreateItem(int priority, string command)
        {
            return new PriorityQueueItem(priority, command);
        }

        /// <summary>
        /// Helper method that sets the Id of a PriorityQueueItem using reflection.
        /// This is useful for testing scenarios that require control over the item's Id.
        /// </summary>
        private void SetPriorityQueueItemId(PriorityQueueItem item, Guid newId)
        {
            // The auto-property backing field is usually named "<Id>k__BackingField"
            var idField = typeof(PriorityQueueItem).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (idField == null)
            {
                throw new InvalidOperationException("Could not find backing field for Id.");
            }
            idField.SetValue(item, newId);
        }

        [Fact]
        public void Enqueue_ForwardsCallToInnerQueue()
        {
            // Arrange
            var mockQueue = new MockPriorityQueue();
            var decorator = new LazyDeleteDecorator(mockQueue);
            var item = CreateItem(1, "Test");

            // Act
            decorator.Enqueue(item);

            // Assert:
            // Directly dequeuing from the inner queue (our mock) should return the same item.
            var dequeued = mockQueue.Dequeue();
            Assert.NotNull(dequeued);
            Assert.Equal(item.Id, dequeued.Id);
        }

        [Fact]
        public void Dequeue_ReturnsItem_WhenNotMarkedForDeletion()
        {
            // Arrange
            var mockQueue = new MockPriorityQueue();
            var decorator = new LazyDeleteDecorator(mockQueue);
            var item = CreateItem(1, "NotDeleted");
            decorator.Enqueue(item);

            // Act
            var dequeued = decorator.Dequeue();

            // Assert
            Assert.NotNull(dequeued);
            Assert.Equal(item.Id, dequeued.Id);
        }

        [Fact]
        public void Dequeue_SkipsItem_MarkedForDeletion_And_ReturnsNextItem()
        {
            // Arrange
            var mockQueue = new MockPriorityQueue();
            var decorator = new LazyDeleteDecorator(mockQueue);

            // Create two items.
            var itemToDelete = CreateItem(1, "DeleteMe");
            var itemToKeep = CreateItem(2, "KeepMe");

            // Enqueue both items (order matters).
            decorator.Enqueue(itemToDelete);
            decorator.Enqueue(itemToKeep);

            // Mark the first item for deletion.
            decorator.MarkForDeletion(itemToDelete.Id);

            // Act:
            // The first call to Dequeue should skip the item marked for deletion.
            var firstDequeue = decorator.Dequeue();

            // Assert:
            // The returned item should be the second one.
            Assert.NotNull(firstDequeue);
            Assert.Equal(itemToKeep.Id, firstDequeue.Id);
        }

        [Fact]
        public void Dequeue_ReturnsNull_WhenAllItemsAreMarkedForDeletion()
        {
            // Arrange
            var mockQueue = new MockPriorityQueue();
            var decorator = new LazyDeleteDecorator(mockQueue);

            // Create two items.
            var item1 = CreateItem(1, "Item1");
            var item2 = CreateItem(2, "Item2");

            // Enqueue both items.
            decorator.Enqueue(item1);
            decorator.Enqueue(item2);

            // Mark both items for deletion.
            decorator.MarkForDeletion(item1.Id);
            decorator.MarkForDeletion(item2.Id);

            // Act:
            // Dequeue should skip both and eventually return null.
            var result = decorator.Dequeue();

            // Assert:
            Assert.Null(result);
        }

        [Fact]
        public void MarkForDeletion_IsOneTimeUse()
        {
            // Arrange
            var mockQueue = new MockPriorityQueue();
            var decorator = new LazyDeleteDecorator(mockQueue);

            // Create an item.
            var item = CreateItem(1, "OneTimeDelete");

            // Use reflection to assign a fixed id so we can later reuse it.
            var fixedId = Guid.NewGuid();
            SetPriorityQueueItemId(item, fixedId);

            // Enqueue the item.
            decorator.Enqueue(item);

            // Mark the item for deletion.
            decorator.MarkForDeletion(item.Id);

            // Act & Assert:
            // First Dequeue should skip the item and return null.
            var firstAttempt = decorator.Dequeue();
            Assert.Null(firstAttempt);

            // Now create a new item and force it to have the same fixed id.
            var newItem = CreateItem(1, "NewItemWithSameId");
            SetPriorityQueueItemId(newItem, fixedId);

            // Enqueue the new item.
            decorator.Enqueue(newItem);

            // Since the deletion marker should have been removed after being used,
            // the new item (with the same fixed id) should be returned.
            var secondAttempt = decorator.Dequeue();
            Assert.NotNull(secondAttempt);
            Assert.Equal(fixedId, secondAttempt.Id);
        }
    }
}
