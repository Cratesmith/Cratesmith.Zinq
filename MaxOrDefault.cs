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
            /// Iterate through all items and return the one with the highest value via a selector func.
            /// </summary>
            /// <param name="_selector"></param>
            /// <typeparam name="TResult"></typeparam>
            /// <returns></returns>
            public TSource MaxOrDefault<TResult>(Func<TSource, TResult> _selector)
            {
                var comparer = Comparer<TResult>.Default;
                bool found = false;
                TResult bestResult = default;
                TSource best = default;
                while (enumerator.MoveNext())
                {
                    var currentResult = _selector(enumerator.Current);
                    if (!found || comparer.Compare(currentResult, bestResult) == 1)
                    {
                        found = true;
                        best = enumerator.Current;
                        bestResult = currentResult;
                    }
                }

                return best;
            }

            /// <summary>
            /// Iterate through all items and return the one with the highest value via a selector func.
            /// </summary>
            /// <param name="_selector"></param>
            /// <typeparam name="TResult"></typeparam>
            /// <returns></returns>
            public TSource MaxOrDefault<TContext, TResult>(TContext _context,
                Func<TContext, TSource, TResult> _selector)
            {
                var comparer = Comparer<TResult>.Default;
                bool found = false;
                TResult bestResult = default;
                TSource best = default;
                while (enumerator.MoveNext())
                {
                    var currentResult = _selector(_context, enumerator.Current);
                    if (!found || comparer.Compare(currentResult, bestResult) == 1)
                    {
                        found = true;
                        best = enumerator.Current;
                        bestResult = currentResult;
                    }
                }

                return best;
            }
        }
    }
}