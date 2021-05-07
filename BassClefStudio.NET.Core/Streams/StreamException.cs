using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// An <see cref="Exception"/> thrown whenever an <see cref="IStream{T}"/> operation or extension method fails.
    /// </summary>
    [Serializable]
    public class StreamException : Exception
    {
        /// <inheritdoc/>
        public StreamException() { }
        /// <inheritdoc/>
        public StreamException(string message) : base(message) { }
        /// <inheritdoc/>
        public StreamException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected StreamException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
