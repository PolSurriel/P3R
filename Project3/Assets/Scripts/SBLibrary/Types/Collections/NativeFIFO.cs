using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


namespace SurrealBoost
{
    namespace Types
    {
        public struct NativeFIFO<T> where T : struct
        {
            public NativeArray<T> data;

            private int firstIndex;
            private int lastIndex;

            private int _length;
            public int Length
            {
                get { return _length; }
            }

            public void Init(int maxCapacity, Allocator allocator = Allocator.Persistent)
            {
                firstIndex = 0;
                lastIndex = 0;
                data = new NativeArray<T>(maxCapacity, allocator);
            }

            public void Dispose()
            {
                data.Dispose();
            }

            public void Add(T element)
            {
                data[lastIndex] = element;
                lastIndex = (lastIndex + 1) % data.Length;
                _length++;
            }

            public void Pop()
            {
                if (Empty())
                {
                    Debug.LogError("The NativeFIFO is empty. You cannot use .Pop().");
                }
                firstIndex = (firstIndex + 1) % data.Length;
                _length--;



            }

            public bool Empty()
            {
                return firstIndex == lastIndex;
            }

            public T this[int i]
            {
                get
                {

                    int index = (firstIndex + i) % data.Length;

                    return data[index];
                }

                set
                {
                    int index = (firstIndex + i) % data.Length;
                    data[index] = value;

                }
            }




        }

    }

}