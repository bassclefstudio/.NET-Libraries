using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// An <see cref="IStream{T}"/> that filters incoming inputs from a parent <see cref="IStream{T}"/> with the previously emitted item, allowing for the filtering and comparing of distinct items.
    /// </summary>
    /// <typeparam name="T">The type of output values this <see cref="IStream{T}"/> produces.</typeparam>
    public class DistinctStream<T> : IStream<T>
    {
        /// <inheritdoc/>
        public bool Started { get; private set; }

        /// <summary>
        /// The previously emitted value from <see cref="ParentStream"/>.
        /// </summary>
        public T PreviousValue { get; private set; }

        /// <summary>
        /// A <see cref="Func{T1, T2, TResult}"/> that takes in the incoming and previous <typeparamref name="T"/> inputs and returns a <see cref="bool"/> indicating whether the incoming value should be emitted.
        /// </summary>
        public Func<T, T, bool> IncludeFunc { get; }

        /// <summary>
        /// The parent <see cref="IStream{T}"/> this <see cref="DistinctStream{T}"/> is based on.
        /// </summary>
        public IStream<T> ParentStream { get; }

        /// <inheritdoc/>
        public StreamBinding<T> ValueEmitted { get; }

        /// <summary>
        /// Creates a new <see cref="DistinctStream{T}"/>.
        /// </summary>
        /// <param name="parentStream">The parent <see cref="IStream{T}"/> this <see cref="DistinctStream{T}"/> is based on.</param>
        /// <param name="includeFunc">A <see cref="Func{T1, T2, TResult}"/> that takes in the incoming and previous <typeparamref name="T"/> inputs and returns a <see cref="bool"/> indicating whether the incoming value should be emitted.</param>
        public DistinctStream(IStream<T> parentStream, Func<T, T, bool> includeFunc)
        {
            ValueEmitted = new StreamBinding<T>();
            ParentStream = parentStream;
            IncludeFunc = includeFunc;
        }

        /// <inheritdoc/>
        public void Start()
        {
            if(!Started)
            {
                Started = true;
                PreviousValue = default(T);
                ParentStream.ValueEmitted.AddAction(ParentValueEmitted);
                ParentStream.Start();
            }
        }

        private void ParentValueEmitted(StreamValue<T> e)
        {
            if (e.DataType == StreamValueType.Completed)
            {
                ValueEmitted.EmitValue(new StreamValue<T>());
            }
            else if (e.DataType == StreamValueType.Error)
            {
                ValueEmitted.EmitValue(new StreamValue<T>(e.Error));
            }
            else if (e.DataType == StreamValueType.Result)
            {
                try
                {
                    if (IncludeFunc(e.Result, PreviousValue))
                    {
                        ValueEmitted.EmitValue(new StreamValue<T>(e.Result));
                        PreviousValue = e.Result;
                    }
                }
                catch (Exception ex)
                {
                    ValueEmitted.EmitValue(new StreamValue<T>(ex));
                }
            }
        }
    }
}
