using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using PrioQ.Infrastructure.Analytics;
using PrioQ.Application.Interfaces;  // For IAnalyticsCollector, if needed
using PrioQ.Domain.Entities;

namespace Tests.UnitTests.Analytics
{
    public class AnalyticsReportGeneratorTests
    {
        /// <summary>
        /// Creates an AnalyticsCollector preloaded with sample data.
        /// </summary>
        /// <returns>A collector with sample data points.</returns>
        private AnalyticsCollector CreateCollectorWithSampleData()
        {
            var collector = new AnalyticsCollector();
            var now = DateTime.UtcNow;

            // Priority 1: Two items based on the current time.
            // First item: 5 minutes in queue.
            collector.RecordItemProcessed(new AnalyticsDataPoint
            {
                ItemId = Guid.NewGuid(),
                Priority = 1,
                EnqueuedAt = now.AddMinutes(-7),
                DequeuedAt = now.AddMinutes(-2) // 5 minutes
            });
            // Second item: 7 minutes in queue.
            collector.RecordItemProcessed(new AnalyticsDataPoint
            {
                ItemId = Guid.NewGuid(),
                Priority = 1,
                EnqueuedAt = now.AddMinutes(-8),
                DequeuedAt = now.AddMinutes(-1) // 7 minutes
            });

            // Priority 2: One item with 10 minutes in queue.
            collector.RecordItemProcessed(new AnalyticsDataPoint
            {
                ItemId = Guid.NewGuid(),
                Priority = 2,
                EnqueuedAt = now.AddMinutes(-12),
                DequeuedAt = now.AddMinutes(-2) // 10 minutes
            });

            // Additional data point for grouping tests: fixed time in the future.
            var specificTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            collector.RecordItemProcessed(new AnalyticsDataPoint
            {
                ItemId = Guid.NewGuid(),
                Priority = 1,
                EnqueuedAt = specificTime.AddMinutes(-10),
                DequeuedAt = specificTime // 10 minutes
            });

            return collector;
        }

        [Fact]
        public void GetAverageTimeInQueueByPriority_ReturnsCorrectAverages()
        {
            // Arrange
            var collector = CreateCollectorWithSampleData();
            var reportGenerator = new AnalyticsReportGenerator(collector);

            // Act
            var averages = reportGenerator.GetAverageTimeInQueueByPriority();

            // Assert:
            // For Priority 1, we have three items: 5, 7, and 10 minutes.
            // Expected average = (5 + 7 + 10) / 3 ≈ 7.33 minutes.
            Assert.True(averages.ContainsKey(1));
            var expectedPriority1 = TimeSpan.FromMinutes((5 + 7 + 10) / 3.0);
            // Allowing a margin of ±1 second for rounding differences.
            Assert.InRange(averages[1].TotalSeconds, expectedPriority1.TotalSeconds - 1, expectedPriority1.TotalSeconds + 1);

            // For Priority 2, only one item exists with 10 minutes.
            Assert.True(averages.ContainsKey(2));
            Assert.Equal(TimeSpan.FromMinutes(10), averages[2]);
        }

        [Fact]
        public void GetTotalItemsProcessed_ReturnsCorrectCount()
        {
            // Arrange
            var collector = CreateCollectorWithSampleData();
            var reportGenerator = new AnalyticsReportGenerator(collector);

            // Act
            // Use a wide range that covers all sample data points.
            var totalCount = reportGenerator.GetTotalItemsProcessed(new DateTime(2000, 1, 1), new DateTime(2100, 1, 1));

            // Assert: There are 4 sample data points in our sample collector.
            Assert.Equal(4, totalCount);
        }

        [Fact]
        public void GetItemsProcessedByDayOfWeek_ReturnsCorrectGrouping()
        {
            // Arrange
            var collector = CreateCollectorWithSampleData();
            var reportGenerator = new AnalyticsReportGenerator(collector);

            // Act
            var grouping = reportGenerator.GetItemsProcessedByDayOfWeek();

            // Assert:
            // The total sum of items grouped by day should equal the total sample count (4).
            Assert.NotEmpty(grouping);
            var totalItems = grouping.Values.Sum();
            Assert.Equal(4, totalItems);

            // Verify that the specific day (e.g., Wednesday, since 2025-01-01 is a Wednesday) has at least one data point.
            Assert.True(grouping.ContainsKey(DayOfWeek.Wednesday));
            Assert.True(grouping[DayOfWeek.Wednesday] >= 1);
        }
    }
}
