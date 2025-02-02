using System;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.Analytics;
using PrioQ.Infrastructure.Decorators;

using PrioQ.Application.Interfaces;
using PrioQ.Tests.Mocks;

namespace Tests.UnitTests.Analytics
{
    public class AnalyticsDecoratorTests
    {
        [Fact]
        public void Enqueue_ForwardsCallToInnerQueue_AndDoesNotRecordAnalytics()
        {
            // Arrange
            var mockQueue = new MockPriorityQueue();
            var mockCollector = new MockAnalyticsCollector();
            var decorator = new AnalyticsDecorator(mockQueue, mockCollector);
            var item = new PriorityQueueItem(1, "Test Enqueue");

            // Act
            decorator.Enqueue(item);

            // Assert:
            // The inner queue should contain the item.
            var dequeued = mockQueue.Dequeue();
            Assert.NotNull(dequeued);
            Assert.Equal(item.Id, dequeued.Id);
            // And since Enqueue doesn't record analytics, the collector should be empty.
            Assert.Empty(mockCollector.DataPoints);
        }

        [Fact]
        public void Dequeue_RecordsAnalyticsAndReturnsItem_WhenItemExists()
        {
            // Arrange
            var mockQueue = new MockPriorityQueue();
            var mockCollector = new MockAnalyticsCollector();
            var decorator = new AnalyticsDecorator(mockQueue, mockCollector);
            var item = new PriorityQueueItem(2, "Test Dequeue");
            mockQueue.Enqueue(item);

            // Act
            // Capture a time window before and after Dequeue to check the recorded DequeuedAt.
            var startTime = DateTime.UtcNow;
            var returnedItem = decorator.Dequeue();
            var endTime = DateTime.UtcNow;

            // Assert:
            // The returned item should match the one enqueued.
            Assert.NotNull(returnedItem);
            Assert.Equal(item.Id, returnedItem.Id);

            // The collector should have exactly one recorded data point.
            Assert.Single(mockCollector.DataPoints);
            var dataPoint = mockCollector.DataPoints[0];
            Assert.Equal(item.Id, dataPoint.ItemId);
            Assert.Equal(item.Priority, dataPoint.Priority);
            Assert.Equal(item.EnqueuedAt, dataPoint.EnqueuedAt);
            // Verify that the DequeuedAt timestamp is between startTime and endTime.
            Assert.True(dataPoint.DequeuedAt >= startTime && dataPoint.DequeuedAt <= endTime,
                $"Expected DequeuedAt between {startTime} and {endTime}, but was {dataPoint.DequeuedAt}");
        }

        [Fact]
        public void Dequeue_DoesNotRecordAnalytics_WhenNoItemExists()
        {
            // Arrange
            var mockQueue = new MockPriorityQueue();
            var mockCollector = new MockAnalyticsCollector();
            var decorator = new AnalyticsDecorator(mockQueue, mockCollector);

            // Act
            var returnedItem = decorator.Dequeue();

            // Assert:
            // When no item exists, the returned value should be null.
            Assert.Null(returnedItem);
            // And no analytics data should be recorded.
            Assert.Empty(mockCollector.DataPoints);
        }
    }
}
