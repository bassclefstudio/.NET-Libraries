using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Sync
{
    /// <summary>
    /// Represents an <see cref="Exception"/> thrown during the syncing of items between a data store and .NET.
    /// </summary>
    public class SyncException : Exception
    {
        public SyncException() { }
        public SyncException(string message) : base(message) { }
        public SyncException(string message, Exception inner) : base(message, inner) { }
    }
}
