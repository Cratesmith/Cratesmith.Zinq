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
            /// Ends the query and returns the final enumerator for the query
            /// </summary>
            /// <returns></returns>
            public TEnumerator AsEnumerator() => enumerator;
        }
    }
}