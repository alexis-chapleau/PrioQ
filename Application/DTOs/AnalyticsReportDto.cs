using System;
using System.Collections.Generic;

namespace PrioQ.Application.DTOs
{
    public class AnalyticsReportDto
    {
        /// <summary>
        /// Average time in queue for items grouped by priority.
        /// </summary>
        public IDictionary<int, TimeSpan> AverageTimeInQueueByPriority { get; set; }

        /// <summary>
        /// Number of items processed per day of the week.
        /// </summary>
        public IDictionary<DayOfWeek, int> ItemsProcessedByDayOfWeek { get; set; }

        /// <summary>
        /// Total items processed within the specified time window.
        /// </summary>
        public int TotalItemsProcessed { get; set; }
    }
}
