using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrioQ.Domain.Entities;

namespace PrioQ.Domain.Entities
{
    public abstract class BasePriorityQueue
    {
        public bool UseLogging { get; set; }
        public bool UseLocking { get; set; }
        public bool UseLazyDelete { get; set; }
        public bool UseAnalytics { get; set; }
        public int MaxPriority { get; set; }
        public PriorityQueueAlgorithm Algorithm { get; set; }
        public bool UnboundedPriority { get; set; }

        public abstract void Enqueue(PriorityQueueItem item);
        public abstract PriorityQueueItem Dequeue();
    }
}
