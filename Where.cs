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
            /// Where query.
            /// Only passes on items that pass the specified predicate
            /// </summary>
            /// <param name="_predicate"></param>
            /// <returns></returns>
            public ZinqHelper<ZWhereEnumerator<TEnumerator>> Where(Predicate<TSource> _predicate) 
            {
                var query = new ZWhereEnumerator<TEnumerator>(enumerator, _predicate);
                return new ZinqHelper<ZWhereEnumerator<TEnumerator>>(query);
            }

            /// <summary>
            /// Where query (with context invariant)
            /// Only passes on items that pass the specified predicate
            /// </summary>
            /// <param name="_predicate"></param>
            /// <returns></returns>
            public ZinqHelper<ZWhereContextEnumerator<TEnumerator, TContext>> Where<TContext>(TContext _context, Func<TContext, TSource, bool> _predicate) 
            {
                var query = new ZWhereContextEnumerator<TEnumerator, TContext>(enumerator, _context, _predicate);
                return new ZinqHelper<ZWhereContextEnumerator<TEnumerator, TContext>>(query);
            }
        }
        
        public struct ZWhereEnumerator<TEnumerator> : IEnumerator<TSource>
            where TEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            readonly Predicate<TSource> m_Predicate;

            public ZWhereEnumerator(TEnumerator _enumerator, Predicate<TSource> _predicate)
            {
                m_Enumerator = _enumerator;
                m_Predicate = _predicate;
            }

            public bool MoveNext()
            {
                while (m_Enumerator.MoveNext())
                {
                    if (m_Predicate(Current)) return true;
                }

                return false;
            }

            public void Reset() => m_Enumerator.Reset();
            public TSource Current => m_Enumerator.Current;
            object IEnumerator.Current => Current;
            public void Dispose() => m_Enumerator.Dispose();
        }

        public struct ZWhereContextEnumerator<TEnumerator, TContext> : IEnumerator<TSource>
            where TEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            readonly TContext m_Context;
            readonly Func<TContext,TSource, bool> m_Predicate;

            public ZWhereContextEnumerator(TEnumerator _enumerator, TContext _context, Func<TContext,TSource, bool> _predicate)
            {
                m_Enumerator = _enumerator;
                m_Predicate = _predicate;
                m_Context = _context;
            }

            public bool MoveNext()
            {
                while (m_Enumerator.MoveNext())
                {
                    if (m_Predicate(m_Context, Current)) return true;
                }

                return false;
            }

            public void Reset() => m_Enumerator.Reset();
            public TSource Current => m_Enumerator.Current;
            object IEnumerator.Current => Current;
            public void Dispose() => m_Enumerator.Dispose();
        }
    }
}