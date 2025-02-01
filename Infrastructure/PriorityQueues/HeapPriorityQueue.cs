using System.Collections.Generic;
using PrioQ.Domain.Entities;
using PrioQ.Domain.Interfaces;

namespace PrioQ.Infrastructure.PriorityQueues
{
    public class HeapPriorityQueue : IPriorityQueue
    {
        private readonly List<PriorityQueueItem> _heap;

        public HeapPriorityQueue(QueueConfig config)
        {
            _heap = new List<PriorityQueueItem>();
        }

        public void Enqueue(PriorityQueueItem item)
        {
            _heap.Add(item);
            HeapifyUp(_heap.Count - 1);
        }

        public PriorityQueueItem Dequeue()
        {
            if (_heap.Count == 0)
                return null;

            var item = _heap[0];
            _heap[0] = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);
            HeapifyDown(0);
            return item;
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_heap[index].Priority < _heap[parent].Priority)
                {
                    Swap(index, parent);
                    index = parent;
                }
                else
                {
                    break;
                }
            }
        }

        private void HeapifyDown(int index)
        {
            int lastIndex = _heap.Count - 1;
            while (index < _heap.Count)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;

                if (leftChild <= lastIndex && _heap[leftChild].Priority < _heap[smallest].Priority)
                    smallest = leftChild;
                if (rightChild <= lastIndex && _heap[rightChild].Priority < _heap[smallest].Priority)
                    smallest = rightChild;
                if (smallest != index)
                {
                    Swap(index, smallest);
                    index = smallest;
                }
                else
                {
                    break;
                }
            }
        }

        private void Swap(int i, int j)
        {
            var temp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = temp;
        }
    }
}
