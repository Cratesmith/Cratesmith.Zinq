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
            /// Ends the query and returns the first item (or default if no items exist)
            /// </summary>
            /// <param name="_outCollection"></param>
            /// <typeparam name="TCollection"></typeparam>
            public TSource FirstOrDefault() => enumerator.MoveNext() 
                ? enumerator.Current 
                : default;

            /// <summary>
            /// Ends the query and returns the first item that meets the predicate (or default if no items exist)
            /// </summary>
            /// <param name="_outCollection"></param>
            /// <typeparam name="TCollection"></typeparam>
            public TSource FirstOrDefault(ZPredicate<TSource> _predicate)
            {
                using (enumerator)
                {
                    while (enumerator.MoveNext())
                    {
                        if (_predicate(enumerator.Current)) return enumerator.Current;
                    }
                    return default;
                }
            }
            
            /// <summary>
            /// Ends the query and returns the first item that meets the predicate (or default if no items exist)
            /// </summary>
            /// <param name="_outCollection"></param>
            /// <typeparam name="TCollection"></typeparam>
            public TSource FirstOrDefault<TContext>(in TContext _context, ZFunc<TContext, TSource, bool> _predicate)
            {
                using (enumerator)
                {
                    while (enumerator.MoveNext())
                    {
                        if (_predicate(_context, enumerator.Current)) return enumerator.Current;
                    }
                    return default;
                }
            }
        }
    }
}