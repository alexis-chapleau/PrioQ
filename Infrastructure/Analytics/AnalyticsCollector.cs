using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;

namespace PrioQ.Infrastructure.Analytics
{
    public class AnalyticsCollector : IAnalyticsCollector
    {
        private readonly ConcurrentBag<AnalyticsDataPoint> _dataPoints = new ConcurrentBag<AnalyticsDataPoint>();

        public void RecordItemProcessed(AnalyticsDataPoint dataPoint)
        {
            _dataPoints.Add(dataPoint);
        }

        public IReadOnlyCollection<AnalyticsDataPoint> GetDataPoints()
        {
            // Return a snapshot of the collected data.
            return _dataPoints.ToArray();
        }
    }
}
