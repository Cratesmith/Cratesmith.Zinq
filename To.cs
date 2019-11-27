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
            /// Ends the query and copies all values to an existing collection
            /// </summary>
            /// <param name="_outCollection"></param>
            /// <typeparam name="TCollection"></typeparam>
            public TCollection To<TCollection>(TCollection _outCollection) where TCollection:ICollection<TSource>
            {
                while (enumerator.MoveNext())
                {
                    _outCollection.Add(enumerator.Current);
                }

                return _outCollection;
            }
            
            /// <summary>
            /// Ends the query and copies all values to an existing collection
            /// </summary>
            /// <param name="_outCollection"></param>
            /// <typeparam name="TCollection"></typeparam>
            public TCollection To<TCollection>(TCollection _outCollection, int _count) where TCollection:ICollection<TSource>
            {
                while (enumerator.MoveNext() && _count>0)
                {
                    _outCollection.Add(enumerator.Current);
                    --_count;
                }
                
                return _outCollection;
            }
        }
    }
}