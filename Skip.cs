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
            /// Skips the next n items
            /// </summary>
            /// <param name="_count"></param>
            public void Skip(int _count)
            {
                while (enumerator.MoveNext())
                {
                    if (_count <= 0) return;
                    --_count;
                }
                enumerator.Dispose();
            }
        }
    }
}