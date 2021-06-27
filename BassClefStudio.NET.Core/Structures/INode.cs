using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Core.Structures
{
    /// <summary>
    /// Represents a single identifiable node in a graph.
    /// </summary>
    public interface INode : IIdentifiable<string>, IEquatable<INode>
    { }

    /// <summary>
    /// A basic <see cref="INode"/> containing only a <see cref="string"/> ID.
    /// </summary>
    public struct Node : INode
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// Creates a new <see cref="Node"/>.
        /// </summary>
        /// <param name="id">The <see cref="Node"/>'s <see cref="string"/> ID.</param>
        public Node(string id)
        {
            Id = id;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Node node && Equals(node);
        }

        /// <inheritdoc/>
        public bool Equals(INode other)
        {
            return other is Node node && Equals(node);
        }

        /// <inheritdoc/>
        public bool Equals(Node other)
        {
            return Id == other.Id;
        }
    }
}
