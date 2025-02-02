using System;

namespace PrioQ.Domain.Entities
{
    public class PriorityQueueItem
    {
        public Guid Id { get; }
        public int Priority { get; set; }

        public long InsertionOrder { get; set; }
        public string Command { get; set; }
        public DateTime EnqueuedAt { get; set; }

        public PriorityQueueItem(int priority, string command)
        {
            Id = Guid.NewGuid();
            Priority = priority;
            Command = command;
            EnqueuedAt = DateTime.UtcNow;
        }
    }
}
