using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Core.Structures
{
    /// <summary>
    /// Represents a mathematical graph data structure, containing <see cref="Node"/> nodes connected by <see cref="Connection{T}"/> connections.
    /// </summary>
    public class Graph : Graph<Node, Connection<Node>>
    { }

    /// <summary>
    /// Represents a mathematical graph data structure, containing <typeparamref name="TNode"/> nodes connected by <see cref="Connection{T}"/> connections.
    /// </summary>
    /// <typeparam name="TNode">The type of <see cref="INode"/> nodes in the graph.</typeparam>
    public class Graph<TNode> : Graph<TNode, Connection<TNode>> where TNode : INode
    { }

    /// <summary>
    /// Represents a mathematical graph data structure, containing <typeparamref name="TNode"/> nodes connected by <typeparamref name="TConnection"/> connections.
    /// </summary>
    /// <typeparam name="TNode">The type of <see cref="INode"/> nodes in the graph.</typeparam>
    /// <typeparam name="TConnection">The type of <see cref="IConnection{T}"/> connections between <typeparamref name="TNode"/> nodes in the graph.</typeparam>
    public class Graph<TNode, TConnection> : Observable where TNode : INode where TConnection : IConnection<TNode>
    {
        /// <summary>
        /// The writable <see cref="Nodes"/> collection.
        /// </summary>
        protected ObservableCollection<TNode> nodes;
        /// <summary>
        /// The full collection of all <typeparamref name="TNode"/> nodes in the graph.
        /// </summary>
        public ReadOnlyObservableCollection<TNode> Nodes { get; }

        /// <summary>
        /// The writable <see cref="Connections"/> collection.
        /// </summary>
        protected ObservableCollection<TConnection> connections;
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
        public virtual void AddConnection(TConnection connection)
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
        public virtual void AddNode(TNode node)
        {
            nodes.Add(node);
        }

        /// <summary>
        /// Removes a <see cref="IConnection{T}"/> and any dependencies from the <see cref="Graph{TNode, TConnection}"/>.
        /// </summary>
        /// <param name="connection">The <typeparamref name="TConnection"/> connection to remove.</param>
        public virtual void RemoveConnection(TConnection connection)
        {
            connections.Remove(connection);
        }

        /// <summary>
        /// Removes a <see cref="INode"/> and any dependencies from the <see cref="Graph{TNode, TConnection}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> node to remove.</param>
        public virtual void RemoveNode(TNode node)
        {
            nodes.Remove(node);
            foreach (var connection in connections.Where(l => l.StartNode.Equals(node) || l.EndNode.Equals(node)).ToArray())
            {
                RemoveConnection(connection);
            }
        }

        #endregion
        #region Queries

        /// <summary>
        /// Get all <typeparamref name="TNode"/> nodes that can be connected to the given <typeparamref name="TNode"/>.
        /// </summary>
        /// <param name="myNode">The <see cref="INode"/> being queried.</param>
        /// <param name="useModes">A <see cref="bool"/> indicating whether the query should take the <see cref="IConnection{T}.Mode"/> into account.</param>
        /// <returns>A collection of all <see cref="INode"/>s that are connected via <see cref="IConnection{T}"/>s to <paramref name="myNode"/>.</returns>
        public IEnumerable<TNode> GetNodes(TNode myNode, bool useModes = true)
        {
            if (useModes)
            {
                return Connections.Where(c => c.Mode.HasFlag(ConnectionMode.Forwards) && c.StartNode.Equals(myNode))
                    .Select(c => c.EndNode)
                    .Concat(Connections.Where(c => c.Mode.HasFlag(ConnectionMode.Backwards) && c.EndNode.Equals(myNode))
                    .Select(c => c.StartNode)).Distinct();
            }
            else
            {
                return Connections.Where(c => c.StartNode.Equals(myNode))
                    .Select(c => c.EndNode)
                    .Concat(Connections.Where(c => c.EndNode.Equals(myNode))
                    .Select(c => c.StartNode)).Distinct();
            }
        }

        /// <summary>
        /// Get all <typeparamref name="TConnection"/> connections from a given <typeparamref name="TNode"/>.
        /// </summary>
        /// <param name="myNode">The <see cref="INode"/> being queried.</param>
        /// <param name="useModes">A <see cref="bool"/> indicating whether the query should take the <see cref="IConnection{T}.Mode"/> into account.</param>
        /// <returns>A collection of <see cref="IConnection{T}"/>s coming from <paramref name="myNode"/>.</returns>
        public IEnumerable<TConnection> GetConnections(TNode myNode, bool useModes = true)
        {
            if (useModes)
            {
                return Connections.Where(c => c.Mode.HasFlag(ConnectionMode.Forwards) && c.StartNode.Equals(myNode))
                    .Concat(Connections.Where(c => c.Mode.HasFlag(ConnectionMode.Backwards) && c.EndNode.Equals(myNode)))
                    .Distinct();
            }
            else
            {
                return Connections.Where(c => c.StartNode.Equals(myNode) || c.EndNode.Equals(myNode));
            }
        }

        /// <summary>
        /// Uses a brute-force method to calculate the shortest <see cref="IPath{TNode, TConnection}"/> between a start and end <typeparamref name="TNode"/>.
        /// </summary>
        /// <param name="start">The <see cref="INode"/> to start at.</param>
        /// <param name="end">The <see cref="INode"/> to end at.</param>
        /// <param name="useModes">A <see cref="bool"/> indicating whether the search should take the <see cref="IConnection{T}.Mode"/> into account when building a route.</param>
        /// <returns>The shortest <see cref="IPath{TNode, TConnection}"/> between the two nodes, or 'null' if none exists.</returns>
        public IPath<TNode, TConnection> FindPath(TNode start, TNode end, bool useModes = true)
        {
            HashSet<TNode> visitedNodes = new HashSet<TNode>();
            IPath<TNode, TConnection>[] paths = new IPath<TNode, TConnection>[] { new Path<TNode, TConnection>(start) };

            while (paths.Length > 0 && !paths.Any(p => p.EndNode.Equals(end)))
            {
                paths = paths.SelectMany(p => GetConnections(p.EndNode, useModes)
                    .Select(c => p.Concat(c)))
                    .Where(p => visitedNodes.Add(p.EndNode))
                    .ToArray();
            }

            return paths.FirstOrDefault(p => p.EndNode.Equals(end));
        }

        #endregion
    }
}
