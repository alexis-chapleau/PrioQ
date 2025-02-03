using PrioQ.Domain.Entities;

namespace PrioQ.UI.Models
{
    public class SetupViewModel
    {
        // Queue configuration.
        public bool UnboundedPriority { get; set; }
        public int MaxPriority { get; set; }
        public PriorityQueueAlgorithm Algorithm { get; set; } // e.g., "NormalBuckets", "Bitmask", "DoubleBitmask", "Heap"
        public bool UseLogging { get; set; }
        public bool UseLocking { get; set; }
        public bool UseLazyDelete { get; set; }
        public bool UseAnalytics { get; set; }

        // New properties for UI feedback
        public bool QueueIsRunning { get; set; }
        public string QueueServerInfo { get; set; }
        public string QueueWarningMessage { get; set; }
    
    }
}
