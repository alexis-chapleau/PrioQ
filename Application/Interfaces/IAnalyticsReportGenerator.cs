using System;
using System.Collections.Generic;

namespace PrioQ.Application.Interfaces
{
    public interface IAnalyticsReportGenerator
    {
        /// <summary>
        /// Computes the average time in queue for each priority level.
        /// </summary>
        IDictionary<int, TimeSpan> GetAverageTimeInQueueByPriority();

        /// <summary>
        /// Returns the total number of items processed between the specified times.
        /// </summary>
        /// <param name="from">The start time for the report.</param>
        /// <param name="to">The end time for the report.</param>
        int GetTotalItemsProcessed(DateTime from, DateTime to);

        /// <summary>
        /// Returns a count of processed items grouped by the day of the week.
        /// </summary>
        IDictionary<DayOfWeek, int> GetItemsProcessedByDayOfWeek();
    }
}
