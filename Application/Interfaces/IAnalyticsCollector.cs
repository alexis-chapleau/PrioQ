using System.Collections.Generic;
using PrioQ.Domain.Entities;

namespace PrioQ.Application.Interfaces
{
    public interface IAnalyticsCollector
    {
        void RecordItemProcessed(AnalyticsDataPoint dataPoint);
        IReadOnlyCollection<AnalyticsDataPoint> GetDataPoints();
    }
}
