namespace PrioQ.Domain.Entities
{
    public class QueueConfig
    {
        public bool UseBucketAlgorithm { get; set; }
        public bool UseAnalytics { get; set; }
        public bool UseLogging { get; set; }
        public bool UseLocking { get; set; }
        public bool UseLazyDelete { get; set; }
        public bool UseBitmaskOptimization { get; set; }
        public int MaxPriority { get; set; }
    }
}
