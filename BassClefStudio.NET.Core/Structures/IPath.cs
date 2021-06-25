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
    public interface IPath<out TNode, out TConnection> where TNode : INode where TConnection : IConnection<TNode>
    {
        /// <summary>
        /// The ordered collection of <typeparamref name="TConnection"/> connections that make up this contiguous <see cref="IPath{TNode, TConnection}"/>.
        /// </summary>
        IEnumerable<TConnection> Connections { get; }
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
                TNode currentNode = path.Connections.First().StartNode;
                foreach (var c in path.Connections)
                {
                    if (!currentNode.Equals(c.StartNode))
                    {
                        return false;
                    }
                    currentNode = c.EndNode;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
