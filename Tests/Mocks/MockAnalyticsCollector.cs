using System.Collections.Generic;
using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.Analytics;

namespace PrioQ.Tests.Mocks
{
    /// <summary>
    /// A Mock analytics collector that records every data point in memory.
    /// </summary>
    public class MockAnalyticsCollector : IAnalyticsCollector
    {
        public List<AnalyticsDataPoint> DataPoints { get; } = new List<AnalyticsDataPoint>();

        public void RecordItemProcessed(AnalyticsDataPoint dataPoint)
        {
            DataPoints.Add(dataPoint);
        }

        public IReadOnlyCollection<AnalyticsDataPoint> GetDataPoints()
        {
            return DataPoints.AsReadOnly();
        }
    }
}
