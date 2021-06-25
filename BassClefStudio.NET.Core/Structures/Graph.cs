using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Core.Structures
{
    /// <summary>
    /// Represents a mathematical graph data structure, containing <typeparamref name="TNode"/> nodes connected by <typeparamref name="TConnection"/> connections.
    /// </summary>
    /// <typeparam name="TNode">The type of <see cref="INode"/> nodes in the graph.</typeparam>
    /// <typeparam name="TConnection">The type of <see cref="IConnection{T}"/> connections between <typeparamref name="TNode"/> nodes in the graph.</typeparam>
    public class Graph<TNode, TConnection> where TNode : INode where TConnection : IConnection<TNode>
    {
        private ObservableCollection<TNode> nodes;
        /// <summary>
        /// The full collection of all <typeparamref name="TNode"/> nodes in the graph.
        /// </summary>
        public ReadOnlyObservableCollection<TNode> Nodes { get; }

        private ObservableCollection<TConnection> connections;
        /// <summary>
        /// The full collection of all <typeparamref name="TNode"/> nodes in the graph.
        /// </summary>
        public ReadOnlyObservableCollection<TConnection> Connections { get; }

        /// <summary>
        /// Creates a new, empty <see cref="Graph{TNode, TConnection}"/>.
        /// </summary>
        public Graph()
        {
            nodes = new ObservableCollection<TNode>();
            Nodes = new ReadOnlyObservableCollection<TNode>(nodes);
            connections = new ObservableCollection<TConnection>();
            Connections = new ReadOnlyObservableCollection<TConnection>(connections);
        }

        #region Actions

        /// <summary>
        /// Adds a <see cref="IConnection{T}"/> and all its dependencies to the <see cref="Graph{TNode, TConnection}"/>.
        /// </summary>
        /// <param name="connection">The <typeparamref name="TConnection"/> connection to add.</param>
        public void AddConnection(TConnection connection)
        {
            if (!Nodes.Contains(connection.StartNode))
            {
                AddNode(connection.StartNode);
            }

            if (!Nodes.Contains(connection.EndNode))
            {
                AddNode(connection.EndNode);
            }

            connections.Add(connection);
        }

        /// <summary>
        /// Adds a <see cref="INode"/> and all its dependencies to the <see cref="Graph{TNode, TConnection}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> node to add.</param>
        public void AddNode(TNode node)
        {
            nodes.Add(node);
        }

        /// <summary>
        /// Removes a <see cref="IConnection{T}"/> and any dependencies from the <see cref="Graph{TNode, TConnection}"/>.
        /// </summary>
        /// <param name="connection">The <typeparamref name="TConnection"/> connection to remove.</param>
        public void RemoveConnection(TConnection connection)
        {
            connections.Remove(connection);
        }

        /// <summary>
        /// Removes a <see cref="INode"/> and any dependencies from the <see cref="Graph{TNode, TConnection}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> node to remove.</param>
        public void RemoveNode(TNode node)
        {
            nodes.Remove(node);
            foreach (var connection in connections.Where(l => l.StartNode.Equals(node) || l.EndNode.Equals(node)).ToArray())
            {
                RemoveConnection(connection);
            }
        }

        #endregion
    }
}
