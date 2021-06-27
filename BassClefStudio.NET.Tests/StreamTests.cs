using BassClefStudio.NET.Core.Streams;
using BassClefStudio.NET.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Tests
{
    [TestClass]
    public class StreamTests
    {
        #region Binding

        private static MyClass CreateTestObject()
        {
            return new MyClass()
            {
                Property = new MyPropertyClass()
                {
                    Name = "Test 1",
                    Keys = new ObservableCollection<string>() { "Test 1" }
                }
            };
        }

        private static void TestBinding(Func<MyClass, IStream<string>> getBinding)
        {
            var myObject = CreateTestObject();
            string value = null;
            IStream<string> nameBinding = getBinding(myObject)
                .BindResult(v => value = v)
                .BindError(e => throw e);
            nameBinding.Start();

            myObject.Property.Name = "Test 2";
            Assert.IsNotNull(value, "ValueChanged event was not fired.");
            Assert.AreEqual(myObject.Property.Name, value, "Incorrect StoredValue on nameBinding.");

            value = null;
            myObject.Property = new MyPropertyClass()
            {
                Name = "Test 3"
            };
            Assert.IsNotNull(value, "ValueChanged event was not fired.");
            Assert.AreEqual(myObject.Property.Name, value, "Incorrect StoredValue on nameBinding.");
        }

        [TestMethod]
        public void TestPropertyBinding()
        {
            TestBinding(myObject => myObject.AsStream()
                .Property(m => m.Property)
                .Property(p => p.Name));
            //// Strongly-typed - think {x:Bind Property.Name}
        }

        [TestMethod]
        public void TestReflectionBinding()
        {
            TestBinding(myObject => myObject.AsStream()
                .Property<MyClass, string>("Property.Name"));
            //// Reflection - weakly-typed - think {Binding Property.Name}
        }

        [TestMethod]
        public void TestBadReflectionPath()
        {
            var myObject = CreateTestObject();
            Assert.ThrowsException<StreamException>(() =>
                myObject.AsStream()
                .Property<MyClass, string>("Property.Blah"));
            //// Reflection - weakly-typed - think {Binding Property.Blah} (where the property name doesn't exist).
        }

        [TestMethod]
        public void TestNullSets()
        {
            var a = new MyPropertyClass()
            {
                Name = "Fred",
                Keys = null
            };

            var b = new MyPropertyClass()
            {
                Name = "George",
                Keys = null
            };

            var myObject = new MyClass()
            {
                Property = a
            };

            int register = 0;
            IStream<ObservableCollection<string>> keysBinding = myObject
                .AsStream().Property(m => m.Property).Property(p => p.Keys)
                .BindResult(v => register++);
            keysBinding.Start();

            myObject.Property = b;
            Assert.AreEqual(0, register, "ValueChanged event was accidentally fired.");
        }

        #endregion
        #region Linq

        [TestMethod]
        public void TestFilter()
        {
            string[] values = new string[] { "wow!", "hello", "cool!", "great", "awesome!" };
            List<string> results = new List<string>();
            var stream = new SourceStream<string>(values)
                .Where(s => s.Last() == '!');
            SetupException(stream);
            stream.BindResult(results.Add);
            Assert.AreEqual(3, results.Count, "Result does not contain expected number of items.");
            Assert.IsTrue(results.SequenceEqual(new string[] { values[0], values[2], values[4] }));
        }

        [TestMethod]
        public void TestAggregateCounter()
        {
            int length = 8;
            int number = 0;
            var source = SourceStream<string>.Repeat("Hello World!", length)
                .Join(new SourceStream<string>(new StreamValue<string>()));
            var stream = source
                .Aggregate<string, int>((n, s) => n + 1);
            SetupException(stream);
            stream.BindResult(n => number = n);
            Assert.AreEqual(length, number, "Aggregate was not expected value.");
        }

        [TestMethod]
        public void TestSum()
        {
            int length = 8;
            int number = 0;
            var stream = SourceStream<int>.Repeat(2, length)
                .Sum();
            SetupException(stream);
            stream.BindResult(n => number = n);
            Assert.AreEqual(length * 2, number, "Sum was not expected value.");
        }

        [TestMethod]
        public void TestCount()
        {
            int length = 8;
            int number = 0;
            var stream = SourceStream<string>.Repeat("Hello World!", length)
                .Count();
            SetupException(stream);
            stream.BindResult(n => number = n);
            Assert.AreEqual(length, number, "Count was not expected value.");
        }

        [TestMethod]
        public void TestJoin()
        {
            int length = 8;
            List<int> numbers = new List<int>();
            var streamA = SourceStream<int>.CountStream(1, length);
            var streamB = new SourceStream<int>(2);
            IStream<int> join = streamB
                .Join(streamA, (i, s) => i + s);
            SetupException(join);
            join.BindResult(n => numbers.Add(n));
            Assert.AreEqual(numbers.Count, length + 1, "Returned values were of an unexpected length");
            Assert.IsTrue(numbers.SequenceEqual(Enumerable.Range(2, length + 1)), "Sequence of returned values was unexpected.");
        }

        [TestMethod]
        public void TestUnique()
        {
            int length = 8;
            int number = 0;
            var source = SourceStream<string>.Repeat("Hello World!", length)
                .Join(new SourceStream<string>(new StreamValue<string>()));
            var stream = source.Unique();
            SetupException(stream);
            stream.BindResult(n => number++);
            Assert.AreEqual(1, number, "Number of unique items was invalid.");
        }

        [TestMethod]
        public void TestRec()
        {
            int length = 4;
            int number = 0;
            SourceStream<int> source = null;
            Func<SourceStream<int>> recSource = () => source;
            IStream<int> stream = recSource.Rec()
                .Count()
                .BindResult(n => number = n, false);
            source = SourceStream<int>.Repeat(1, length);
            SetupException(stream);
            stream.Start();
            Assert.AreEqual(number, length, "Lazy stream evaluation returned the incorrect count.");
        }

        [TestMethod]
        public void TestCast()
        {
            int length = 4;
            int count = 0;
            SourceStream<object> source = SourceStream<object>.Repeat(new MyClass(), length);
            IStream<MyClass> stream = source.Cast<object, MyClass>();
            SetupException(stream);
            stream.BindResult(c => count++);
            Assert.AreEqual(count, length, "Stream casting did not correctly cast all values.");
        }

        [TestMethod]
        public void TestAs()
        {
            int length = 4;
            int count = 0;
            SourceStream<MyClass> source = SourceStream<MyClass>.Repeat(new MyClass(), length);
            IStream<Observable> stream = source.As<MyClass, Observable>();
            SetupException(stream);
            stream.BindResult(c => count++);
            Assert.AreEqual(count, length, "Stream casting did not correctly cast all values.");
        }

        [TestMethod]
        public void TestCastFilter()
        {
            int length = 4;
            int count = 0;
            SourceStream<object> source = SourceStream<object>.Repeat(4, length);
            IStream<MyClass> stream = source.OfType<object, MyClass>();
            SetupException(stream);
            stream.BindResult(c => count++);
            for (int i = 0; i < length; i++)
            {
                source.EmitValue(new MyClass());
            }
            Assert.AreEqual(length, count, "Stream should have filtered all but the last MyClass item by type.");
        }

        [TestMethod]
        public void TestTakePair()
        {
            int length = 4;
            int sum = 0;
            SourceStream<int> source = SourceStream<int>.CountStream(1, length);
            IStream<int> stream = source.Take((v1, v2) => v1 + v2);
            SetupException(stream);
            stream.BindResult(c => sum += c);
            Assert.AreEqual((1 + 2) + (2 + 3) + (3 + 4), sum, "Take stream should have expected sum of all consecutive pairs of [1,2,3,4].");
        }

        [TestMethod]
        public void TestTakeMany()
        {
            int length = 4;
            int sum = 0;
            SourceStream<int> source = SourceStream<int>.CountStream(1, length);
            IStream<int> stream = source.Take((vs) => vs[0] + vs[1] + vs[2], 3);
            SetupException(stream);
            stream.BindResult(c => sum += c);
            Assert.AreEqual((1 + 2 + 3) + (2 + 3 + 4), sum, "Take stream should have expected sum of all consecutive triples of [1,2,3,4].");
        }

        #endregion
        #region Sources

        [TestMethod]
        public void EmptySource()
        {
            SourceStream<string> source = new SourceStream<string>();
            string value = null;
            source.BindResult(s => value = s);
            SetupException(source);
            source.Start();
            Assert.AreEqual(null, value, "SourceStream unintentionally emitted a value.");
        }

        [TestMethod]
        public void ListSource()
        {
            SourceStream<string> source = new SourceStream<string>("hello", "world!");
            string value = null;
            source.BindResult(s => value = s);
            SetupException(source);
            source.Start();
            Assert.IsNotNull(value, "SourceStream failed to emit a value.");
            Assert.AreEqual("world!", value, "SourceStream's last emitted value was unexpected.");
        }

        #endregion

        private void SetupException<T>(IStream<T> stream)
        {
            stream.BindError(e => throw e);
        }
    }

    class MyClass : Observable
    {
        private MyPropertyClass property;
        public MyPropertyClass Property { get => property; set => Set(ref property, value); }
    }

    class MyPropertyClass : Observable
    {
        private string name;
        public string Name { get => name; set => Set(ref name, value); }

        public ObservableCollection<string> Keys { get; set; }
    }
}
