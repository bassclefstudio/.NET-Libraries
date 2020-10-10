using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    /// <summary>
    /// Represents an object of type <typeparamref name="T"/> cached locally and synced with a remote data source (such as an API, file, or database).
    /// </summary>
    /// <typeparam name="T">The type of the item to sync.</typeparam>
    public interface ISyncItem<T>
    {
        /// <summary>
        /// The locally cached <typeparamref name="T"/> item.
        /// </summary>
        T Item { get; set; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="Item"/> has been initialized from the source.
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// Updates <see cref="Item"/> with the latest content from the data source.
        /// </summary>
        Task UpdateAsync(ISyncInfo<T> info = null);

        /// <summary>
        /// Pushes the content of <see cref="Item"/> to the data source.
        /// </summary>
        Task PushAsync(ISyncInfo<T> info = null);
    }


    public interface IKeyedSyncItem<T, TKey> : ISyncItem<T>, IIdentifiable<TKey> where T : IIdentifiable<TKey> where TKey : IEquatable<TKey>
    { }

    public interface ISyncCollection<T> : ISyncItem<ObservableCollection<T>>
    { }
}
