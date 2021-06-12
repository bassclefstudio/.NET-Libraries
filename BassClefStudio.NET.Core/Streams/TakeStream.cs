using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// An <see cref="IStream{T}"/> that manages incoming inputs from a parent <see cref="IStream{T}"/> with previously emitted items, allowing for the comparing of previous values.
    /// </summary>
    /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
    /// <typeparam name="T2">The type of output values this <see cref="IStream{T}"/> produces.</typeparam>
    public class TakeStream<T1, T2> : IStream<T2>
    {
        /// <inheritdoc/>
        public bool Started { get; private set; }

        /// <summary>
        /// The previously emitted value from <see cref="ParentStream"/>.
        /// </summary>
        private Stack<T1> PreviousValues { get; set; }

        /// <summary>
        /// The <see cref="int"/> number of items from the <see cref="ParentStream"/> that should be queued before/when making calls to the <see cref="ProduceFunc"/> is called to create <typeparamref name="T2"/> values.
        /// </summary>
        public int TakeLength { get; }

        /// <summary>
        /// A <see cref="Func{T, TResult}"/> that takes in the incoming and previous <typeparamref name="T1"/> inputs and returns a <typeparamref name="T2"/> value that to be emitted.
        /// </summary>
        public Func<T1[], T2> ProduceFunc { get; }

        /// <summary>
        /// The parent <see cref="IStream{T}"/> this <see cref="DistinctStream{T}"/> is based on.
        /// </summary>
        public IStream<T1> ParentStream { get; }

        /// <inheritdoc/>
        public StreamBinding<T2> ValueEmitted { get; }

        /// <summary>
        /// Creates a new <see cref="DistinctStream{T}"/>.
        /// </summary>
        /// <param name="parentStream">The parent <see cref="IStream{T}"/> this <see cref="TakeStream{T1, T2}"/> is based on.</param>
        /// <param name="produceFunc">A <see cref="Func{T, TResult}"/> that takes in the incoming and previous <typeparamref name="T1"/> inputs and returns a <typeparamref name="T2"/> value that to be emitted.</param>
        /// <param name="takeLength">The <see cref="int"/> number of items from the <see cref="ParentStream"/> that should be queued before/when making calls to the <see cref="ProduceFunc"/> is called to create <typeparamref name="T2"/> values.</param>
        public TakeStream(IStream<T1> parentStream, Func<T1[], T2> produceFunc, int takeLength = 2)
        {
            ValueEmitted = new StreamBinding<T2>();
            ParentStream = parentStream;
            ProduceFunc = produceFunc;
            TakeLength = takeLength;
        }

        /// <inheritdoc/>
        public void Start()
        {
            if (!Started)
            {
                Started = true;
                PreviousValues = new Stack<T1>();
                ParentStream.ValueEmitted.AddAction(ParentValueEmitted);
                ParentStream.Start();
            }
        }

        private void ParentValueEmitted(StreamValue<T1> e)
        {
            if (e.DataType == StreamValueType.Completed)
            {
                ValueEmitted.EmitValue(new StreamValue<T2>());
            }
            else if (e.DataType == StreamValueType.Error)
            {
                ValueEmitted.EmitValue(new StreamValue<T2>(e.Error));
            }
            else if (e.DataType == StreamValueType.Result)
            {
                try
                {
                    PreviousValues.Push(e.Result);
                    if (PreviousValues.Count > TakeLength)
                    {
                        PreviousValues.Pop();
                    }

                    if (PreviousValues.Count == TakeLength)
                    {
                        ValueEmitted.EmitValue(new StreamValue<T2>(ProduceFunc(PreviousValues.ToArray())));
                    }
                }
                catch (Exception ex)
                {
                    ValueEmitted.EmitValue(new StreamValue<T2>(ex));
                }
            }
        }
    }
}
