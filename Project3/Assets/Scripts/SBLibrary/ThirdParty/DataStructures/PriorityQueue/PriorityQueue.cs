using System;
using DataStructures.FibonacciHeap;

namespace DataStructures.PriorityQueue
{
    public class PriorityQueue<TElement, TPriority> : IPriorityQueue<TElement, TPriority>
        where TPriority : IComparable<TPriority>
    {
        private readonly FibonacciHeap<TElement, TPriority> heap;

        private int lenght;

        public bool Empty()
        {
            return lenght<=0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="minPriority">Minimum value of the priority - to be used for comparing.</param>
        public PriorityQueue(TPriority minPriority)
        {
            heap = new FibonacciHeap<TElement, TPriority>(minPriority);
        }

        public void Insert(TElement item, TPriority priority)
        {

            lenght++;

            heap.Insert(new FibonacciHeapNode<TElement, TPriority>(item, priority));
        }

        public TElement Top()
        {
            return heap.Min().Data;
        }

        public TElement Pop()
        {
            lenght--;
            return heap.RemoveMin().Data;
        }
    }
}


