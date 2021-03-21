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
        Task PushAsync(ISyncItem<T> item);

        /// <summary>
        /// Updates a given <typeparamref name="T"/> item with data from the remote source.
        /// </summary>
        /// <param name="item">An <see cref="ISyncItem{T}"/> wrapper of the <typeparamref name="T"/> item to sync.</param>
        Task UpdateAsync(ISyncItem<T> item);
    }
}
