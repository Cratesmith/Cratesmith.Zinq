﻿using System;
using System.Collections;
using System.Collections.Generic;
 
/// Zinq! It's like Linq.
/// Except it doesn't create a mountain of GC
namespace Zinq
{
    public static class Zinq<TSource> 
    {
        public struct ZEnumerable<TEnumerator> : IEnumerable<TSource>
        where TEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            bool m_Used; 

            public ZEnumerable(TEnumerator _enumerator)
            {
                m_Enumerator = _enumerator;
                m_Used = false;
            }

            public TEnumerator GetEnumerator()
            {
                if(m_Used) throw new InvalidOperationException("Attempting to perform multiple iteration on a Zinq query. This is not supported by design.");
                m_Used = true;

                return m_Enumerator;
            }
            IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => throw new InvalidOperationException("Attempting to access Zinq query via IEnumerable<T>. This would cause GC and is not allowed.");
            IEnumerator IEnumerable.GetEnumerator() => throw new InvalidOperationException("Attempting to access Zinq query via IEnumerable. This would cause GC and is not allowed.");
        }
    
        /// <summary>
        /// Enumerates an array by stepping through indices, simulating a for loop
        /// </summary>
        public struct ZArrayEnumerator : IEnumerator<TSource>
        {
            readonly TSource[] m_Array;
            int m_NextIndex;
            TSource m_Current;

            public ZArrayEnumerator(TSource[] _array)
            {
                m_NextIndex = 0;
                m_Array = _array;
                m_Current = default;
            }

            public bool MoveNext()
            {
                if (m_NextIndex >= m_Array.Length)
                {
                    m_Current = default;
                    return false;
                }

                m_Current = m_Array[m_NextIndex];
                ++m_NextIndex;
                return true;
            }

            public void Reset()
            {
                m_NextIndex = 0;
            }

