﻿using System;
using System.Collections;
using System.Collections.Generic;
 
/// Zinq! It's like Linq.
/// Except it doesn't create a mountain of GC
namespace Zinq
{
    public static partial class Zinq<TSource>
    {
        public delegate bool ZPredicate<TSource>(in TSource source);  
        public delegate bool ZPredicate<TContext, TSource>(in TContext context, in TSource source);
        public delegate TResult ZFunc<TSource, TResult>(in TSource source);
        public delegate TResult ZFunc<TContext, TSource, TResult>(in TContext context, in TSource source);
        
       public partial struct ZinqHelper<TEnumerator>
            where TEnumerator: struct,IEnumerator<TSource>
        {
            TEnumerator enumerator;
            public ZinqHelper(in TEnumerator _enumerator) => enumerator = _enumerator;
            
            public static implicit operator TEnumerator(ZinqHelper<TEnumerator> @this) => @this.enumerator;
            public static implicit operator ZinqHelper<TEnumerator>(in TEnumerator @this) => new ZinqHelper<TEnumerator>(@this);
        }
    }
}