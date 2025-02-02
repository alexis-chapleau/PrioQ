using System;

namespace PrioQ.Domain.Exceptions
{
    public class PriorityOutOfRangeException : Exception
    {
        public int ProvidedPriority { get; }
        public int MaxAllowed { get; }

        public PriorityOutOfRangeException(int provided, int maxAllowed)
            : base($"Priority {provided} is out of range. It must be between 1 and {maxAllowed}.")
        {
            ProvidedPriority = provided;
            MaxAllowed = maxAllowed;
        }
    }
}
