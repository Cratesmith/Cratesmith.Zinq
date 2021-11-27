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
            /// Ends the query and returns the final enumerator in an AsEnumerable wrapper
            /// Note: this wrapper will throw an exception if passed by interface. As that would create GC via interface boxing
            /// </summary>
            /// <returns></returns>
            public ZEnumerable<TEnumerator> AsEnumerable()
            {
                return new ZEnumerable<TEnumerator>(enumerator);
            }
        }
        
        public struct ZEnumerable<TEnumerator> : IEnumerable<TSource>
            where TEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            bool m_Used; 

            public ZEnumerable(in TEnumerator _enumerator)
            {
                m_Enumerator = _enumerator;
                m_Used = false;
            }

            public TEnumerator GetEnumerator()
            {
                if(m_Used) throw new InvalidOperationException("Attempting to perform multiple iteration on a Zinq query. This is not supported by design.");
                m_Used = true;

                return m_Enumerator;
            }
            IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => throw new InvalidOperationException("Attempting to access Zinq query via IEnumerable<T>. This would cause GC and is not allowed.");
            IEnumerator IEnumerable.GetEnumerator() => throw new InvalidOperationException("Attempting to access Zinq query via IEnumerable. This would cause GC and is not allowed.");
        }
    }
}