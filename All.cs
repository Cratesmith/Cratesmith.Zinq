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
            /// Ends the query and returns true if any values meet the specified predicate
            /// </summary>
            /// <param name="_predicate"></param>
            /// <returns></returns>
            public bool All(Predicate<TSource> _predicate)
            {
                using (enumerator)
                {
                    while (enumerator.MoveNext())
                    {
                        if (!_predicate(enumerator.Current)) return false;
                    }

                    return true;
                }
            }

            /// <summary>
            /// Ends the query and returns true if any values meet the specified predicate
            /// </summary>
            /// <param name="_predicate"></param>
            /// <returns></returns>
            public bool All<TContext>(TContext _context, Func<TContext, TSource, bool> _predicate)
            {
                using (enumerator)
                {
                    while (enumerator.MoveNext())
                    {
                        if (!_predicate(_context, enumerator.Current)) return false;
                    }

                    return true;
                }
            }
        }
    }
}