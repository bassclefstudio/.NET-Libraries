using BassClefStudio.NET.Core.Streams;
using BassClefStudio.NET.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Tests
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
                .BindResult(v => value = v);
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
                .Where(s => s.Last() == '!')
                .BindResult(results.Add);
            stream.Start();
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
                .Aggregate<string, int>((n, s) => n + 1)
                .BindResult(n => number = n);
            stream.Start();
            Assert.AreEqual(length, number, "Aggregate was not expected value.");
        }

        [TestMethod]
        public void TestSum()
        {
            int length = 8;
            int number = 0;
            var stream = SourceStream<int>.Repeat(2, length)
                .Sum()
                .BindResult(n => number = n);
            stream.Start();
            Assert.AreEqual(length * 2, number, "Sum was not expected value.");
        }

        [TestMethod]
        public void TestCount()
        {
            int length = 8;
            int number = 0;
            var stream = SourceStream<string>.Repeat("Hello World!", length)
                .Count()
                .BindResult(n => number = n);
            stream.Start();
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
                .Join(streamA, (i, s) => i + s)
                .BindResult(n => numbers.Add(n));
            join.Start();
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
            var stream = source
                .Unique()
                .BindResult(n => number++);
            stream.Start();
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
                .BindResult(n => number = n);
            source = SourceStream<int>.Repeat(1, length);
            stream.Start();
            Assert.AreEqual(number, length, "Lazy stream evaluation returned the incorrect count.");
        }

        #endregion
        #region Sources

        [TestMethod]
        public void EmptySource()
        {
            SourceStream<string> source = new SourceStream<string>();
            string value = null;
            source.BindResult(s => value = s);
            source.Start();
            Assert.AreEqual(null, value, "SourceStream unintentionally emitted a value.");
        }

        [TestMethod]
        public void ListSource()
        {
            SourceStream<string> source = new SourceStream<string>("hello", "world!");
            string value = null;
            source.BindResult(s => value = s);
            source.Start();
            Assert.IsNotNull(value, "SourceStream failed to emit a value.");
            Assert.AreEqual("world!", value, "SourceStream's last emitted value was unexpected.");
        }

        #endregion
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
