using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Core.Structures
{
    /// <summary>
    /// Represents the connection between two <typeparamref name="T"/> nodes in a graph.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="INode"/> nodes this <see cref="IConnection{T}"/> connects.</typeparam>
    public interface IConnection<out T> where T : INode
    {
        /// <summary>
        /// The first <typeparamref name="T"/> node this <see cref="IConnection{T}"/> connects.
        /// </summary>
        T StartNode { get; }

        /// <summary>
        /// The second <typeparamref name="T"/> node this <see cref="IConnection{T}"/> connects.
        /// </summary>
        T EndNode { get; }

        /// <summary>
        /// The <see cref="ConnectionMode"/> describing how this <see cref="IConnection{T}"/> currently functions.
        /// </summary>
        ConnectionMode Mode { get; }
    }

    /// <summary>
    /// A basic <see cref="IConnection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="INode"/> nodes this <see cref="IConnection{T}"/> connects.</typeparam>
    public struct Connection<T> : IConnection<T> where T : INode
    {
        /// <inheritdoc/>
        public T StartNode { get; }

        /// <inheritdoc/>
        public T EndNode { get; }

        /// <inheritdoc/>
        public ConnectionMode Mode { get; }

        /// <summary>
        /// Creates a new <see cref="Connection{T}"/>.
        /// </summary>
        /// <param name="start">The first <typeparamref name="T"/> node this <see cref="IConnection{T}"/> connects.</param>
        /// <param name="end">The second <typeparamref name="T"/> node this <see cref="IConnection{T}"/> connects.</param>
        /// <param name="mode">The <see cref="ConnectionMode"/> describing how this <see cref="IConnection{T}"/> currently functions.</param>
        public Connection(T start, T end, ConnectionMode mode = ConnectionMode.Forwards)
        {
            StartNode = start;
            EndNode = end;
            Mode = mode;
        }
    }

    /// <summary>
    /// An enum defining the ability for a <see cref="IConnection{T}"/> to be used to navigate between two <see cref="INode"/>s.
    /// </summary>
    [Flags]
    public enum ConnectionMode
    {
        /// <summary>
        /// The <see cref="IConnection{T}"/> is closed and no connection can currently be made.
        /// </summary>
        Closed = 0,

        /// <summary>
        /// Connection can be made from the <see cref="IConnection{T}.StartNode"/> to the <see cref="IConnection{T}.EndNode"/>.
        /// </summary>
        Forwards = 1,

        /// <summary>
        /// Connection can be made from the <see cref="IConnection{T}.EndNode"/> to the <see cref="IConnection{T}.StartNode"/>.
        /// </summary>
        Backwards = 2,

        /// <summary>
        /// Connection can be made in either direction between <see cref="IConnection{T}.StartNode"/> and <see cref="IConnection{T}.EndNode"/>.
        /// </summary>
        Both = Forwards | Backwards
    }
}
