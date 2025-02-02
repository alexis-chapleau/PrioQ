namespace PrioQ.Domain.Entities
{
    public enum PriorityQueueAlgorithm
    {
        NormalBuckets,
        Bitmask,        // Single-level (supports up to 64 priorities)
        DoubleBitmask,  // Supports up to 256 priorities
        Heap
    }
}
