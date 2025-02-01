using System;
using PrioQ.Application.DTOs;
using PrioQ.Application.Interfaces;

namespace PrioQ.Application.UseCases
{
    public class AnalyticsReportUseCase : IAnalyticsReportUseCase
    {
        private readonly IAnalyticsReportGenerator _reportGenerator;

        public AnalyticsReportUseCase(IAnalyticsReportGenerator reportGenerator)
        {
            _reportGenerator = reportGenerator;
        }

        public AnalyticsReportDto GenerateReport(DateTime from, DateTime to)
        {
            var averageByPriority = _reportGenerator.GetAverageTimeInQueueByPriority();
            var processedByDay = _reportGenerator.GetItemsProcessedByDayOfWeek();
            var totalItems = _reportGenerator.GetTotalItemsProcessed(from, to);

            return new AnalyticsReportDto
            {
                AverageTimeInQueueByPriority = averageByPriority,
                ItemsProcessedByDayOfWeek = processedByDay,
                TotalItemsProcessed = totalItems
            };
        }
    }
}
