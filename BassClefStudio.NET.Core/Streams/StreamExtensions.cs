using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// Provides a series of extension methods for dealing with <see cref="IStream{T}"/>s of all types.
    /// </summary>
    public static class StreamExtensions
    {
        #region Cast

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that returns the values of the given <see cref="IStream{T}"/> as type <typeparamref name="T2"/>.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the cast values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns cast <typeparamref name="T2"/> values.</returns>
        public static IStream<T2> As<T1, T2>(this IStream<T1> stream) where T1 : T2
        {
            return new MapStream<T1, T2>(stream, t1 => (T2)t1);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that attempts to cast the values of the given <see cref="IStream{T}"/> as type <typeparamref name="T2"/>.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the cast values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns cast <typeparamref name="T2"/> values.</returns>
        public static IStream<T2> Cast<T1, T2>(this IStream<T1> stream)
        {
            return new MapStream<T1, T2>(
                stream,
                t1 => t1 is T2 t2
                    ? t2
                    : throw new StreamException($"Invalid casting in MapStream: {t1?.GetType()?.Name} to {typeof(T2).Name}."));
        }

        #endregion
        #region Select

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that maps the values of the given <see cref="IStream{T}"/> as <typeparamref name="T2"/> values.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="mapFunc">The function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting <typeparamref name="T2"/> values.</returns>
        public static IStream<T2> Select<T1, T2>(this IStream<T1> stream, Func<T1, T2> mapFunc)
        {
            return new MapStream<T1, T2>(stream, mapFunc);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that maps the values of the given <see cref="IStream{T}"/> asynchronously as <typeparamref name="T2"/> values.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="mapFunc">The asynchronous function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting <typeparamref name="T2"/> values as they're converted.</returns>        
        public static IStream<T2> Select<T1, T2>(this IStream<T1> stream, Func<T1, Task<T2>> mapFunc)
        {
            return new ParallelMapStream<T1, T2>(stream, mapFunc);
        }

        #endregion
        #region Where

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that filters the values returned by the given <see cref="IStream{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T"/> values.</param>
        /// <param name="filter">A function that returns a <see cref="bool"/> for each <typeparamref name="T"/> input indicating whether it should propogate onto this stream.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns only the filtered <typeparamref name="T"/> values.</returns>
        public static IStream<T> Where<T>(this IStream<T> stream, Func<T, bool> filter)
        {
            return new FilterStream<T>(stream, filter);
        }

        #endregion
        #region Aggregate

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that aggregates the values of the given <see cref="IStream{T}"/> into a <typeparamref name="T2"/> returned state.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="aggregateFunc">The function that returns a new <typeparamref name="T2"/> aggregate from the current <typeparamref name="T2"/> state and the next <typeparamref name="T1"/> input.</param>
        /// <param name="initialState">The initial <typeparamref name="T2"/> state.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting <typeparamref name="T2"/> values.</returns>
        public static IStream<T2> Aggregate<T1, T2>(this IStream<T1> stream, Func<T2, T1, T2> aggregateFunc, T2 initialState = default(T2))
        {
            return new AggregateStream<T1, T2>(stream, aggregateFunc, initialState);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that aggregates the values of the given <see cref="IStream{T}"/> asynchronously into a <typeparamref name="T2"/> returned state.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="aggregateFunc">The asynchronous function that returns a new <typeparamref name="T2"/> aggregate from the current <typeparamref name="T2"/> state and the next <typeparamref name="T1"/> input.</param>
        /// <param name="initialState">The initial <typeparamref name="T2"/> state.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting <typeparamref name="T2"/> values as they're produced.</returns>
        public static IStream<T2> Aggregate<T1, T2>(this IStream<T1> stream, Func<T2, T1, Task<T2>> aggregateFunc, T2 initialState = default(T2))
        {
            return new ParallelAggregateStream<T1, T2>(stream, aggregateFunc, initialState);
        }

        #endregion
        #region Join

        /// <summary>
        /// Joins an <see cref="IStream{T}"/> to an existing <see cref="IStream{T}"/> to create a concatenated <see cref="IStream{T}"/> with all of the output values of both existing streams.
        /// </summary>
        /// <typeparam name="T">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The first <see cref="IStream{T}"/>.</param>
        /// <param name="other">The <see cref="IStream{T}"/> to concatenate with <paramref name="stream"/>.</param>
        /// <returns>An <see cref="IStream{T}"/> that emits all values recieved from both parent <see cref="IStream{T}"/>s.</returns>
        public static IStream<T> Join<T>(this IStream<T> stream, IStream<T> other)
        {
            return new ConcatStream<T>(stream, other);
        }

        /// <summary>
        /// Joins a collection of <see cref="IStream{T}"/>s to create a concatenated <see cref="IStream{T}"/> with all of the output values of both existing streams.
        /// </summary>
        /// <typeparam name="T">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="streams">The parent <see cref="IStream{T}"/>s producing <typeparamref name="T"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that emits all values recieved from all parent <see cref="IStream{T}"/>s.</returns>
        public static IStream<T> Join<T>(this IEnumerable<IStream<T>> streams)
        {
            return new ConcatStream<T>(streams);
        }

        /// <summary>
        /// Merges a collection of <see cref="IStream{T}"/>s to produce output <typeparamref name="T2"/> values from their combined <typeparamref name="T1"/> emitted values.
        /// </summary>
        /// <param name="streams">The parent <see cref="IStream{T}"/>s producing <typeparamref name="T1"/> values.</param>
        /// <param name="transformFunc">A <see cref="Func{T, TResult}"/> that produces a <typeparamref name="T2"/> result from the most recent <typeparamref name="T1"/> values produced by each of the parent <see cref="IStream{T}"/>s.</param>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>s.</typeparam>
        /// <typeparam name="T2">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <returns>An <see cref="IStream{T}"/> that emits <typeparamref name="T2"/> transformed values every time a parent <see cref="IStream{T}"/> emits a new <typeparamref name="T1"/> input.</returns>
        public static IStream<T2> Join<T1, T2>(this IEnumerable<IStream<T1>> streams, Func<T1[], T2> transformFunc)
        {
            return new MergeStream<T1, T2>(transformFunc, streams);
        }

        /// <summary>
        /// Merges this <see cref="IStream{T}"/> with another <see cref="IStream{T}"/> to produce output <typeparamref name="T2"/> values from their combined <typeparamref name="T1"/> emitted values.
        /// </summary>
        /// <param name="stream">The first <see cref="IStream{T}"/>.</param>
        /// <param name="other">The second/other <see cref="IStream{T}"/> which will merge with this <paramref name="stream"/>.</param>
        /// <param name="transformFunc">A <see cref="Func{T, TResult}"/> that produces a <typeparamref name="T2"/> result from the most recent <typeparamref name="T1"/> values produced by the two parent <see cref="IStream{T}"/>s.</param>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>s.</typeparam>
        /// <typeparam name="T2">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <returns>An <see cref="IStream{T}"/> that emits <typeparamref name="T2"/> transformed values every time a parent <see cref="IStream{T}"/> emits a new <typeparamref name="T1"/> input.</returns>
        public static IStream<T2> Join<T1, T2>(this IStream<T1> stream, IStream<T1> other, Func<T1, T1, T2> transformFunc)
        {
            return new MergeStream<T1, T2>(ts => transformFunc(ts[0], ts[1]), stream, other);
        }

        #endregion
        #region Distinct

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that only returns all consecutive unique output values.
        /// </summary>
        /// <typeparam name="T">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/>.</param>
        /// <returns>An <see cref="IStream{T}"/> returning (locally) unique <typeparamref name="T"/> outputs.</returns>
        public static IStream<T> Unique<T>(this IStream<T> stream)
        {
            return new DistinctStream<T>(stream, (t1, t2) => !Equals(t1, t2));
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that only returns all consecutive unique output values, as determined by the <see cref="IEquatable{T}.Equals(T)"/> method.
        /// </summary>
        /// <typeparam name="T">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/>.</param>
        /// <returns>An <see cref="IStream{T}"/> returning (locally) unique <typeparamref name="T"/> outputs.</returns>
        public static IStream<T> UniqueEq<T>(this IStream<T> stream) where T : IEquatable<T>
        {
           return new DistinctStream<T>(
                stream, 
                (t1, t2) => (!(t1 == null && t2 == null)
                    && ((t1 == null && t2 != null)
                    || !t1.Equals(t2))));
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that filters consecutive output values through a given function.
        /// </summary>
        /// <typeparam name="T">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/>.</param>
        /// <param name="includeFunc">A <see cref="Func{T1, T2, TResult}"/> that takes in the incoming and previous <typeparamref name="T"/> inputs and returns a <see cref="bool"/> indicating whether the incoming value should be emitted.</param>
        /// <returns>An <see cref="IStream{T}"/> returning (locally) included <typeparamref name="T"/> outputs as per <paramref name="includeFunc"/>.</returns>
        public static IStream<T> Distinct<T>(this IStream<T> stream, Func<T, T, bool> includeFunc)
        {
            return new DistinctStream<T>(stream, includeFunc);
        }

        #endregion
        #region Sum

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that returns the sum of all the values returned by the given <see cref="IStream{T}"/> up to that point.
        /// </summary>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <see cref="int"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting sums.</returns>
        public static IStream<int> Sum(this IStream<int> stream)
        {
            return new AggregateStream<int, int>(stream, (sum, val) => sum + val, 0);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that returns the sum of all the values returned by the given <see cref="IStream{T}"/> up to that point.
        /// </summary>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <see cref="double"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting sums.</returns>
        public static IStream<double> Sum(this IStream<double> stream)
        {
            return new AggregateStream<double, double>(stream, (sum, val) => sum + val, 0);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that returns the sum of all the values returned by the given <see cref="IStream{T}"/> up to that point.
        /// </summary>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <see cref="int"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting sums.</returns>
        public static IStream<float> Sum(this IStream<float> stream)
        {
            return new AggregateStream<float, float>(stream, (sum, val) => sum + val, 0);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that returns the count of values returned by the <see cref="IStream{T}"/> parent.
        /// </summary>
        /// <param name="stream">The parent <see cref="IStream{T}"/>.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns the resulting count.</returns>
        public static IStream<int> Count<T>(this IStream<T> stream)
        {
            return new AggregateStream<T, int>(stream, (i, t) => i + 1, 0);
        }

        #endregion
        #region Bind

        /// <summary>
        /// Binds the incoming <typeparamref name="T"/> results from an <see cref="IStream{T}"/> to a given <see cref="Action{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of values emitted by this <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="TStream">The type of <see cref="IStream{T}"/> being bound to.</typeparam>
        /// <param name="stream">The <see cref="IStream{T}"/> stream to bind to.</param>
        /// <param name="action">An action that takes in an input <typeparamref name="T"/> value and will be executed every time the <paramref name="stream"/> emits a value of <see cref="StreamValueType.Result"/>.</param>
        /// <returns>The input <see cref="IStream{T}"/> <paramref name="stream"/>.</returns>
        public static TStream BindResult<T, TStream>(this TStream stream, Action<T> action) where TStream : IStream<T>
        {
            stream.ValueEmitted += (s, e) =>
            {
                if(e.DataType == StreamValueType.Result)
                {
                    action(e.Result);
                }
            };
            return stream;
        }

        /// <summary>
        /// Binds any incoming <see cref="Exception"/>s from an <see cref="IStream{T}"/> to a given <see cref="Action{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of values emitted by this <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="TStream">The type of <see cref="IStream{T}"/> being bound to.</typeparam>
        /// <param name="stream">The <see cref="IStream{T}"/> stream to bind to.</param>
        /// <param name="action">An action that takes in an input <see cref="Exception"/> and will be executed every time the <paramref name="stream"/> emits a value of <see cref="StreamValueType.Error"/>.</param>
        /// <returns>The input <see cref="IStream{T}"/> <paramref name="stream"/>.</returns>
        public static TStream BindError<T, TStream>(this TStream stream, Action<Exception> action) where TStream : IStream<T>
        {
            stream.ValueEmitted += (s, e) =>
            {
                if (e.DataType == StreamValueType.Error)
                {
                    action(e.Error);
                }
            };
            return stream;
        }

        /// <summary>
        /// Binds the completion of an <see cref="IStream{T}"/> to a given <see cref="Action"/>.
        /// </summary>
        /// <typeparam name="T">The type of values emitted by this <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="TStream">The type of <see cref="IStream{T}"/> being bound to.</typeparam>
        /// <param name="stream">The <see cref="IStream{T}"/> stream to bind to.</param>
        /// <param name="action">An action that will be executed every time the <paramref name="stream"/> emits a value of <see cref="StreamValueType.Completed"/>.</param>
        /// <returns>The input <see cref="IStream{T}"/> <paramref name="stream"/>.</returns>
        public static TStream BindComplete<T, TStream>(this TStream stream, Action action) where TStream : IStream<T>
        {
            stream.ValueEmitted += (s, e) =>
            {
                if (e.DataType == StreamValueType.Completed)
                {
                    action();
                }
            };
            return stream;
        }

        /// <summary>
        /// Binds the incoming <typeparamref name="T"/> results from an <see cref="IStream{T}"/> to a given <see cref="Action{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of values emitted by this <see cref="IStream{T}"/>.</typeparam>
        /// <param name="stream">The <see cref="IStream{T}"/> stream to bind to.</param>
        /// <param name="action">An action that takes in an input <typeparamref name="T"/> value and will be executed every time the <paramref name="stream"/> emits a value of <see cref="StreamValueType.Result"/>.</param>
        /// <returns>The input <see cref="IStream{T}"/> <paramref name="stream"/>.</returns>
        public static IStream<T> BindResult<T>(this IStream<T> stream, Action<T> action)
        {
            stream.ValueEmitted += (s, e) =>
            {
                if (e.DataType == StreamValueType.Result)
                {
                    action(e.Result);
                }
            };
            return stream;
        }

        /// <summary>
        /// Binds any incoming <see cref="Exception"/>s from an <see cref="IStream{T}"/> to a given <see cref="Action{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of values emitted by this <see cref="IStream{T}"/>.</typeparam>
        /// <param name="stream">The <see cref="IStream{T}"/> stream to bind to.</param>
        /// <param name="action">An action that takes in an input <see cref="Exception"/> and will be executed every time the <paramref name="stream"/> emits a value of <see cref="StreamValueType.Error"/>.</param>
        /// <returns>The input <see cref="IStream{T}"/> <paramref name="stream"/>.</returns>
        public static IStream<T> BindError<T>(this IStream<T> stream, Action<Exception> action)
        {
            stream.ValueEmitted += (s, e) =>
            {
                if (e.DataType == StreamValueType.Error)
                {
                    action(e.Error);
                }
            };
            return stream;
        }

        /// <summary>
        /// Binds the completion of an <see cref="IStream{T}"/> to a given <see cref="Action"/>.
        /// </summary>
        /// <typeparam name="T">The type of values emitted by this <see cref="IStream{T}"/>.</typeparam>
        /// <param name="stream">The <see cref="IStream{T}"/> stream to bind to.</param>
        /// <param name="action">An action that will be executed every time the <paramref name="stream"/> emits a value of <see cref="StreamValueType.Completed"/>.</param>
        /// <returns>The input <see cref="IStream{T}"/> <paramref name="stream"/>.</returns>
        public static IStream<T> BindComplete<T>(this IStream<T> stream, Action action)
        {
            stream.ValueEmitted += (s, e) =>
            {
                if (e.DataType == StreamValueType.Completed)
                {
                    action();
                }
            };
            return stream;
        }

        #endregion
        #region Properties

        /// <summary>
        /// Returns an <see cref="IStream{T}"/> that emits the change-notified values of the given <typeparamref name="T2"/> property on this <typeparamref name="T1"/> object.
        /// </summary>
        /// <typeparam name="T1">The type of the (usually <see cref="INotifyPropertyChanged"/>) objects that will alert the <see cref="IStream{T}"/> to incoming changes.</typeparam>
        /// <typeparam name="T2">The type of output values this <see cref="IStream{T}"/> produces.</typeparam>
        /// <param name="parent">The parent <see cref="IStream{T}"/> that produces parent objects.</param>
        /// <param name="getProperty">The <see cref="Func{T, TResult}"/> that gets the <typeparamref name="T2"/> property from the <typeparamref name="T1"/> parent.</param>
        /// <param name="propertyName">For debugging purposes, include the name of the property this <see cref="IStream{T}"/> is retrieving.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns the <typeparamref name="T2"/> property values.</returns>
        public static IStream<T2> Property<T1, T2>(this IStream<T1> parent, Func<T1, T2> getProperty, string propertyName = null)
        {
            return new PropertyStream<T1, T2>(parent, getProperty, propertyName);
        }

        /// <summary>
        /// Returns an <see cref="IStream{T}"/> that emits the change-notified values of the given <typeparamref name="T2"/> property on this <typeparamref name="T1"/> object.
        /// </summary>
        /// <typeparam name="T1">The type of the (usually <see cref="INotifyPropertyChanged"/>) objects that will alert the <see cref="IStream{T}"/> to incoming changes.</typeparam>
        /// <typeparam name="T2">The type of output values this <see cref="IStream{T}"/> produces.</typeparam>
        /// <param name="parent">The parent <see cref="IStream{T}"/> that produces parent objects.</param>
        /// <param name="propertyPath">The <see cref="string"/>, dot-delimited path to the desired property.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns <typeparamref name="T2"/> property values through reflection.</returns>
        public static IStream<T2> Property<T1, T2>(this IStream<T1> parent, string propertyPath)
        {
            var pathParts = propertyPath.Split(new string[] { "." }, StringSplitOptions.None);
            Type currentType = typeof(T1);
            IStream<object> currentBinding = parent.Cast<T1, object>();
            foreach (var part in pathParts)
            {
                var property = currentType.GetProperty(part);
                if (property == null)
                {
                    throw new StreamException($"Failed to find property with name {part} on type {currentType.Name}.");
                }

                currentBinding = currentBinding.Property(
                         o => property.GetValue(o),
                         property.Name);

                currentType = property.PropertyType;
            }
            return currentBinding.Cast<object, T2>();
        }

        #endregion
        #region Source

        /// <summary>
        /// Returns a deferred <see cref="IStream{T}"/> that will emit the given value when <see cref="IStream{T}.Start"/> is called.
        /// </summary>
        /// <typeparam name="T">The type of value emitted by this <see cref="IStream{T}"/>.</typeparam>
        /// <param name="value">The singular <typeparamref name="T"/> value to emit.</param>
        /// <returns>An <see cref="IStream{T}"/> that will emit <paramref name="value"/> when <see cref="IStream{T}.Start"/> is called.</returns>
        public static IStream<T> AsStream<T>(this T value)
        {
            return new SourceStream<T>(value);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that will lazily evaluate the provided <see cref="Func{TResult}"/> to find the parent <see cref="IStream{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of output values this <see cref="IStream{T}"/> produces.</typeparam>
        /// <param name="getStream">A <see cref="Func{TResult}"/> that will, when evaluated, return the parent <see cref="IStream{T}"/>.</param>
        /// <returns>An <see cref="IStream{T}"/> that will resolve when <see cref="IStream{T}.Start"/> is called.</returns>
        public static IStream<T> Rec<T>(this Func<IStream<T>> getStream)
        {
            return new RecStream<T>(getStream);
        }

        #endregion
        #region Time

        /// <summary>
        /// Returns an <see cref="IStream{T}"/> that buffers inputs into blocks spanning a time given by a specified <see cref="TimeSpan"/>. The emitted <typeparamref name="T2"/> values are generated by the given buffer function.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the transformed values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="timeSpan">A <see cref="TimeSpan"/> indicating the amount of time to buffer potential inputs.</param>
        /// <param name="bufferFunc">The <see cref="Func{T, TResult}"/> that gets the <typeparamref name="T2"/> value from a buffered list of <typeparamref name="T1"/> items.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns a <typeparamref name="T2"/> output after a <paramref name="timeSpan"/> of time for each group of <typeparamref name="T1"/> inputs recieved during that time.</returns>
        public static IStream<T2> Buffer<T1, T2>(this IStream<T1> stream, TimeSpan timeSpan, Func<IEnumerable<T1>, T2> bufferFunc)
        {
            return new BufferStream<T1, T2>(stream, timeSpan, bufferFunc);
        }

        /// <summary>
        /// Returns an <see cref="IStream{T}"/> that buffers inputs over a time given by a specified <see cref="TimeSpan"/>. The emitted <typeparamref name="T"/> values are the first from each buffered group.
        /// </summary>
        /// <typeparam name="T">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T"/> values.</param>
        /// <param name="timeSpan">A <see cref="TimeSpan"/> indicating the amount of time to buffer potential inputs.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns the first <typeparamref name="T"/> output after a <paramref name="timeSpan"/> of time for each group of <typeparamref name="T"/> inputs recieved during that time.</returns>
        public static IStream<T> BufferFirst<T>(this IStream<T> stream, TimeSpan timeSpan)
        {
            return new BufferStream<T, T>(stream, timeSpan, ts => ts.FirstOrDefault());
        }

        /// <summary>
        /// Returns an <see cref="IStream{T}"/> that buffers inputs over a time given by a specified <see cref="TimeSpan"/>. The emitted <typeparamref name="T"/> values are the last from each buffered group.
        /// </summary>
        /// <typeparam name="T">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T"/> values.</param>
        /// <param name="timeSpan">A <see cref="TimeSpan"/> indicating the amount of time to buffer potential inputs.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns the last <typeparamref name="T"/> output after a <paramref name="timeSpan"/> of time for each group of <typeparamref name="T"/> inputs recieved during that time.</returns>
        public static IStream<T> BufferLast<T>(this IStream<T> stream, TimeSpan timeSpan)
        {
            return new BufferStream<T, T>(stream, timeSpan, ts => ts.LastOrDefault());
        }

        #endregion
        #region Tasks

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that executes the <see cref="Task{TResult}"/>s of the parent <see cref="IStream{T}"/> asynchronously, and returns the results as they arrive.
        /// </summary>
        /// <typeparam name="T">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <see cref="Task{TResult}"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting <typeparamref name="T"/> outputs asynchronously.</returns>        
        public static IStream<T> Await<T>(this IStream<Task<T>> stream)
        {
            return new ParallelMapStream<Task<T>, T>(stream, t => t);
        }

        #endregion
    }
}
