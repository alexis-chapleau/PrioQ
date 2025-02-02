namespace PrioQ.Domain.Entities
{
    public class QueueConfig
    {
        public bool UseLogging { get; set; }
        public bool UseLocking { get; set; }
        public bool UseLazyDelete { get; set; }
        public bool UseAnalytics { get; set; }

        public bool UnboundedPriority { get; set; }
        public int MaxPriority { get; set; }  // For bounded priorities

        public PriorityQueueAlgorithm Algorithm { get; set; }
    }
}
