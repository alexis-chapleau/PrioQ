using System;
using System.Linq;
using Xunit;
using PrioQ.Infrastructure.Analytics;
using PrioQ.Domain.Entities;

namespace Tests.UnitTests.Analytics
{
    public class AnalyticsCollectorTests
    {
        [Fact]
        public void RecordItemProcessed_ShouldAddDataPoint()
        {
            // Arrange
            var collector = new AnalyticsCollector();
            var dataPoint = new AnalyticsDataPoint
            {
                ItemId = Guid.NewGuid(),
                Priority = 1,
                EnqueuedAt = DateTime.UtcNow.AddMinutes(-5),
                DequeuedAt = DateTime.UtcNow
            };

            // Act
            collector.RecordItemProcessed(dataPoint);
            var dataPoints = collector.GetDataPoints();

            // Assert
            Assert.Single(dataPoints);
            Assert.Equal(dataPoint.ItemId, dataPoints.First().ItemId);
        }

        [Fact]
        public void GetDataPoints_ShouldReturnSnapshotOfAllRecordedData()
        {
            // Arrange
            var collector = new AnalyticsCollector();
            var dataPoint1 = new AnalyticsDataPoint
            {
                ItemId = Guid.NewGuid(),
                Priority = 1,
                EnqueuedAt = DateTime.UtcNow.AddMinutes(-10),
                DequeuedAt = DateTime.UtcNow.AddMinutes(-5)
            };
            var dataPoint2 = new AnalyticsDataPoint
            {
                ItemId = Guid.NewGuid(),
                Priority = 2,
                EnqueuedAt = DateTime.UtcNow.AddMinutes(-20),
                DequeuedAt = DateTime.UtcNow.AddMinutes(-10)
            };

            // Act
            collector.RecordItemProcessed(dataPoint1);
            collector.RecordItemProcessed(dataPoint2);
            var dataPoints = collector.GetDataPoints();

            // Assert
            Assert.Equal(2, dataPoints.Count);
            Assert.Contains(dataPoints, dp => dp.ItemId == dataPoint1.ItemId);
            Assert.Contains(dataPoints, dp => dp.ItemId == dataPoint2.ItemId);
        }
    }
}
