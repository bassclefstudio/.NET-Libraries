using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    /// <summary>
    /// Represents a connection between data of type <typeparamref name="T"/> and some data on a remote data source (such as an API, file, or database).
    /// </summary>
    /// <typeparam name="T">The type of data to sync through this <see cref="ILink{T}"/>.</typeparam>
    public interface ILink<T>
    {
        /// <summary>
        /// Pushes a given <typeparamref name="T"/> item to the remote data source.
        /// </summary>
        /// <param name="item">An <see cref="ISyncItem{T}"/> wrapper of the <typeparamref name="T"/> item to sync.</param>
        /// <param name="info">Optionally, an <see cref="ILink{T}"/> can use the provided data to access the data store differently or more efficiently (for example, caching related fields).</param>
        Task PushAsync(ISyncItem<T> item, ISyncInfo<T> info = null);

        /// <summary>
        /// Updates a given <typeparamref name="T"/> item with data from the remote source.
        /// </summary>
        /// <param name="item">An <see cref="ISyncItem{T}"/> wrapper of the <typeparamref name="T"/> item to sync.</param>
        /// <param name="info">Optionally, an <see cref="ILink{T}"/> can use the provided data to access the data store differently or more efficiently (for example, caching related fields).</param>
        Task UpdateAsync(ISyncItem<T> item, ISyncInfo<T> info = null);
    }

    /// <summary>
    /// Represents additional information or cached data that an <see cref="ILink{T}"/> can use while syncing data of the given type.
    /// </summary>
    /// <typeparam name="T">The type of data this <see cref="ISyncInfo{T}"/> handles.</typeparam>
    public interface ISyncInfo<T>
    { }
}
