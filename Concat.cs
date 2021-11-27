using System;
using System.Collections;
using System.Collections.Generic;
 
/// Zinq! It's like Linq.
/// Except it doesn't create a mountain of GC
namespace Zinq
{
    public static partial class Zinq<TSource>
    {
        public partial struct ZinqHelper<TEnumerator>
            where TEnumerator : struct, IEnumerator<TSource>
        {
            /// <summary>
            /// Concat query
            /// Appends all items from the supplied enumerator.
            /// </summary>
            /// <param name="_otherEnumerator"></param>
            /// <typeparam name="TOtherEnumerator"></typeparam>
            /// <returns></returns>
            public ZinqHelper<ZConcatEnumerator<TEnumerator, TOtherEnumerator>> Concat<TOtherEnumerator>(TOtherEnumerator _otherEnumerator) where TOtherEnumerator : struct, IEnumerator<TSource>
            {
                var query = new ZConcatEnumerator<TEnumerator, TOtherEnumerator>(enumerator, _otherEnumerator);
                return new ZinqHelper<ZConcatEnumerator<TEnumerator, TOtherEnumerator>>(query);
            }
        }
        
        public struct ZConcatEnumerator<TEnumerator, TOtherEnumerator> : IEnumerator<TSource> 
            where TEnumerator: struct, IEnumerator<TSource>
            where TOtherEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            TOtherEnumerator m_OtherEnumerator;
            TSource m_Current;

            public ZConcatEnumerator(in TEnumerator _enumerator, TOtherEnumerator _otherEnumerator)
            {
                m_Enumerator = _enumerator;
                m_OtherEnumerator = _otherEnumerator;
                m_Current = default;
            }

            public bool MoveNext()
            {
                if (m_Enumerator.MoveNext())
                {
                    m_Current = m_Enumerator.Current;
                    return true;
                }
                m_Enumerator.Dispose();

                if (m_OtherEnumerator.MoveNext())
                {
                    m_Current = m_OtherEnumerator.Current;
                    return true;
                }
                
                m_OtherEnumerator.Dispose();
                m_Current = default;
                return false;
            }
            
            public void Reset()
            {
                m_Enumerator.Reset();
                m_OtherEnumerator.Reset();
                m_Current = default;
            }

            public TSource Current => m_Current;
            object IEnumerator.Current => Current;
            public void Dispose()
            {
                m_Enumerator.Dispose();
                m_OtherEnumerator.Dispose();
            }
        }
    }
}