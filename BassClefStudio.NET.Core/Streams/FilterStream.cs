using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// Represents an <see cref="IStream{T}"/> which filters only certain events from a parent <see cref="IStream{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of values emitted by this <see cref="IStream{T}"/>.</typeparam>
    public class FilterStream<T> : IStream<T>
    {
        /// <inheritdoc/>
        public bool Started { get; private set; } = false;

        /// <inheritdoc/>
        public StreamBinding<T> ValueEmitted { get; }

        /// <summary>
        /// The parent <see cref="IStream{T}"/> this <see cref="FilterStream{T}"/> will filter.
        /// </summary>
        public IStream<T> ParentStream { get; }

        /// <summary>
        /// A function that returns a <see cref="bool"/> for each <typeparamref name="T"/> input indicating whether it should propogate onto this stream.
        /// </summary>
        public Func<T, bool> Filter { get; }

        /// <summary>
        /// Creates a new <see cref="FilterStream{T}"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="IStream{T}"/> this <see cref="FilterStream{T}"/> will filter.</param>
        /// <param name="filter">A function that returns a <see cref="bool"/> for each <typeparamref name="T"/> input indicating whether it should propogate onto this stream.</param>
        public FilterStream(IStream<T> parent, Func<T, bool> filter)
        {
            ValueEmitted = new StreamBinding<T>();
            ParentStream = parent;
            Filter = filter;
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

        private void ParentValueEmitted(StreamValue<T> value)
        {
            if (value.DataType == StreamValueType.Result)
            {
                try
                {
                    if (Filter(value.Result))
                    {
                        ValueEmitted.EmitValue(value);
                    }
                }
                catch(Exception ex)
                {
                    ValueEmitted.EmitValue(new StreamValue<T>(ex));
                }
            }
            else
            {
                ValueEmitted.EmitValue(value);
            }
        }
    }
}
