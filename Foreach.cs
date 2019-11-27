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
            /// Ends the query and performs an action to each item
            /// </summary>
            /// <param name="_outCollection"></param>
            /// <typeparam name="TCollection"></typeparam>
            public void Foreach(Action<TSource> _action)
            {
                using (enumerator)
                {
                    while (enumerator.MoveNext())
                    {
                        _action(enumerator.Current);
                    }    
                }
            }
            
            /// <summary>
            /// Ends the query and performs an action to each item
            /// </summary>
            /// <param name="_outCollection"></param>
            /// <typeparam name="TCollection"></typeparam>
            public void Foreach<TContext>(TContext _context, Action<TContext, TSource> _action)
            {
                using (enumerator)
                {
                    while (enumerator.MoveNext())
                    {
                        _action(_context, enumerator.Current);
                    }    
                }
                
            }
        }
    }
}