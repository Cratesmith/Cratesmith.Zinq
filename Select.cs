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
            /// Select query
            /// Applies the selector function to all items and passes along the result values
            /// </summary>
            /// <param name="_selector"></param>
            /// <typeparam name="TResult"></typeparam>
            /// <returns></returns>
            public Zinq<TResult>.ZinqHelper<ZSelectEnumerator<TEnumerator, TResult>> Select<TResult>(ZFunc<TSource,TResult> _selector) 
            {
                var query = new ZSelectEnumerator<TEnumerator, TResult>(enumerator, _selector);
                return new Zinq<TResult>.ZinqHelper<ZSelectEnumerator<TEnumerator, TResult>>(query);
            }
            
            /// <summary>
            /// Select query (with context invariant)
            /// Applies the selector function to all items and passes along the result values
            /// </summary>
            /// <param name="_selector"></param>
            /// <typeparam name="TResult"></typeparam>
            /// <returns></returns>
            public Zinq<TResult>.ZinqHelper<ZSelectContextEnumerator<TEnumerator, TContext, TResult>> Select<TContext, TResult>(in TContext _context, ZFunc<TContext, TSource, TResult> _selector) 
            {
                var query = new ZSelectContextEnumerator<TEnumerator, TContext, TResult>(enumerator, _context, _selector);
                return new Zinq<TResult>.ZinqHelper<ZSelectContextEnumerator<TEnumerator, TContext, TResult>>(query);
            }
        }
        
        public struct ZSelectEnumerator<TEnumerator, TResult> : IEnumerator<TResult> 
            where TEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            readonly ZFunc<TSource, TResult> m_Selector;
            TResult m_Current;

            public ZSelectEnumerator(in TEnumerator _enumerator, ZFunc<TSource, TResult> _selector)
            {
                m_Enumerator = _enumerator;
                m_Selector = _selector;
                m_Current = default;
            }

            public bool MoveNext()
            {
                if (m_Enumerator.MoveNext())
                {
                    m_Current = m_Selector(m_Enumerator.Current);
                    return true;
                }

                m_Enumerator.Dispose();
                m_Current = default;
                return false;
            }
            
            public void Reset() => m_Enumerator.Reset();
            public TResult Current => m_Current;
            object IEnumerator.Current => Current;
            public void Dispose() => m_Enumerator.Dispose();
        }
    
        public struct ZSelectContextEnumerator<TEnumerator, TContext, TResult> : IEnumerator<TResult> 
            where TEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            readonly ZFunc<TContext, TSource, TResult> m_Selector;
            TResult m_Current;
            TContext m_Context;

            public ZSelectContextEnumerator(in TEnumerator _enumerator, in TContext _context, ZFunc<TContext, TSource, TResult> _selector)
            {
                m_Enumerator = _enumerator;
                m_Selector = _selector;
                m_Current = default;
                m_Context = _context;
            }

            public bool MoveNext()
            {
                if (m_Enumerator.MoveNext())
                {
                    m_Current = m_Selector(m_Context, m_Enumerator.Current);
                    return true;
                }

                m_Enumerator.Dispose();
                m_Current = default;
                return false;
            }
            
            public void Reset() => m_Enumerator.Reset();
            public TResult Current => m_Current;
            object IEnumerator.Current => Current;
            public void Dispose() => m_Enumerator.Dispose();
        }
    }
}