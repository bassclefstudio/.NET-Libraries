using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Core.Structures
{
    /// <summary>
    /// Represents a single identifiable node in a graph.
    /// </summary>
    public interface INode : IIdentifiable<string>, IEquatable<INode>
    {
    }
}
