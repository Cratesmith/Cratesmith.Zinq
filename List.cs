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
        /// Begin a new query on a list
        /// </summary>
        /// <param name="_array"></param>
        /// <returns></returns>
        public static ZinqHelper<ZListEnumerator<TList>> FromList<TList>(TList _list)
            where TList:IList<TSource>
        {
            return new ZinqHelper<ZListEnumerator<TList>>(new ZListEnumerator<TList>(_list));
        }
        
        /// <summary>
        /// Enumerates a list by stepping through indices, simulating a for loop
        /// </summary>
        public struct ZListEnumerator<TList> : IEnumerator<TSource>
            where TList:IList<TSource>
        {
            readonly TList m_List;
            int m_NextIndex;
            TSource m_Current;
            int m_Count;

            public ZListEnumerator(TList _list)
            {
                m_NextIndex = 0;
                m_List = _list;
                m_Current = default;
                m_Count = m_List.Count;
            }

            public bool MoveNext()
            {
                if (m_Count != m_List.Count)
                {
                    throw new InvalidOperationException("List has been modified during iteration!");
                }
                
                if (m_NextIndex >= m_List.Count)
                {
                    m_Current = default;
                    return false;
                }

                m_Current = m_List[m_NextIndex];
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
        public static Zinq<TSource>.ZinqHelper<Zinq<TSource>.ZListEnumerator<List<TSource>>> Zinq<TSource>(this List<TSource> @this) => global::Zinq.Zinq<TSource>.FromList(@this);
    }
}