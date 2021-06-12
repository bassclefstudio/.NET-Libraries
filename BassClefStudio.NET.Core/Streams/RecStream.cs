using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// An <see cref="IStream{T}"/> which attaches to a parent <see cref="IStream{T}"/> only at the point the stream is started (see <see cref="IStream{T}.Start"/>).
    /// </summary>
    /// <typeparam name="T">The type of output values this <see cref="IStream{T}"/> produces.</typeparam>
    public class RecStream<T> : IStream<T>
    {
        /// <inheritdoc/>
        public bool Started { get; private set; }

        /// <summary>
        /// A <see cref="Func{TResult}"/> that will, when evaluated, return the parent <see cref="IStream{T}"/>.
        /// </summary>
        public Func<IStream<T>> GetStream { get; }

        /// <summary>
        /// The evaluated <see cref="IStream{T}"/> parent stream.
        /// </summary>
        private IStream<T> ParentStream { get; set; }

        /// <inheritdoc/>
        public StreamBinding<T> ValueEmitted { get; }

        /// <summary>
        /// Creates a new <see cref="RecStream{T}"/>.
        /// </summary>
        /// <param name="getStream">A <see cref="Func{TResult}"/> that will, when evaluated, return the parent <see cref="IStream{T}"/>.</param>
        public RecStream(Func<IStream<T>> getStream)
        {
            ValueEmitted = new StreamBinding<T>();
            GetStream = getStream;
        }

        /// <inheritdoc/>
        public void Start()
        {
            if(!Started)
            {
                Started = true;
                ParentStream = GetStream();
                ParentStream.ValueEmitted.AddAction(ParentValueEmitted);
                ParentStream.Start();
            }
        }

        private void ParentValueEmitted(StreamValue<T> e)
        {
            ValueEmitted.EmitValue(e);
        }
    }
}
