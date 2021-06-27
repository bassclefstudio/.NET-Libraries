using BassClefStudio.NET.Core.Structures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Tests
{
    [TestClass]
    public class GraphTests
    {
        [TestMethod]
        public void TestCreateGraphC()
        {
            Graph testGraph = new Graph();
            Node[] nodes = new Node[]
            {
                new Node("0"),
                new Node("1"),
                new Node("2"),
                new Node("3"),
                new Node("4")
            };

            Connection<Node>[] connections = new Connection<Node>[]
            {
                new Connection<Node>(nodes[0], nodes[1]),
                new Connection<Node>(nodes[1], nodes[2]),
                new Connection<Node>(nodes[2], nodes[3]),
                new Connection<Node>(nodes[3], nodes[4])
            };

            foreach (var connection in connections)
            {
                testGraph.AddConnection(connection);
            }

            Assert.AreEqual(nodes.Length, testGraph.Nodes.Count, "Incorrect number of nodes found in graph.");
            Assert.AreEqual(connections.Length, testGraph.Connections.Count, "Incorrect number of connections found in graph.");

            foreach (var connection in connections)
            {
                testGraph.RemoveConnection(connection);
            }

            Assert.AreEqual(nodes.Length, testGraph.Nodes.Count, "Incorrect number of nodes found in graph after connections removed.");
            Assert.AreEqual(0, testGraph.Connections.Count, "Expected all connections removed.");

            foreach (var node in nodes)
            {
                testGraph.RemoveNode(node);
            }

            Assert.AreEqual(0, testGraph.Nodes.Count, "Expected all nodes removed.");
            Assert.AreEqual(0, testGraph.Connections.Count, "Expected all connections removed.");
        }

        [TestMethod]
        public void TestCreateGraphN()
        {
            Graph testGraph = new Graph();
            Node[] nodes = new Node[]
            {
                new Node("0"),
                new Node("1"),
                new Node("2"),
                new Node("3"),
                new Node("4")
            };

            Connection<Node>[] connections = new Connection<Node>[]
            {
                new Connection<Node>(nodes[0], nodes[1]),
                new Connection<Node>(nodes[1], nodes[2]),
                new Connection<Node>(nodes[2], nodes[3]),
                new Connection<Node>(nodes[3], nodes[4])
            };

            foreach (var node in nodes)
            {
                testGraph.AddNode(node);
            }

            Assert.AreEqual(nodes.Length, testGraph.Nodes.Count, "Incorrect number of nodes found in graph.");

            foreach (var connection in connections)
            {
                testGraph.AddConnection(connection);
            }

            Assert.AreEqual(nodes.Length, testGraph.Nodes.Count, "Incorrect number of nodes found in graph.");
            Assert.AreEqual(connections.Length, testGraph.Connections.Count, "Incorrect number of connections found in graph.");

            foreach (var node in nodes)
            {
                testGraph.RemoveNode(node);
            }

            Assert.AreEqual(0, testGraph.Nodes.Count, "Expected all nodes removed.");
            Assert.AreEqual(0, testGraph.Connections.Count, "Expected all connections removed.");
        }

        [TestMethod]
        public void TestCreatePath()
        {
            Node[] nodes = new Node[]
            {
                new Node("0"),
                new Node("1"),
                new Node("2"),
                new Node("3"),
                new Node("4")
            };

            Connection<Node>[] connections = new Connection<Node>[]
            {
                new Connection<Node>(nodes[0], nodes[1]),
                new Connection<Node>(nodes[1], nodes[2]),
                new Connection<Node>(nodes[2], nodes[3]),
                new Connection<Node>(nodes[3], nodes[4])
            };

            Path<Node, Connection<Node>> path = new Path<Node, Connection<Node>>(nodes[0], connections);
            Assert.AreEqual(nodes[4], path.EndNode, "Incorrect calculated end node.");
            Assert.AreEqual(ConnectionMode.Forwards, path.GetConnectionMode(), "Incorrect calculated connection mode.");
        }

        [TestMethod]
        public void TestFindPath()
        {
            Graph testGraph = new Graph();
            Node[] nodes = new Node[]
            {
                new Node("0"),
                new Node("1"),
                new Node("2"),
                new Node("3"),
                new Node("4")
            };

            Connection<Node>[] connections = new Connection<Node>[]
            {
                new Connection<Node>(nodes[0], nodes[1]),
                new Connection<Node>(nodes[1], nodes[2]),
                new Connection<Node>(nodes[2], nodes[3]),
                new Connection<Node>(nodes[3], nodes[4]),
                new Connection<Node>(nodes[1], nodes[4])
            };

            foreach (var connection in connections)
            {
                testGraph.AddConnection(connection);
            }

            var path = testGraph.FindPath(nodes[0], nodes[4]);
            Assert.IsNotNull(path, "Path between nodes not found.");
            Assert.AreEqual(nodes[0], path.StartNode, "Incorrect path start node.");
            Assert.AreEqual(nodes[0], path.StartNode, "Incorrect path end node.");
            Assert.AreEqual(2, path.Connections.Count(), "Number of connections was not optimal value.");
            Assert.AreEqual(ConnectionMode.Forwards, path.GetConnectionMode(), "Incorrect calculated connection mode.");
        }

        [TestMethod]
        public void PathNotExist()
        {
            Graph testGraph = new Graph();
            Node[] nodes = new Node[]
            {
                new Node("0"),
                new Node("1"),
                new Node("2"),
                new Node("3"),
                new Node("4")
            };

            Connection<Node>[] connections = new Connection<Node>[]
            {
                new Connection<Node>(nodes[0], nodes[1]),
                new Connection<Node>(nodes[1], nodes[2]),
                new Connection<Node>(nodes[3], nodes[2]),
                new Connection<Node>(nodes[3], nodes[4]),
            };

            foreach (var connection in connections)
            {
                testGraph.AddConnection(connection);
            }

            var path1 = testGraph.FindPath(nodes[0], nodes[4]);
            Assert.IsNull(path1, "Path between nodes found, but does not exist.");
            var path2 = testGraph.FindPath(nodes[0], nodes[4], false);
            Assert.IsNotNull(path2, "Indiscriminate path between nodes not found.");
            Assert.AreEqual(nodes[0], path2.StartNode, "Incorrect path start node.");
            Assert.AreEqual(nodes[0], path2.StartNode, "Incorrect path end node.");
            Assert.AreEqual(4, path2.Connections.Count(), "Number of connections was not optimal value.");
            Assert.AreEqual(ConnectionMode.Closed, path2.GetConnectionMode(), "Incorrect calculated connection mode.");
        }
    }
}