            public TSource Current => m_Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                Reset();
            }
        }
    
        /// <summary>
        /// Enumerates a list by stepping through indices, simulating a for loop
        /// </summary>
        public struct ZListEnumerator<TList> : IEnumerator<TSource>
            where TList:IList<TSource>
        {
            readonly TList m_List;
            int m_NextIndex;
            TSource m_Current;
            int m_Count;

            public ZListEnumerator(TList _list)
            {
                m_NextIndex = 0;
                m_List = _list;
                m_Current = default;
                m_Count = m_List.Count;
            }

            public bool MoveNext()
            {
                if (m_Count != m_List.Count)
                {
                    throw new InvalidOperationException("List has been modified during iteration!");
                }
                
                if (m_NextIndex >= m_List.Count)
                {
                    m_Current = default;
                    return false;
                }

                m_Current = m_List[m_NextIndex];
                ++m_NextIndex;
                return true;
            }

            public void Reset()
            {
                m_NextIndex = 0;
            }

            public TSource Current => m_Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                Reset();
            }
        }

        public struct ZWhereEnumerator<TEnumerator> : IEnumerator<TSource>
            where TEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            readonly Predicate<TSource> m_Predicate;

            public ZWhereEnumerator(TEnumerator _enumerator, Predicate<TSource> _predicate)
            {
                m_Enumerator = _enumerator;
                m_Predicate = _predicate;
            }

            public bool MoveNext()
            {
                while (m_Enumerator.MoveNext())
                {
                    if (m_Predicate(Current)) return true;
                }

                return false;
            }

            public void Reset() => m_Enumerator.Reset();
            public TSource Current => m_Enumerator.Current;
            object IEnumerator.Current => Current;
            public void Dispose() => m_Enumerator.Dispose();
        }

        public struct ZWhereContextEnumerator<TEnumerator, TContext> : IEnumerator<TSource>
            where TEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            readonly TContext m_Context;
            readonly Func<TContext,TSource, bool> m_Predicate;

            public ZWhereContextEnumerator(TEnumerator _enumerator, TContext _context, Func<TContext,TSource, bool> _predicate)
            {
                m_Enumerator = _enumerator;
                m_Predicate = _predicate;
                m_Context = _context;
            }

            public bool MoveNext()
            {
                while (m_Enumerator.MoveNext())
                {
                    if (m_Predicate(m_Context, Current)) return true;
                }

                return false;
            }

            public void Reset() => m_Enumerator.Reset();
            public TSource Current => m_Enumerator.Current;
            object IEnumerator.Current => Current;
            public void Dispose() => m_Enumerator.Dispose();
        }
    
        public struct ZSelectEnumerator<TEnumerator, TResult> : IEnumerator<TResult> 
            where TEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            readonly Func<TSource, TResult> m_Selector;
            TResult m_Current;

            public ZSelectEnumerator(TEnumerator _enumerator, Func<TSource, TResult> _selector)
            {
                m_Enumerator = _enumerator;
                m_Selector = _selector;
                m_Current = default;
            }

            public bool MoveNext()
            {
                if (m_Enumerator.MoveNext())
                {
                    m_Current = m_Selector(m_Enumerator.Current);
                    return true;
                }

                m_Current = default;
                return false;
            }
            
            public void Reset() => m_Enumerator.Reset();
            public TResult Current => m_Current;
            object IEnumerator.Current => Current;
            public void Dispose() => m_Enumerator.Dispose();
        }
    
        public struct ZSelectContextEnumerator<TEnumerator, TContext, TResult> : IEnumerator<TResult> 
            where TEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            readonly Func<TContext, TSource, TResult> m_Selector;
            TResult m_Current;
            TContext m_Context;

            public ZSelectContextEnumerator(TEnumerator _enumerator, TContext _context, Func<TContext, TSource, TResult> _selector)
            {
                m_Enumerator = _enumerator;
                m_Selector = _selector;
                m_Current = default;
                m_Context = _context;
            }

            public bool MoveNext()
            {
                if (m_Enumerator.MoveNext())
                {
                    m_Current = m_Selector(m_Context, m_Enumerator.Current);
                    return true;
                }

                m_Current = default;
                return false;
            }
            
            public void Reset() => m_Enumerator.Reset();
            public TResult Current => m_Current;
            object IEnumerator.Current => Current;
            public void Dispose() => m_Enumerator.Dispose();
        }
        public struct ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult> : IEnumerator<TResult> 
            where TEnumerator: struct, IEnumerator<TSource>
            where TInnerEnumerator:IEnumerator<TInnerSource>
        {
            TEnumerator m_Enumerator;
            TInnerEnumerator m_EnumeratorInner;
            readonly Func<TInnerSource, TResult> m_ResultSelector;
            readonly Func<TSource, TInnerEnumerator> m_EnumeratorSelector;
            TResult m_Current;
            bool m_HasInnerEnumerator;

            public ZSelectManyEnumerator(TEnumerator _enumerator, Func<TSource, TInnerEnumerator> _enumeratorSelector, Func<TInnerSource, TResult> _resultSelector)
            {
                m_Enumerator = _enumerator;
                m_EnumeratorSelector = _enumeratorSelector;
                m_ResultSelector = _resultSelector;
                m_Current = default;
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;
            }

            public bool MoveNext()
            {
                if (MoveNextInner())
                {
                    return true;
                }
                            
                if (m_Enumerator.MoveNext())
                {
                    m_EnumeratorInner = m_EnumeratorSelector.Invoke(m_Enumerator.Current);
                    m_HasInnerEnumerator = true;
                    if (MoveNextInner())
                    {
                        return true;
                    }
                }
                
                m_Current = default;
                return false;
            }

            bool MoveNextInner()
            {
                if (!m_HasInnerEnumerator)
                {
                    return false;
                }
                
                if (m_EnumeratorInner.MoveNext())
                {
                    m_Current = m_ResultSelector.Invoke(m_EnumeratorInner.Current);
                    return true;
                }

                m_EnumeratorInner.Dispose();
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;

                return false;
            }

            public void Reset()
            {
                m_Enumerator.Reset();
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;
            }

            public TResult Current => m_Current;
            object IEnumerator.Current => Current;
            public void Dispose()
            {
                m_EnumeratorInner.Dispose();
                m_Enumerator.Dispose();
            }
        }
    
        public struct ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult, TContext> : IEnumerator<TResult> 
            where TEnumerator: struct, IEnumerator<TSource>
            where TInnerEnumerator:IEnumerator<TInnerSource>
        {
            TEnumerator m_Enumerator;
            TInnerEnumerator m_EnumeratorInner;
            readonly Func<TContext, TInnerSource, TResult> m_ResultSelector;
            TResult m_Current;
            Func<TContext, TSource, TInnerEnumerator> m_EnumeratorSelector;
            TContext m_Context;
            bool m_HasInnerEnumerator;

            public ZSelectManyEnumerator(TEnumerator _enumerator, TContext _context, Func<TContext, TSource, TInnerEnumerator> _enumeratorSelector, Func<TContext, TInnerSource, TResult> _resultSelector)
            {
                m_Enumerator = _enumerator;
                m_EnumeratorSelector = _enumeratorSelector;
                m_ResultSelector = _resultSelector;
                m_Current = default;
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;
                m_Context = _context;
            }

            public bool MoveNext()
            {
                if (MoveNextInner())
                {
                    return true;
                }
                            
                if (m_Enumerator.MoveNext())
                {
                    m_EnumeratorInner = m_EnumeratorSelector.Invoke(m_Context, m_Enumerator.Current);
                    m_HasInnerEnumerator = true;
                    if (MoveNextInner())
                    {
                        return true;
                    }
                }

                m_Current = default;
                return false;
            }

            bool MoveNextInner()
            {
                if (!m_HasInnerEnumerator)
                {
                    return false;
                }
             
                if (m_EnumeratorInner.MoveNext())
                {
                    m_Current = m_ResultSelector.Invoke(m_Context, m_EnumeratorInner.Current);
                    return true;
                }

                m_EnumeratorInner.Dispose();
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;
                return false;
            }

            public void Reset()
            {
                m_Enumerator.Reset();
                m_EnumeratorInner = default;
                m_HasInnerEnumerator = false;
            }

            public TResult Current => m_Current;
            object IEnumerator.Current => Current;
            public void Dispose()
            {
                m_EnumeratorInner.Dispose();
                m_Enumerator.Dispose();
            }
        }
        
        public struct ZConcatEnumerator<TEnumerator, TOtherEnumerator> : IEnumerator<TSource> 
            where TEnumerator: struct, IEnumerator<TSource>
            where TOtherEnumerator: struct, IEnumerator<TSource>
        {
            TEnumerator m_Enumerator;
            TOtherEnumerator m_OtherEnumerator;
            TSource m_Current;

            public ZConcatEnumerator(TEnumerator _enumerator, TOtherEnumerator _otherEnumerator)
            {
                m_Enumerator = _enumerator;
                m_OtherEnumerator = _otherEnumerator;
                m_Current = default;
            }

            public bool MoveNext()
            {
                if (m_Enumerator.MoveNext())
                {
                    m_Current = m_Enumerator.Current;
                    return true;
                }

                if (m_OtherEnumerator.MoveNext())
                {
                    m_Current = m_OtherEnumerator.Current;
                    return true;
                }
                
                m_Current = default;
                return false;
            }
            
            public void Reset()
            {
                m_Enumerator.Reset();
                m_OtherEnumerator.Reset();
                m_Current = default;
            }

            public TSource Current => m_Current;
            object IEnumerator.Current => Current;
            public void Dispose() => m_Enumerator.Dispose();
        }
        
        public struct ZinqHelper<TEnumerator>
            where TEnumerator: struct,IEnumerator<TSource>
        {
            TEnumerator enumerator;
            public ZinqHelper(TEnumerator _enumerator) => enumerator = _enumerator;
            
            public static implicit operator TEnumerator(ZinqHelper<TEnumerator> @this) => @this.enumerator;
            public static implicit operator ZinqHelper<TEnumerator>(TEnumerator @this) => new ZinqHelper<TEnumerator>(@this);
            
            /// <summary>
            /// Where query.
            /// Only passes on items that pass the specified predicate
            /// </summary>
            /// <param name="_predicate"></param>
            /// <returns></returns>
            public ZinqHelper<ZWhereEnumerator<TEnumerator>> Where(Predicate<TSource> _predicate) 
            {
                var query = new ZWhereEnumerator<TEnumerator>(enumerator, _predicate);
                return new ZinqHelper<ZWhereEnumerator<TEnumerator>>(query);
            }

            /// <summary>
            /// Where query (with context invariant)
            /// Only passes on items that pass the specified predicate
            /// </summary>
            /// <param name="_predicate"></param>
            /// <returns></returns>
            public ZinqHelper<ZWhereContextEnumerator<TEnumerator, TContext>> Where<TContext>(TContext _context, Func<TContext, TSource, bool> _predicate) 
            {
                var query = new ZWhereContextEnumerator<TEnumerator, TContext>(enumerator, _context, _predicate);
                return new ZinqHelper<ZWhereContextEnumerator<TEnumerator, TContext>>(query);
            }
            
            /// <summary>
            /// Select query
            /// Applies the selector function to all items and passes along the result values
            /// </summary>
            /// <param name="_selector"></param>
            /// <typeparam name="TResult"></typeparam>
            /// <returns></returns>
            public Zinq<TResult>.ZinqHelper<ZSelectEnumerator<TEnumerator, TResult>> Select<TResult>(Func<TSource,TResult> _selector) 
            {
                var query = new ZSelectEnumerator<TEnumerator, TResult>(enumerator, _selector);
                return new Zinq<TResult>.ZinqHelper<ZSelectEnumerator<TEnumerator, TResult>>(query);
            }
            
            /// <summary>
            /// Select query (with context invariant)
            /// Applies the selector function to all items and passes along the result values
            /// </summary>
            /// <param name="_selector"></param>
            /// <typeparam name="TResult"></typeparam>
            /// <returns></returns>
            public Zinq<TResult>.ZinqHelper<ZSelectContextEnumerator<TEnumerator, TContext, TResult>> Select<TContext, TResult>(TContext _context, Func<TContext, TSource, TResult> _selector) 
            {
                var query = new ZSelectContextEnumerator<TEnumerator, TContext, TResult>(enumerator, _context, _selector);
                return new Zinq<TResult>.ZinqHelper<ZSelectContextEnumerator<TEnumerator, TContext, TResult>>(query);
            }

            /// <summary>
            /// SelectMany query
            /// Iterates multiple enumerators as if they were a single collection, applying the selector function to all items
            /// ** NOTE ** You may likely to specify the argument type of the selector! For example: (int x)=>x instead of x=>x  
            /// </summary>
            /// <param name="_enumerator"></param>
            /// <param name="_selector"></param>
            /// <typeparam name="TInnerEnumerator"></typeparam>
            /// <typeparam name="TInnerSource"></typeparam>
            /// <typeparam name="TResult"></typeparam>
            /// <returns></returns>
            public Zinq<TResult>.ZinqHelper<ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult>> 
                SelectMany<TInnerEnumerator, TInnerSource, TResult>(Func<TSource, TInnerEnumerator> _enumerator, Func<TInnerSource, TResult> _selector)  
                where TInnerEnumerator:IEnumerator<TInnerSource>
            {
                var query = new ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult>(enumerator, _enumerator, _selector);
                return new Zinq<TResult>.ZinqHelper<ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult>>(query);
            }

            /// <summary>
            /// SelectMany query
            /// Iterates multiple enumerators as if they were a single collection, applying the selector function to all items
            /// ** NOTE ** You may likely to specify the argument type of the selector! For example: (int x)=>x instead of x=>x  
            /// </summary>
            /// <param name="_context"></param>
            /// <param name="_enumerator"></param>
            /// <param name="_selector"></param>
            /// <typeparam name="TInnerEnumerator"></typeparam>
            /// <typeparam name="TInnerSource"></typeparam>
            /// <typeparam name="TResult"></typeparam>
            /// <typeparam name="TContext"></typeparam>
            /// <returns></returns>
            public Zinq<TResult>.ZinqHelper<ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult, TContext>> 
                SelectMany<TInnerEnumerator, TInnerSource, TResult, TContext>(TContext _context, Func<TContext, TSource, TInnerEnumerator> _enumerator, Func<TContext, TInnerSource, TResult> _selector)  
                where TInnerEnumerator:IEnumerator<TInnerSource>
            {
                var query = new ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult, TContext>(enumerator, _context, _enumerator, _selector);
                return new Zinq<TResult>.ZinqHelper<ZSelectManyEnumerator<TEnumerator, TInnerEnumerator, TInnerSource, TResult, TContext>>(query);
            }

            /// <summary>
            /// Concat query
            /// Appends all items from the supplied enumerator.
            /// </summary>
            /// <param name="_otherEnumerator"></param>
            /// <typeparam name="TOtherEnumerator"></typeparam>
            /// <returns></returns>
            public ZinqHelper<ZConcatEnumerator<TEnumerator, TOtherEnumerator>> Concat<TOtherEnumerator>(TOtherEnumerator _otherEnumerator) where TOtherEnumerator : struct, IEnumerator<TSource>
            {
                var query = new ZConcatEnumerator<TEnumerator, TOtherEnumerator>(enumerator, _otherEnumerator);
                return new ZinqHelper<ZConcatEnumerator<TEnumerator, TOtherEnumerator>>(query);
            }
            
            /// <summary>
            /// Ends the query and returns true if any values meet the specified predicate
            /// </summary>
            /// <param name="_predicate"></param>
            /// <returns></returns>
            public bool Any(Predicate<TSource> _predicate)
            {
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current)) return true;
                }
                return false;
            }
            
            /// <summary>
            /// Ends the query and returns true if any values meet the specified predicate
            /// </summary>
            /// <param name="_predicate"></param>
            /// <returns></returns>
            public bool Any<TContext>(TContext _context, Func<TContext, TSource, bool> _predicate)
            {
                while (enumerator.MoveNext())
                {
                    if (_predicate(_context, enumerator.Current)) return true;
                }
                return false;
            }
            
            /// <summary>
            /// Ends the query and returns true if any values meet the specified predicate
            /// </summary>
            /// <param name="_predicate"></param>
            /// <returns></returns>
            public bool All(Predicate<TSource> _predicate)
            {
                while (enumerator.MoveNext())
                {
                    if (!_predicate(enumerator.Current)) return false;
                }
                return true;
            }
            
            /// <summary>
            /// Ends the query and returns true if any values meet the specified predicate
            /// </summary>
            /// <param name="_predicate"></param>
            /// <returns></returns>
            public bool All<TContext>(TContext _context, Func<TContext,TSource, bool> _predicate)
            {
                while (enumerator.MoveNext())
                {
                    if (!_predicate(_context, enumerator.Current)) return false;
                }
                return true;
            }

            /// <summary>
            /// Ends the query and returns the final enumerator for the query
            /// </summary>
            /// <returns></returns>
            public TEnumerator GetEnumerator() => enumerator;

            /// <summary>
            /// Ends the query and returns the final enumerator in an Enumerable wrapper
            /// Note: this wrapper will throw an exception if passed by interface. As that would create GC via interface boxing
            /// </summary>
            /// <returns></returns>
            public ZEnumerable<TEnumerator> Enumerable()
            {
                return new ZEnumerable<TEnumerator>(enumerator);
            }

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
            
            /// <summary>
            /// Skips the first n items
            /// </summary>
            /// <param name="_count"></param>
            public void Skip(int _count)
            {
                while (enumerator.MoveNext() && _count>0)
                {
                    --_count;
                }
            }
            
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
            public TSource FirstOrDefault(Predicate<TSource> _predicate)
            {
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current)) return enumerator.Current;
                }
                return default;
            }
            
            /// <summary>
            /// Ends the query and returns the first item that meets the predicate (or default if no items exist)
            /// </summary>
            /// <param name="_outCollection"></param>
            /// <typeparam name="TCollection"></typeparam>
            public TSource FirstOrDefault<TContext>(TContext _context, Func<TContext, TSource, bool> _predicate)
            {
                while (enumerator.MoveNext())
                {
                    if (_predicate(_context, enumerator.Current)) return enumerator.Current;
                }
                return default;
            }

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
            
            public TSource MaxOrDefault<TContext, TResult>(TContext _context, Func<TContext, TSource, TResult> _selector)
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
            
            public TSource MinOrDefault<TResult>(Func<TSource, TResult> _selector)
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
            
            public TSource MinOrDefault<TContext, TResult>(TContext _context, Func<TContext, TSource, TResult> _selector)
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
            
            /// <summary>
            /// Ends the query and performs an action to each item
            /// </summary>
            /// <param name="_outCollection"></param>
            /// <typeparam name="TCollection"></typeparam>
            public void Foreach(Action<TSource> _action)
            {
                while (enumerator.MoveNext())
                {
                    _action(enumerator.Current);
                }
            }
            
            /// <summary>
            /// Ends the query and performs an action to each item
            /// </summary>
            /// <param name="_outCollection"></param>
            /// <typeparam name="TCollection"></typeparam>
            public void Foreach<TContext>(TContext _context, Action<TContext, TSource> _action)
            {
                while (enumerator.MoveNext())
                {
                    _action(_context, enumerator.Current);
                }
            }
        }

        /// <summary>
        /// Begin a new query on an array
        /// </summary>
        /// <param name="_array"></param>
        /// <returns></returns>
        public static ZinqHelper<ZArrayEnumerator> BeginA(TSource[] _array)
        {
            return new ZinqHelper<ZArrayEnumerator>(new ZArrayEnumerator(_array));
        }

        /// <summary>
        /// Begin a new query on a list
        /// </summary>
        /// <param name="_array"></param>
        /// <returns></returns>
        public static ZinqHelper<ZListEnumerator<TList>> BeginL<TList>(TList _list)
            where TList:IList<TSource>
        {
            return new ZinqHelper<ZListEnumerator<TList>>(new ZListEnumerator<TList>(_list));
        }

        /// <summary>
        /// Begin a new query on an enumerator
        /// </summary>
        /// <param name="_enumerator"></param>
        /// <typeparam name="TEnumerator"></typeparam>
        /// <returns></returns>
        public static ZinqHelper<TEnumerator> BeginE<TEnumerator>(TEnumerator _enumerator) 
            where TEnumerator: struct,IEnumerator<TSource>
        {
            return new ZinqHelper<TEnumerator>(_enumerator);
        }
    }

    public static class ZinqExtensions
    {
        public static Zinq<TSource>.ZinqHelper<Zinq<TSource>.ZListEnumerator<List<TSource>>> Zinq<TSource>(this List<TSource> @this) => global::Zinq.Zinq<TSource>.BeginL(@this);
        public static Zinq<TSource>.ZinqHelper<Zinq<TSource>.ZArrayEnumerator> Zinq<TSource>(this TSource[] @this) => global::Zinq.Zinq<TSource>.BeginA(@this);
    }
}