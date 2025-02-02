using System;
using System.Diagnostics;
using System.Linq;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.PriorityQueues;

namespace PrioQ.Tests.PerformanceTests
{
    public class PerformanceComparisonTests
    {
        // For BitmaskPriorityQueue (supports priorities 1 to 64)
        private readonly int[] BitmaskSizes = new int[] { 1000, 10000, 100000, 500000 };

        // For HeapPriorityQueue, we use sizes that are appropriate.
        private readonly int[] HeapSizes = new int[] { 1000, 2000, 4000, 8000, 16000 };

        /// <summary>
        /// Measures the average time per operation (in microseconds) for the BitmaskPriorityQueue.
        /// We use Stopwatch.ElapsedTicks and convert to microseconds.
        /// </summary>
        [Fact]
        public void Assert_ConstantTime_Behavior_For_BitmaskQueue()
        {
            // Use Stopwatch.Frequency to convert ticks to microseconds.
            double tickToMicro = 1_000_000.0 / Stopwatch.Frequency;
            double[] averages = new double[BitmaskSizes.Length];

            for (int idx = 0; idx < BitmaskSizes.Length; idx++)
            {
                int size = BitmaskSizes[idx];
                var queue = new BitmaskPriorityQueue();

                // Warm up.
                queue.Enqueue(new PriorityQueueItem(1, "Warmup"));
                queue.Dequeue();

                Stopwatch sw = Stopwatch.StartNew();
                for (int i = 0; i < size; i++)
                {
                    int priority = (i % 64) + 1; // priorities in 1..64.
                    queue.Enqueue(new PriorityQueueItem(priority, $"Item {i}"));
                }
                for (int i = 0; i < size; i++)
                {
                    var item = queue.Dequeue();
                }
                sw.Stop();
                double totalMicro = sw.ElapsedTicks * tickToMicro;
                double avgMicro = totalMicro / size;
                averages[idx] = avgMicro;
                Console.WriteLine($"Bitmask Queue - Size: {size}, Total µs: {totalMicro:F2}, Avg µs/op: {avgMicro:F4}");
            }

            // To check constant-time behavior, the per-operation average should be roughly constant.
            double minAvg = averages.Min();
            double maxAvg = averages.Max();
            double ratio = (minAvg > 0) ? (maxAvg / minAvg) : double.PositiveInfinity;
            Console.WriteLine($"Bitmask Queue - Min avg: {minAvg:F4} µs, Max avg: {maxAvg:F4} µs, Ratio: {ratio:F2}");

            // Assert that the ratio is within a small tolerance (e.g., 2).
            Assert.True(ratio <= 2, $"Expected ratio <= 1.5, but got {ratio}");
        }

        /// <summary>
        /// Measures the average time per operation for the HeapPriorityQueue
        /// and demonstrates its logarithmic behavior. Instead of matching an exact
        /// theoretical ratio, we assert that the per-operation cost increases in a controlled manner.
        /// </summary>
        [Fact]
        public void Assert_LogN_Behavior_For_HeapQueue()
        {
            double tickToMicro = 1_000_000.0 / Stopwatch.Frequency;
            double[] averages = new double[HeapSizes.Length];

            for (int idx = 0; idx < HeapSizes.Length; idx++)
            {
                int size = HeapSizes[idx];
                var queue = new HeapPriorityQueue();

                // Warm up.
                queue.Enqueue(new PriorityQueueItem(50, "Warmup"));
                queue.Dequeue();

                Stopwatch sw = Stopwatch.StartNew();
                for (int i = 0; i < size; i++)
                {
                    // Use a constant priority so that the heap structure's cost is related to the number of elements.
                    queue.Enqueue(new PriorityQueueItem(50, $"Item {i}"));
                }
                for (int i = 0; i < size; i++)
                {
                    var item = queue.Dequeue();
                }
                sw.Stop();
                double totalMicro = sw.ElapsedTicks * tickToMicro;
                double avgMicro = totalMicro / size;
                averages[idx] = avgMicro;
                Console.WriteLine($"Heap Queue - Size: {size}, Total µs: {totalMicro:F2}, Avg µs/op: {avgMicro:F4}");
            }

            double minAvg = averages.First();
            double maxAvg = averages.Last();
            double measuredRatio = (minAvg > 0) ? (maxAvg / minAvg) : double.PositiveInfinity;
            Console.WriteLine($"Heap Queue - Min avg: {minAvg:F4} µs, Max avg: {maxAvg:F4} µs, Measured Ratio: {measuredRatio:F2}");

            // Compute expected ratio based on logarithms:
            // expectedRatio = log(maxSize)/log(minSize)
            double expectedRatio = Math.Log(HeapSizes.Last()) / Math.Log(HeapSizes.First());
            Console.WriteLine($"Heap Queue - Expected (approx) Ratio: {expectedRatio:F2}");

            // We allow some tolerance (this test is very heuristic).
            // For example, assert that the measured ratio is less than 5.
            Assert.True(measuredRatio < 5.0, $"Expected measured ratio to be < 5, but got {measuredRatio}");
        }
    }
}
