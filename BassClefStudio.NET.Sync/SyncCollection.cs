using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    /// <summary>
    /// Represents synced information about the whole of a synced collection of items.
    /// </summary>
    /// <typeparam name="TKey">The unique key which this table of items is built on.</typeparam>
    public interface ISyncCollectionInfo<T, TKey> : ISyncInfo<T> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets a collection of <typeparamref name="TKey"/> keys for every item found in the collection.
        /// </summary>
        IEnumerable<TKey> GetKeys();
    }

    public abstract class SyncCollection<TItem, TKey> : ISyncCollection<IKeyedSyncItem<TItem, TKey>> where TItem : IIdentifiable<TKey> where TKey : IEquatable<TKey>
    {
        /// <inheritdoc/>
        public ObservableCollection<IKeyedSyncItem<TItem, TKey>> Item { get; }

        /// <inheritdoc/>
        public bool Initialized => true;

        protected abstract ILink<TItem> GetLink(TKey key);

        protected abstract Task<ISyncCollectionInfo<TItem, TKey>> GetCollectionInfo();

        /// <inheritdoc/>
        public async Task UpdateAsync(ISyncInfo<ObservableCollection<IKeyedSyncItem<TItem, TKey>>> info = null)
        {
            var collectionInfo = await GetCollectionInfo();
            Item.Sync(collectionInfo.GetKeys(), i => new KeyedSyncItem<TItem, TKey>(GetLink(i)));
            await Item.Select(i => i.UpdateAsync(collectionInfo)).RunParallelAsync();
        }

        /// <inheritdoc/>
        public async Task PushAsync(ISyncInfo<ObservableCollection<IKeyedSyncItem<TItem, TKey>>> info = null)
        {
            await Item.Select(i => i.PushAsync()).RunParallelAsync();
        }
    }
}
