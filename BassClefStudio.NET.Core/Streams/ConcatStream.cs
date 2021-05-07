using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// An <see cref="IStream{T}"/> that concatenates and sends out the combined <see cref="StreamValue{T}"/> values of all the parent <see cref="IStream{T}"/>s.
    /// </summary>
    /// <typeparam name="T">The type of output values this <see cref="IStream{T}"/> produces.</typeparam>
    public class ConcatStream<T> : IStream<T>
    {
        /// <inheritdoc/>
        public bool Started { get; private set; } = false;

        /// <summary>
        /// A collection of <see cref="IStream{T}"/> parent streams, from which emitted values are passed to the <see cref="ConcatStream{T}"/>.
        /// </summary>
        public IStream<T>[] ParentStreams { get; }

        /// <inheritdoc/>
        public event EventHandler<StreamValue<T>> ValueEmitted;

        /// <summary>
        /// Creates a new <see cref="ConcatStream{T}"/>.
        /// </summary>
        /// <param name="parents">A collection of <see cref="IStream{T}"/> parent streams, from which emitted values are passed to the <see cref="ConcatStream{T}"/>.</param>
        public ConcatStream(params IStream<T>[] parents)
        {
            ParentStreams = parents;
        }

        /// <summary>
        /// Creates a new <see cref="ConcatStream{T}"/>.
        /// </summary>
        /// <param name="parents">A collection of <see cref="IStream{T}"/> parent streams, from which emitted values are passed to the <see cref="ConcatStream{T}"/>.</param>
        public ConcatStream(IEnumerable<IStream<T>> parents)
        {
            ParentStreams = parents.ToArray();
        }

        /// <inheritdoc/>
        public void Start()
        {
            if (!Started)
            {
                Started = true;
                foreach (var p in ParentStreams)
                {
                    p.ValueEmitted += ParentValueEmitted;
                }

                foreach (var p in ParentStreams)
                {
                    p.Start();
                }
            }
        }

        private void ParentValueEmitted(object sender, StreamValue<T> e)
        {
            ValueEmitted?.Invoke(this, e);
        }
    }
}
