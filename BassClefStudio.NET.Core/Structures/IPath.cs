using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Core.Structures
{
    /// <summary>
    /// Represents a description of a path along a collection of <typeparamref name="TConnection"/> connections that links a group of <typeparamref name="TNode"/> nodes in a graph.
    /// </summary>
    /// <typeparam name="TNode">The type of <see cref="INode"/> nodes this <see cref="IPath{TNode, TConnection}"/> links.</typeparam>
    /// <typeparam name="TConnection">The type of <see cref="IConnection{T}"/> connections this <see cref="IPath{TNode, TConnection}"/> traverses.</typeparam>
    public interface IPath<out TNode, out TConnection> : IConnection<TNode> where TNode : INode where TConnection : IConnection<TNode>
    {
        /// <summary>
        /// The ordered collection of <typeparamref name="TConnection"/> connections that make up this contiguous <see cref="IPath{TNode, TConnection}"/>.
        /// </summary>
        IEnumerable<TConnection> Connections { get; }
    }

    /// <summary>
    /// A basic implementation of <see cref="IPath{TNode, TConnection}"/>.
    /// </summary>
    /// <typeparam name="TNode">The type of <see cref="INode"/> nodes this <see cref="IPath{TNode, TConnection}"/> links.</typeparam>
    /// <typeparam name="TConnection">The type of <see cref="IConnection{T}"/> connections this <see cref="IPath{TNode, TConnection}"/> traverses.</typeparam>
    public class Path<TNode, TConnection> : Observable, IPath<TNode, TConnection> where TNode : INode where TConnection : IConnection<TNode>
    {
        /// <inheritdoc/>
        public IEnumerable<TConnection> Connections { get; }

        /// <inheritdoc/>
        public TNode StartNode { get; }

        /// <inheritdoc/>
        public TNode EndNode { get; }

        /// <inheritdoc/>
        public ConnectionMode Mode { get; }

        /// <summary>
        /// Creates a new <see cref="Path{TNode, TConnection}"/>.
        /// </summary>
        /// <param name="start">The <typeparamref name="TNode"/> node this <see cref="IPath{TNode, TConnection}"/> starts at.</param>
        /// <param name="end">The <typeparamref name="TNode"/> node this <see cref="IPath{TNode, TConnection}"/> ends at.</param>
        /// <param name="connections">The ordered collection of <typeparamref name="TConnection"/> connections that make up this contiguous <see cref="IPath{TNode, TConnection}"/>.</param>
        public Path(TNode start, TNode end, IEnumerable<TConnection> connections)
        {
            StartNode = start;
            EndNode = end;
            Connections = connections;
            if(!this.Validate())
            {
                throw new ArgumentException("The provided connections do not link the two nodes.");
            }

            Mode = this.GetConnectionMode();
        }

        /// <summary>
        /// Creates a new <see cref="Path{TNode, TConnection}"/>.
        /// </summary>
        /// <param name="start">The first <typeparamref name="TNode"/> node this <see cref="IPath{TNode, TConnection}"/> connects.</param>
        /// <param name="end">The <typeparamref name="TNode"/> node this <see cref="IPath{TNode, TConnection}"/> ends at.</param>
        /// <param name="connections">The ordered collection of <typeparamref name="TConnection"/> connections that make up this contiguous <see cref="IPath{TNode, TConnection}"/>.</param>
        public Path(TNode start, TNode end, params TConnection[] connections) : this(start, end, (IEnumerable<TConnection>)connections)
        { }

        /// <summary>
        /// Creates a new <see cref="Path{TNode, TConnection}"/> and calculates <see cref="EndNode"/> automatically.
        /// </summary>
        /// <param name="start">The <typeparamref name="TNode"/> node this <see cref="IPath{TNode, TConnection}"/> starts at.</param>
        /// <param name="connections">The ordered collection of <typeparamref name="TConnection"/> connections that make up this contiguous <see cref="IPath{TNode, TConnection}"/>.</param>
        public Path(TNode start, IEnumerable<TConnection> connections)
        {
            StartNode = start;
            Connections = connections;
            EndNode = this.FindEnd();
            if (!this.Validate())
            {
                throw new ArgumentException("The provided connections do not link the two nodes.", "connections");
            }
            Mode = this.GetConnectionMode();
        }

        /// <summary>
        /// Creates a new <see cref="Path{TNode, TConnection}"/> and calculates <see cref="EndNode"/> automatically.
        /// </summary>
        /// <param name="start">The <typeparamref name="TNode"/> node this <see cref="IPath{TNode, TConnection}"/> starts at.</param>
        /// <param name="connections">The ordered collection of <typeparamref name="TConnection"/> connections that make up this contiguous <see cref="IPath{TNode, TConnection}"/>.</param>
        public Path(TNode start, params TConnection[] connections) : this(start, (IEnumerable<TConnection>)connections)
        { }

        /// <summary>
        /// Creates a new <see cref="Path{TNode, TConnection}"/> and calculates <see cref="StartNode"/> and <see cref="EndNode"/> automatically.
        /// </summary>
        /// <param name="connections">The ordered collection of <typeparamref name="TConnection"/> connections that make up this contiguous <see cref="IPath{TNode, TConnection}"/>.</param>
        public Path(IEnumerable<TConnection> connections)
        {
            Connections = connections;
            if(!Connections.Any())
            {
                throw new ArgumentException("The connections collection must have at least one connection if no start or end is specified.", "connections");
            }
            else
            {
                StartNode = Connections.First().StartNode;
                EndNode = this.FindEnd();
                if (!this.Validate())
                {
                    throw new ArgumentException("The provided connections do not link the two nodes.", "connections");
                }
                Mode = this.GetConnectionMode();
            }
        }

        /// <summary>
        /// Creates a new <see cref="Path{TNode, TConnection}"/> and calculates <see cref="StartNode"/> and <see cref="EndNode"/> automatically.
        /// </summary>
        /// <param name="connections">The ordered collection of <typeparamref name="TConnection"/> connections that make up this contiguous <see cref="IPath{TNode, TConnection}"/>.</param>
        public Path(params TConnection[] connections) : this((IEnumerable<TConnection>)connections)
        { }
    }

    /// <summary>
    /// Extension methods for managing and manipulating <see cref="IPath{TNode, TConnection}"/>s in a graph.
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        /// Checks if the <paramref name="path"/> is continuous and connects two valid <typeparamref name="TNode"/> nodes.
        /// </summary>
        /// <typeparam name="TNode">The type of <see cref="INode"/> nodes this <see cref="IPath{TNode, TConnection}"/> links.</typeparam>
        /// <typeparam name="TConnection">The type of <see cref="IConnection{T}"/> connections this <see cref="IPath{TNode, TConnection}"/> traverses.</typeparam>
        /// <param name="path">The <see cref="IPath{TNode, TConnection}"/> path to check.</param>
        /// <returns>A <see cref="bool"/> indicating whether <paramref name="path"/> is continuous.</returns>
        public static bool Validate<TNode, TConnection>(this IPath<TNode, TConnection> path) where TNode : INode where TConnection : IConnection<TNode>
        {
            if (path.Connections.Any())
            {
                TNode currentNode = path.StartNode;
                foreach (var c in path.Connections)
                {
                    if (currentNode.Equals(c.StartNode))
                    {
                        currentNode = c.EndNode;
                    }
                    else if (currentNode.Equals(c.EndNode))
                    {
                        currentNode = c.StartNode;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return path.StartNode.Equals(path.EndNode);
            }
        }

        /// <summary>
        /// Finds the <typeparamref name="TNode"/> node this <see cref="IPath{TNode, TConnection}"/> ends at.
        /// </summary>
        /// <typeparam name="TNode">The type of <see cref="INode"/> nodes this <see cref="IPath{TNode, TConnection}"/> links.</typeparam>
        /// <typeparam name="TConnection">The type of <see cref="IConnection{T}"/> connections this <see cref="IPath{TNode, TConnection}"/> traverses.</typeparam>
        /// <param name="path">The <see cref="IPath{TNode, TConnection}"/> path to check.</param>
        /// <returns>An <see cref="INode"/> end node for the <paramref name="path"/>.</returns>
        public static TNode FindEnd<TNode, TConnection>(this IPath<TNode, TConnection> path) where TNode : INode where TConnection : IConnection<TNode>
        {
            if (path.Connections.Any())
            {
                TNode currentNode = path.StartNode;
                foreach (var c in path.Connections)
                {
                    if (currentNode.Equals(c.StartNode))
                    {
                        currentNode = c.EndNode;
                    }
                    else if (currentNode.Equals(c.EndNode))
                    {
                        currentNode = c.StartNode;
                    }
                    else
                    {
                        throw new ArgumentException("The provided connections do not link the two nodes.", "path");
                    }
                }

                return currentNode;
            }
            else
            {
                return path.StartNode;
            }
        }

        /// <summary>
        /// Gets the <see cref="ConnectionMode"/> mode that can be applied to the given <see cref="IPath{TNode, TConnection}"/> as a whole.
        /// </summary>
        /// <typeparam name="TNode">The type of <see cref="INode"/> nodes this <see cref="IPath{TNode, TConnection}"/> links.</typeparam>
        /// <typeparam name="TConnection">The type of <see cref="IConnection{T}"/> connections this <see cref="IPath{TNode, TConnection}"/> traverses.</typeparam>
        /// <param name="path">The <see cref="IPath{TNode, TConnection}"/> path to check.</param>
        /// <returns>A <see cref="ConnectionMode"/> indicating how the <paramref name="path"/> is traversable.</returns>
        public static ConnectionMode GetConnectionMode<TNode, TConnection>(this IPath<TNode, TConnection> path) where TNode : INode where TConnection : IConnection<TNode>
        {
            if (path.Connections.Any())
            {
                TNode currentNode = path.StartNode;
                ConnectionMode mode = ConnectionMode.Both;
                foreach (var c in path.Connections)
                {
                    if (currentNode.Equals(c.StartNode))
                    {
                        currentNode = c.EndNode;
                        mode &= c.Mode;
                    }
                    else if (currentNode.Equals(c.EndNode))
                    {
                        currentNode = c.StartNode;
                        mode &= c.Mode.Flip();
                    }
                    else
                    {
                        throw new ArgumentException("The provided connections do not link the two nodes.", "path");
                    }
                }

                return mode;
            }
            else
            {
                return ConnectionMode.Both;
            }
        }

        /// <summary>
        /// Flips the given <see cref="ConnectionMode"/> mode to its equivalent for the reverse direction.
        /// </summary>
        /// <param name="mode">The <see cref="ConnectionMode"/> to flip.</param>
        /// <returns>The <see cref="ConnectionMode"/> value for <paramref name="mode"/> from the perspective of travelling in reverse through the relevant <see cref="IConnection{T}"/>.</returns>
        public static ConnectionMode Flip(this ConnectionMode mode)
        {
            if(mode == ConnectionMode.Forwards)
            {
                return ConnectionMode.Backwards;
            }
            else if (mode == ConnectionMode.Backwards)
            {
                return ConnectionMode.Forwards;
            }
            else
            {
                return mode;
            }
        }

        /// <summary>
        /// Creates a new <see cref="IPath{TNode, TConnection}"/> which joins an existing <see cref="IPath{TNode, TConnection}"/> to an additional <see cref="IConnection{T}"/>.
        /// </summary>
        /// <typeparam name="TNode">The type of <see cref="INode"/> nodes this <see cref="IPath{TNode, TConnection}"/> links.</typeparam>
        /// <typeparam name="TConnection">The type of <see cref="IConnection{T}"/> connections this <see cref="IPath{TNode, TConnection}"/> traverses.</typeparam>
        /// <param name="path">The <see cref="IPath{TNode, TConnection}"/> path to concatenate.</param>
        /// <param name="connection">The <see cref="IConnection{T}"/> to add to the <paramref name="path"/>.</param>
        /// <returns>A new <see cref="IPath{TNode, TConnection}"/> instance which is equivalent to <paramref name="path"/> with an added <paramref name="connection"/> at the end.</returns>
        public static IPath<TNode, TConnection> Concat<TNode, TConnection>(this IPath<TNode, TConnection> path, TConnection connection) where TNode : INode where TConnection : IConnection<TNode>
        {
            return new Path<TNode, TConnection>(path.StartNode, path.Connections.Concat(new TConnection[] { connection }));
        }
    }
}
