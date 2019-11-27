using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Burst;

namespace Tests
{
    public class JobsTests
    {   
        public static class JobsOuterScope<TSource> where TSource : struct
        {
            public struct ParallelJobEnumerator<TEnumerator, TResult> 
                : IEnumerator<TResult> 
                where TEnumerator : IEnumerator<TSource>
                where TResult : struct
            {
                public delegate TResult SelectorFunc(TSource _source); 
                
                [BurstCompile]
                public struct Job : IJobParallelFor, IDisposable
                {
                    [ReadOnly]
                    public FunctionPointer<SelectorFunc> m_JobSelector;
                
                    [ReadOnly] 
                    public NativeArray<TSource> m_JobSource;
                    
                    [WriteOnly] 
                    public NativeArray<TResult> m_JobResults;
                    
                    void IJobParallelFor.Execute(int _index)
                    {
                        m_JobResults[_index] = m_JobSelector.Invoke(m_JobSource[_index]);
                    }

                    public void Dispose()
                    {
                        m_JobSource.Dispose();
                        m_JobResults.Dispose();
                    }
                }
                
                TEnumerator m_SourceEnumerator;
                JobHandle m_JobHandle;
                Job m_Job;                
                int m_MoveNextIndex;
                int m_MaxCapacity;
                int m_ItemsPerJob;

                public ParallelJobEnumerator(TEnumerator _enumerator, SelectorFunc _jobSelector, int _maxCapacity, int _itemsPerJob=1)
                {
                    m_MaxCapacity = _maxCapacity;
                    m_Job = new Job
                    {
                        m_JobSelector = new FunctionPointer<SelectorFunc>(Marshal.GetFunctionPointerForDelegate(_jobSelector))
                    };
                    
                    m_MoveNextIndex = -1;
                    m_ItemsPerJob = _itemsPerJob;
                    m_SourceEnumerator = _enumerator;
                    m_JobHandle = default;
                }

                public bool MoveNext()
                {
                    if (m_MoveNextIndex >= m_Job.m_JobResults.Length) return false;
                    
                    // end the job if running
                    if (m_MoveNextIndex == -1)
                    {
                        // run the job
                        m_Job.m_JobSource = new NativeArray<TSource>(m_MaxCapacity, Allocator.TempJob);
                        int itemCount = 0;
                        while (m_SourceEnumerator.MoveNext() && itemCount < m_MaxCapacity)
                        {
                            m_Job.m_JobSource[itemCount] = m_SourceEnumerator.Current;
                            ++itemCount;
                        }
                        
                        m_Job.m_JobResults = new NativeArray<TResult>(itemCount, Allocator.TempJob);
                        m_JobHandle = m_Job.Schedule(m_Job.m_JobResults.Length, m_ItemsPerJob);
                        m_JobHandle.Complete();
                    }
                    
                    ++m_MoveNextIndex;
                    return m_MoveNextIndex<m_Job.m_JobResults.Length;
                }

                void IEnumerator.Reset()
                {
                    throw new NotImplementedException();
                }

                public TResult Current => m_Job.m_JobResults[m_MoveNextIndex];
                object IEnumerator.Current => m_Job.m_JobResults[m_MoveNextIndex];

                void IDisposable.Dispose()
                {
                    m_Job.Dispose();
                }
            }
        }

        // Job adding two floating point values together
        public struct MyParallelJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<float> a;
            [ReadOnly]
            public NativeArray<float> b;
            public NativeArray<float> result;

            public void Execute(int i)
            {
                result[i] = a[i] + b[i];
            }
        }

        [Test]
        public void JobEnumeratorTest()
        {
            var data = Enumerable.Range(1, 100).ToList();
            using (var enumerator =
                new JobsOuterScope<int>.ParallelJobEnumerator<List<int>.Enumerator, int>(data.GetEnumerator(), _x => _x + 1, data.Count, 10))
            {
                while (enumerator.MoveNext())
                {
                    Debug.Log(enumerator.Current.ToString());
                }
            }
        }

        [UnityTest]
        public IEnumerator Jobs()
        {
            NativeArray<float> a = new NativeArray<float>(2, Allocator.TempJob);

            NativeArray<float> b = new NativeArray<float>(2, Allocator.TempJob);

            NativeArray<float> result = new NativeArray<float>(2, Allocator.TempJob);

            a[0] = 1.1f;
            b[0] = 2.2f;
            a[1] = 3.3f;
            b[1] = 4.4f;

            MyParallelJob jobData = new MyParallelJob();
            jobData.a = a;  
            jobData.b = b;
            jobData.result = result;

            // Schedule the job with one Execute per index in the results array and only 1 item per processing batch
            JobHandle handle = jobData.Schedule(result.Length, 1);

            // Wait for the job to complete
            yield return new WaitUntil(()=>handle.IsCompleted);
            handle.Complete();

            // Free the memory allocated by the arrays
            a.Dispose();
            b.Dispose();
            result.Dispose();
        }
    }
}