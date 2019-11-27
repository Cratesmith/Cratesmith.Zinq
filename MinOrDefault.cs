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
            public TSource MinOrDefault<TResult>(Func<TSource, TResult> _selector)
            {
                using (enumerator)
                {
                    var comparer = Comparer<TResult>.Default;
                    bool found = false;
                    TResult bestResult = default;
                    TSource best = default;
                    while (enumerator.MoveNext())
                    {
                        var currentResult = _selector(enumerator.Current);
                        if (!found || comparer.Compare(currentResult, bestResult) == -1)
                        {
                            found = true;
                            best = enumerator.Current;
                            bestResult = currentResult;
                        }
                    }

                    return best;
                }
            }
            
            public TSource MinOrDefault<TContext, TResult>(TContext _context, Func<TContext, TSource, TResult> _selector)
            {
                using (enumerator)
                {
                    var comparer = Comparer<TResult>.Default;
                    bool found = false;
                    TResult bestResult = default;
                    TSource best = default;
                    while (enumerator.MoveNext())
                    {
                        var currentResult = _selector(_context, enumerator.Current);
                        if (!found || comparer.Compare(currentResult, bestResult) == -1)
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
}