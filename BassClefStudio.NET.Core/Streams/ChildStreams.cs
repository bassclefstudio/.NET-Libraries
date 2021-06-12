using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// Represents a basic <see cref="IStream{T}"/> that transforms the returned values from another parent <see cref="IStream{T}"/>, one-to-one, in their initial order.
    /// </summary>
    /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
    /// <typeparam name="T2">The type of the transformed values this <see cref="IStream{T}"/> returns.</typeparam>
    public abstract class ChildStream<T1, T2> : IStream<T2>
    {
        /// <inheritdoc/>
        public bool Started { get; private set; } = false;

        /// <inheritdoc/>
        public StreamBinding<T2> ValueEmitted { get; }

        /// <summary>
        /// The parent <see cref="IStream{T}"/> this <see cref="ChildStream{T1, T2}"/> is based on.
        /// </summary>
        public IStream<T1> ParentStream { get; }

        /// <summary>
        /// A method that takes a <typeparamref name="T1"/> object and produces a <typeparamref name="T2"/> object.
        /// </summary>
        /// <param name="input">The input <typeparamref name="T1"/> from <see cref="ParentStream"/>.</param>
        protected abstract T2 ProduceValue(T1 input);

        /// <summary>
        /// Creates a new <see cref="ChildStream{T1, T2}"/> from the 
        /// </summary>
        /// <param name="parent"></param>
        public ChildStream(IStream<T1> parent)
        {
            ValueEmitted = new StreamBinding<T2>();
            ParentStream = parent;
        }

        /// <inheritdoc/>
        public void Start()
        {
            if (!Started)
            {
                Started = true;
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
                    var output = ProduceValue(e.Result);
                    ValueEmitted.EmitValue(new StreamValue<T2>(output));
                }
                catch (Exception ex)
                {
                    ValueEmitted.EmitValue(new StreamValue<T2>(ex));
                }
            }
        }
    }

    /// <summary>
    /// An <see cref="IStream{T}"/>/<see cref="ChildStream{T1, T2}"/> that maps each <typeparamref name="T1"/> value from the parent stream into a <typeparamref name="T2"/> value via a given function.
    /// </summary>
    /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
    /// <typeparam name="T2">The type of the transformed values this <see cref="IStream{T}"/> returns.</typeparam>
    public class MapStream<T1, T2> : ChildStream<T1, T2>
    {
        /// <summary>
        /// The function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.
        /// </summary>
        public Func<T1, T2> ProduceFunc { get; }

        /// <summary>
        /// Creates a new <see cref="MapStream{T1, T2}"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="mapFunc">The function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.</param>
        public MapStream(IStream<T1> parent, Func<T1, T2> mapFunc) : base(parent)
        {
            ProduceFunc = mapFunc;
        }

        /// <inheritdoc/>
        protected override T2 ProduceValue(T1 input)
        {
            return ProduceFunc(input);
        }
    }

    /// <summary>
    /// An <see cref="IStream{T}"/>/<see cref="ChildStream{T1, T2}"/> that aggregates incoming <typeparamref name="T1"/> values from the parent stream into a <typeparamref name="T2"/> <see cref="CurrentState"/>.
    /// </summary>
    /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
    /// <typeparam name="T2">The type of the transformed values this <see cref="IStream{T}"/> returns.</typeparam>
    public class AggregateStream<T1, T2> : ChildStream<T1, T2>
    {
        /// <summary>
        /// A <typeparamref name="T2"/> value indicating the current aggregated state.
        /// </summary>
        public T2 CurrentState { get; private set; }

        /// <summary>
        /// The function that returns a new <typeparamref name="T2"/> aggregate from the <see cref="CurrentState"/> and the next <typeparamref name="T1"/> input.
        /// </summary>
        public Func<T2, T1, T2> AggregateFunc { get; }

        /// <summary>
        /// Creates a new <see cref="AggregateStream{T1, T2}"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="aggregateFunc">The function that returns a new <typeparamref name="T2"/> aggregate from the <see cref="CurrentState"/> and the next <typeparamref name="T1"/> input.</param>
        /// <param name="initialState">The initial <typeparamref name="T2"/> aggregate to set the <see cref="CurrentState"/> to.</param>
        public AggregateStream(IStream<T1> parent, Func<T2, T1, T2> aggregateFunc, T2 initialState = default(T2)) : base(parent)
        {
            AggregateFunc = aggregateFunc;
            CurrentState = initialState;
        }

        /// <inheritdoc/>
        protected override T2 ProduceValue(T1 input)
        {
            CurrentState = AggregateFunc(CurrentState, input);
            return CurrentState;
        }
    }
}
