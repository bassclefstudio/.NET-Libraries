using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Core
{
    /// <summary>
    /// Represents an object that can be identified by a <typeparamref name="T"/> key.
    /// </summary>
    /// <typeparam name="T">The type of key.</typeparam>
    public interface IIdentifiable<T>
    {
        /// <summary>
        /// The (usually unique) <typeparamref name="T"/> key of the <see cref="IIdentifiable{T}"/> object.
        /// </summary>
        T Id { get; }
    }
}
