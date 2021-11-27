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
            /// SelectMany query
            /// Iterates multiple enumerators as if they were a single collection, applying the selector function to all items
            /// ** NOTE ** You may likely to specify the argument type of the selector! For example: (int x)=>x instead of x=>x  
            /// </summary>
            /// <param name="_enumerator"></param>
            /// <param name="_selector"></param>
            /// <typeparam name="TInnerEnumerator"></typeparam>
            /// <typeparam name="TInnerSource"></typeparam>
            /// <typeparam name="TResult"></typeparam>
            /// <returns></returns>
            public Zinq<TResult>.ZinqHelper<ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult>> 
                SelectMany<TInnerEnumerator, TInnerSource, TResult>(ZFunc<TSource, TInnerEnumerator> _enumerator, ZFunc<TInnerSource, TResult> _selector)  
                where TInnerEnumerator:IEnumerator<TInnerSource>
            {
                var query = new ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult>(enumerator, _enumerator, _selector);
                return new Zinq<TResult>.ZinqHelper<ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult>>(query);
            }

            /// <summary>
            /// SelectMany query
            /// Iterates multiple enumerators as if they were a single collection, applying the selector function to all items
            /// ** NOTE ** You may likely to specify the argument type of the selector! For example: (int x)=>x instead of x=>x  
            /// </summary>
            /// <param name="_context"></param>
            /// <param name="_enumerator"></param>
            /// <param name="_selector"></param>
            /// <typeparam name="TInnerEnumerator"></typeparam>
            /// <typeparam name="TInnerSource"></typeparam>
            /// <typeparam name="TResult"></typeparam>
            /// <typeparam name="TContext"></typeparam>
            /// <returns></returns>
            public Zinq<TResult>.ZinqHelper<ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult, TContext>> 
                SelectMany<TInnerEnumerator, TInnerSource, TResult, TContext>(in TContext _context, ZFunc<TContext, TSource, TInnerEnumerator> _enumerator, ZFunc<TContext, TInnerSource, TResult> _selector)  
                where TInnerEnumerator:IEnumerator<TInnerSource>
            {
                var query = new ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult, TContext>(enumerator, _context, _enumerator, _selector);
                return new Zinq<TResult>.ZinqHelper<ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult, TContext>>(query);
            }
        }
        
        public struct ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult> : IEnumerator<TResult> 
            where TEnumerator: struct, IEnumerator<TSource>
            where TInnerEnumerator:IEnumerator<TInnerSource>
        {
            TEnumerator m_Enumerator;
            TInnerEnumerator m_EnumeratorInner;
            readonly ZFunc<TInnerSource, TResult> m_ResultSelector;
            readonly ZFunc<TSource, TInnerEnumerator> m_EnumeratorSelector;
            TResult m_Current;
            bool m_HasInnerEnumerator;

            public ZSelectManyEnumerator(in TEnumerator _enumerator, ZFunc<TSource, TInnerEnumerator> _enumeratorSelector, ZFunc<TInnerSource, TResult> _resultSelector)
            {
                m_Enumerator = _enumerator;
                m_EnumeratorSelector = _enumeratorSelector;
                m_ResultSelector = _resultSelector;
                m_Current = default;
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;
            }

            public bool MoveNext()
            {
                if (MoveNextInner())
                {
                    return true;
                }
                            
                if (m_Enumerator.MoveNext())
                {
                    m_EnumeratorInner = m_EnumeratorSelector.Invoke(m_Enumerator.Current);
                    m_HasInnerEnumerator = true;
                    if (MoveNextInner())
                    {
                        return true;
                    }
                }
                m_Enumerator.Dispose();
                m_Current = default;
                return false;
            }

            bool MoveNextInner()
            {
                if (!m_HasInnerEnumerator)
                {
                    return false;
                }
                
                if (m_EnumeratorInner.MoveNext())
                {
                    m_Current = m_ResultSelector.Invoke(m_EnumeratorInner.Current);
                    return true;
                }

                m_EnumeratorInner.Dispose();
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;
                return false;
            }

            public void Reset()
            {
                m_Enumerator.Reset();
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;
            }

            public TResult Current => m_Current;
            object IEnumerator.Current => Current;
            public void Dispose()
            {
                m_EnumeratorInner.Dispose();
                m_Enumerator.Dispose();
            }
        }
    
        public struct ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult, TContext> : IEnumerator<TResult> 
            where TEnumerator: struct, IEnumerator<TSource>
            where TInnerEnumerator:IEnumerator<TInnerSource>
        {
            TEnumerator m_Enumerator;
            TInnerEnumerator m_EnumeratorInner;
            readonly ZFunc<TContext, TInnerSource, TResult> m_ResultSelector;
            TResult m_Current;
            ZFunc<TContext, TSource, TInnerEnumerator> m_EnumeratorSelector;
            TContext m_Context;
            bool m_HasInnerEnumerator;

            public ZSelectManyEnumerator(in TEnumerator _enumerator, in TContext _context, ZFunc<TContext, TSource, TInnerEnumerator> _enumeratorSelector, ZFunc<TContext, TInnerSource, TResult> _resultSelector)
            {
                m_Enumerator = _enumerator;
                m_EnumeratorSelector = _enumeratorSelector;
                m_ResultSelector = _resultSelector;
                m_Current = default;
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;
                m_Context = _context;
            }

            public bool MoveNext()
            {
                if (MoveNextInner())
                {
                    return true;
                }
                            
                if (m_Enumerator.MoveNext())
                {
                    m_EnumeratorInner = m_EnumeratorSelector.Invoke(m_Context, m_Enumerator.Current);
                    m_HasInnerEnumerator = true;
                    if (MoveNextInner())
                    {
                        return true;
                    }
                }

                m_Enumerator.Dispose();
                m_Current = default;
                return false;
            }

            bool MoveNextInner()
            {
                if (!m_HasInnerEnumerator)
                {
                    return false;
                }
             
                if (m_EnumeratorInner.MoveNext())
                {
                    m_Current = m_ResultSelector.Invoke(m_Context, m_EnumeratorInner.Current);
                    return true;
                }

                m_EnumeratorInner.Dispose();
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;
                return false;
            }

            public void Reset()
            {
                m_Enumerator.Reset();
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;
            }

            public TResult Current => m_Current;
            object IEnumerator.Current => Current;
            public void Dispose()
            {
                m_EnumeratorInner.Dispose();
                m_Enumerator.Dispose();
            }
        }
    }
}