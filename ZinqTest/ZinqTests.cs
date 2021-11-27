using System.Collections.Generic;
using System.Linq;
using Zinq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace Tests
{
    public class ZinqTests
    {
        [Test]
        public void SelectMany()
        {
            var outputs = new List<int>(100);
            var outputs2 = new List<int>(100);
            var list = new List<List<int>>(10);
            for (int i = 0; i < 3; i++)
            {
                var data = new List<int>(10);
                list.Add(data);
                for (int j = 0; j < 3; j++)
                {
                    data.Add(i+3 + j);
                }
                list.Add(data);
            }
            
            Assert.That(() =>
            {
                list.Zinq().SelectMany((in List<int> _x) => _x.GetEnumerator(), (in int _y) => _y).To(outputs);
                list.Zinq().SelectMany(2, (in int _, in List<int> _x) => _x.GetEnumerator(), (in int _c, in int _y) => _y * _c).To(outputs2);
            }, Is.Not.AllocatingGCMemory());
            
            Assert.IsTrue(list.SelectMany(x=>x).SequenceEqual(outputs));
            Assert.IsTrue(list.SelectMany(x=>x).Select(x=>x*2).SequenceEqual(outputs2));
        }
      
        [Test]
        public void SelectE()
        {            
            var outputs = new List<int>(10);
            var list = new List<int>(10);
            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            Assert.That(() =>
            {
                Zinq<int>.FromEnumerator(list.GetEnumerator())
                    .Select((in int _x) => _x * 5)
                    .To(outputs);
            }, Is.Not.AllocatingGCMemory());

            Debug.Log(string.Join(", ", outputs));
            Assert.IsTrue(list.Select(x=>5*x).SequenceEqual(outputs));
        }

        [Test]
        public void SelectL()
        {            
            var outputs = new List<int>(10);
            var outputs2 = new List<int>(10);
            var list = new List<int>(10);
            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            Assert.That(() =>
            {
                Zinq<int>.FromList(list)
                    .Select((in int _x) => _x * 5)
                    .To(outputs);
                
                list.Zinq().Select((in int _x) => _x * 5).To(outputs2);
            }, Is.Not.AllocatingGCMemory());

            Debug.Log(string.Join(", ", outputs));
            Assert.IsTrue(list.Select(x=>5*x).SequenceEqual(outputs));
            Assert.IsTrue(list.Select(x=>5*x).SequenceEqual(outputs2));
        }
        
        [Test]
        public void Concat()
        {            
            var outputs = new List<int>(20);
            
            var list = new List<int>(10);
            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }
            
            var listb = new List<int>(10);
            for (int i = 11; i < 20; i++)
            {
                listb.Add(i);
            }

            Assert.That(() =>
            {
                list.Zinq().Concat(listb.GetEnumerator()).To(outputs);
            }, Is.Not.AllocatingGCMemory());

            Debug.Log(string.Join(", ", outputs));
            Assert.IsTrue(list.Concat(listb).SequenceEqual(outputs));
        }

        [Test]
        public void SelectA()
        {            
            var outputs = new List<int>(10);
            var outputs2 = new List<int>(10);

            var array = new int[10];
            for (int i = 0; i < 10; i++)
            {
                array[i] = i;
            }

            Assert.That(() =>
            {
                Zinq<int>.FromArray(array)
                    .Select((in int _x) => _x * 5)
                    .To(outputs);
                
                array.Zinq()
                    .Select((in int _x) => _x * 5)
                    .To(outputs2);
            }, Is.Not.AllocatingGCMemory());

            Debug.Log(string.Join(", ", outputs));
            Assert.IsTrue(array.Select(x=>5*x).SequenceEqual(outputs));
            Assert.IsTrue(array.Select(x=>5*x).SequenceEqual(outputs2));
        }

        [Test]
        public void AsEnumerable()
        {            
            var outputs = new List<int>(10);
            var list = new List<int>(10);
            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            Assert.That(() =>
            {
                var query = Zinq<int>.FromEnumerator(list.GetEnumerator())
                    .Select((in int _x) => _x * 5)
                    .AsEnumerable();
                foreach (var result in query)
                {
                    outputs.Add(result);
                }
            }, Is.Not.AllocatingGCMemory());

            Debug.Log(string.Join(", ", outputs));
            Assert.IsTrue(list.Select(x=>5*x).SequenceEqual(outputs));
        }

        
        [Test]
        public void FirstOrDefault()
        {
            int output1 = -1, output2 = -1, output3 = -1, output4 = -1;
            var list = new List<int>(10);
            var array = new int[10];

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
                array[i] = i;
            }
            
            Assert.That(() =>
            {
                output1 = Zinq<int>.FromEnumerator(list.GetEnumerator())
                    .Select((add:10, mul:2), (in (int add, int mul) _c, in int _v) => _c.add + _v*_c.mul)
                    .FirstOrDefault();
                
                output2 = list.Zinq()
                    .Select((add:10, mul:2), (in (int add, int mul) _c, in int _v) => _c.add + _v*_c.mul)
                    .FirstOrDefault((in int _x)=>_x%3 == 0);

                output3 = Zinq<int>.FromArray(array)
                    .Select((add:10, mul:2), (in (int add, int mul) _c, in int _v) => _c.add + _v*_c.mul)
                    .FirstOrDefault();
                
                output4 = array.Zinq()
                    .Select((add:10, mul:2), (in (int add, int mul) _c, in int _v) => _c.add + _v*_c.mul)
                    .FirstOrDefault((in int _x)=>_x%3 == 0);
            }, Is.Not.AllocatingGCMemory());
         
            Debug.Log($"{output1}, {output2}, {output3}, {output4}");

            Assert.AreEqual( output1, list.Select(x=>10+x*2).FirstOrDefault());
            Assert.AreEqual(output1, output3);
            
            Assert.AreEqual( output2, array.Select(x=>10+x*2).FirstOrDefault(_x=>_x%3 == 0));
            Assert.AreEqual(output2, output4);
        }
    }
}
