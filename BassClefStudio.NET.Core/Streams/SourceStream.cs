using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// Represents the most basic implementation of <see cref="IStream{T}"/> that produces content.
    /// </summary>
    /// <typeparam name="T">The type of output values this <see cref="IStream{T}"/> produces.</typeparam>
    public class SourceStream<T> : IStream<T>
    {
        /// <inheritdoc/>
        public bool Started { get; private set; } = false;

        /// <inheritdoc/>
        public event EventHandler<StreamValue<T>> ValueEmitted;

        /// <summary>
        /// Creates an empty <see cref="SourceStream{T}"/>.
        /// </summary>
        public SourceStream()
        {
            StartInputs = Array.Empty<StreamValue<T>>();
        }

        /// <summary>
        /// A collection of <see cref="StreamValue{T}"/> inputs that will be sent onto the <see cref="SourceStream{T}"/> when <see cref="IStream{T}.Start"/> is called.
        /// </summary>
        public IEnumerable<StreamValue<T>> StartInputs { get; protected set; }

        /// <summary>
        /// Creates a <see cref="SourceStream{T}"/> with a collection of <see cref="StreamValue{T}"/> inputs.
        /// </summary>
        /// <param name="inputs">A collection of <see cref="StreamValue{T}"/> inputs that will be sent onto the <see cref="SourceStream{T}"/> when <see cref="IStream{T}.Start"/> is called.</param>
        public SourceStream(IEnumerable<StreamValue<T>> inputs)
        {
            StartInputs = inputs;
        }

        /// <summary>
        /// Creates a <see cref="SourceStream{T}"/> with a collection of <see cref="StreamValue{T}"/> inputs.
        /// </summary>
        /// <param name="inputs">A collection of <see cref="StreamValue{T}"/> inputs that will be sent onto the <see cref="SourceStream{T}"/> when <see cref="IStream{T}.Start"/> is called.</param>
        public SourceStream(params StreamValue<T>[] inputs)
        {
            StartInputs = inputs;
        }

        /// <summary>
        /// Creates a <see cref="SourceStream{T}"/> with a collection of <typeparamref name="T"/> inputs.
        /// </summary>
        /// <param name="inputs">A collection of <typeparamref name="T"/> inputs that will be sent onto the <see cref="SourceStream{T}"/> when <see cref="IStream{T}.Start"/> is called.</param>
        public SourceStream(IEnumerable<T> inputs)
        {
            StartInputs = inputs.Select(t => new StreamValue<T>(t));
        }

        /// <summary>
        /// Creates a <see cref="SourceStream{T}"/> with a collection of <typeparamref name="T"/> inputs.
        /// </summary>
        /// <param name="inputs">A collection of <typeparamref name="T"/> inputs that will be sent onto the <see cref="SourceStream{T}"/> when <see cref="IStream{T}.Start"/> is called.</param>
        public SourceStream(params T[] inputs)
        {
            StartInputs = inputs.Select(t => new StreamValue<T>(t));
        }

        /// <summary>
        /// Creates a <see cref="SourceStream{T}"/> that returns consecutive <see cref="int"/> values.
        /// </summary>
        /// <param name="start">The starting <see cref="int"/> to begin with.</param>
        /// <param name="length">The number of items to emit.</param>
        /// <returns>A new <see cref="SourceStream{T}"/> that outputs <see cref="int"/> values.</returns>
        public static SourceStream<int> CountStream(int start, int length)
        {
            return new SourceStream<int>(Enumerable.Range(start, length));
        }

        /// <summary>
        /// Creates a <see cref="SourceStream{T}"/> that returns some <typeparamref name="T"/> value multiple times.
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> value to emit.</param>
        /// <param name="length">The number of items to emit.</param>
        /// <returns>A new <see cref="SourceStream{T}"/> that outputs <paramref name="value"/> <paramref name="length"/> times.</returns>
        public static SourceStream<T> Repeat(T value, int length)
        {
            return new SourceStream<T>(Enumerable.Repeat(value, length));
        }

        /// <inheritdoc/>
        public void Start()
        {
            if (!Started)
            {
                Started = true;
                EmitValues(StartInputs);
            }
        }

        /// <summary>
        /// Emits a value to the stream.
        /// </summary>
        /// <param name="input">The pertinent <typeparamref name="T"/> value.</param>
        public void EmitValue(StreamValue<T> input)
        {
            ValueEmitted?.Invoke(this, input);
        }

        /// <summary>
        /// Emits a collection of values to the stream.
        /// </summary>
        /// <param name="inputs">The pertinent <typeparamref name="T"/> values.</param>
        public void EmitValues(IEnumerable<StreamValue<T>> inputs)
        {
            foreach (var item in inputs)
            {
                EmitValue(item);
            }
        }

        /// <summary>
        /// Emits a value to the stream.
        /// </summary>
        /// <param name="input">The pertinent <typeparamref name="T"/> value.</param>
        public void EmitValue(T input)
        {
            ValueEmitted?.Invoke(this, new StreamValue<T>(input));
        }

        /// <summary>
        /// Emits a collection of values to the stream.
        /// </summary>
        /// <param name="inputs">The pertinent <typeparamref name="T"/> values.</param>
        public void EmitValues(IEnumerable<T> inputs)
        {
            foreach (var item in inputs)
            {
                EmitValue(item);
            }
        }

        /// <summary>
        /// Throws an exception onto the stream.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> describing the error.</param>
        public void ThrowError(Exception ex)
        {
            ValueEmitted?.Invoke(this, new StreamValue<T>(ex));
        }

        /// <summary>
        /// Completes (see <see cref="StreamValueType.Completed"/>) the stream.
        /// </summary>
        public void Complete()
        {
            ValueEmitted?.Invoke(this, new StreamValue<T>());
        }
    }
}
