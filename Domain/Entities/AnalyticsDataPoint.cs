using System;

namespace PrioQ.Domain.Entities
{
    public class AnalyticsDataPoint
    {
        public Guid ItemId { get; set; }
        public int Priority { get; set; }
        public DateTime EnqueuedAt { get; set; }
        public DateTime DequeuedAt { get; set; }
        public TimeSpan TimeInQueue => DequeuedAt - EnqueuedAt;
    }
}
