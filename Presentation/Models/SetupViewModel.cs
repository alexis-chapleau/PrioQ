﻿namespace PrioQ.UI.Models
{
    public class SetupViewModel
    {
        // Queue configuration.
        public bool UnboundedPriority { get; set; }
        public int MaxPriority { get; set; }
        public string Algorithm { get; set; } // e.g., "NormalBuckets", "Bitmask", "DoubleBitmask", "Heap"
        public bool UseLogging { get; set; }
        public bool UseLocking { get; set; }
        public bool UseLazyDelete { get; set; }
        public bool UseAnalytics { get; set; }
    }
}
