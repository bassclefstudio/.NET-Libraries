using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    /// <summary>
    /// Represents an object of type <typeparamref name="T"/> cached locally and synced with a remote data source (such as an API, file, or database).
    /// </summary>
    /// <typeparam name="T">The type of the item to sync.</typeparam>
    public interface ISyncItem<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// The locally cached <typeparamref name="T"/> item.
        /// </summary>
        T Item { get; set; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="Item"/> is currently being updated from the source.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether this <see cref="ISyncItem{T}"/> has data in it yet that could be synced.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Updates <see cref="Item"/> with the latest content from the data source.
        /// </summary>
        Task UpdateAsync();

        /// <summary>
        /// Pushes the content of <see cref="Item"/> to the data source.
        /// </summary>
        Task PushAsync();
    }

    /// <summary>
    /// Represents a keyed object of type <typeparamref name="T"/> cached locally and synced with a remote data source (such as an API, file, or database).
    /// </summary>
    /// <typeparam name="T">The type of the item to sync.</typeparam>
    /// <typeparam name="TKey">The type of the key of the <see cref="IIdentifiable{T}"/> item.</typeparam>
    public interface IKeyedSyncItem<T, TKey> : ISyncItem<T>, IIdentifiable<TKey> where T : IIdentifiable<TKey> where TKey : IEquatable<TKey>
    { }
}
