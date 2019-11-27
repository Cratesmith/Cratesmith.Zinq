using System;
using System.Collections;
using System.Collections.Generic;
 
/// Zinq! It's like Linq.
/// Except it doesn't create a mountain of GC
namespace Zinq
{
    public static partial class Zinq<TSource>
    {
        /// <summary>
        /// Begin a new query on an array
        /// </summary>
        /// <param name="_array"></param>
        /// <returns></returns>
        public static ZinqHelper<ZArrayEnumerator> FromArray(TSource[] _array)
        {
            return new ZinqHelper<ZArrayEnumerator>(new ZArrayEnumerator(_array));
        }

        /// <summary>
        /// Enumerates an array by stepping through indices, simulating a for loop
        /// </summary>
        public struct ZArrayEnumerator : IEnumerator<TSource>
        {
            readonly TSource[] m_Array;
            int m_NextIndex;
            TSource m_Current;

            public ZArrayEnumerator(TSource[] _array)
            {
                m_NextIndex = 0;
                m_Array = _array;
                m_Current = default;
            }

            public bool MoveNext()
            {
                if (m_NextIndex >= m_Array.Length)
                {
                    m_Current = default;
                    return false;
                }

                m_Current = m_Array[m_NextIndex];
                ++m_NextIndex;
                return true;
            }

            public void Reset()
            {
                m_NextIndex = 0;
            }

            public TSource Current => m_Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                Reset();
            }
        }
    }

    public static partial class ZinqExtensions
    {
        public static Zinq<TSource>.ZinqHelper<Zinq<TSource>.ZListEnumerator<TSource[]>> Zinq<TSource>(this TSource[] @this) => global::Zinq.Zinq<TSource>.FromList(@this);
    }
}
