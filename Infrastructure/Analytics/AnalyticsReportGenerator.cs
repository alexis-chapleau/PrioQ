using System;
using System.Collections.Generic;
using System.Linq;
using PrioQ.Application.Interfaces;

namespace PrioQ.Infrastructure.Analytics
{
    public class AnalyticsReportGenerator : IAnalyticsReportGenerator
    {
        private readonly IAnalyticsCollector _collector;

        public AnalyticsReportGenerator(IAnalyticsCollector collector)
        {
            _collector = collector;
        }

        /// <summary>
        /// Computes the average time in queue for each priority level.
        /// </summary>
        public IDictionary<int, TimeSpan> GetAverageTimeInQueueByPriority()
        {
            var dataPoints = _collector.GetDataPoints();
            return dataPoints
                .GroupBy(dp => dp.Priority)
                .ToDictionary(
                    g => g.Key,
                    g => TimeSpan.FromTicks((long)g.Average(dp => dp.TimeInQueue.Ticks))
                );
        }

        /// <summary>
        /// Returns the total number of items processed between the specified times.
        /// </summary>
        public int GetTotalItemsProcessed(DateTime from, DateTime to)
        {
            var dataPoints = _collector.GetDataPoints();
            return dataPoints.Count(dp => dp.DequeuedAt >= from && dp.DequeuedAt <= to);
        }

        /// <summary>
        /// Returns a count of processed items grouped by the day of the week.
        /// </summary>
        public IDictionary<DayOfWeek, int> GetItemsProcessedByDayOfWeek()
        {
            var dataPoints = _collector.GetDataPoints();
            return dataPoints
                .GroupBy(dp => dp.DequeuedAt.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Additional report methods can be added here.
    }
}
