using System.Collections.Generic;

/// Zinq! It's like Linq.
/// Except it doesn't create a mountain of GC
namespace Zinq
{
    public static partial class Zinq<TSource>
    {
        /// <summary>
        /// Begin a new query on an enumerator
        /// </summary>
        /// <param name="_enumerator"></param>
        /// <typeparam name="TEnumerator"></typeparam>
        /// <returns></returns>
        public static ZinqHelper<TEnumerator> FromEnumerator<TEnumerator>(TEnumerator _enumerator)
            where TEnumerator : struct, IEnumerator<TSource>
        {
            return new ZinqHelper<TEnumerator>(_enumerator);
        }
    }
}