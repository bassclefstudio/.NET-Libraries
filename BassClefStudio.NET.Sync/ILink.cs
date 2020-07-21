using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    /// <summary>
    /// Represents the link between a data store (such as a file, web API, or database) and an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILink<T>
    {
        /// <summary>
        /// Retrieve the associated item asynchronously from the store.
        /// </summary>
        Task<T> GetItem();

        /// <summary>
        /// Save the given item to the store asynchronously.
        /// </summary>
        /// <param name="item">The item, of type <typeparamref name="T"/>, to store.</param>
        Task SaveItem(T item);
    }
}
