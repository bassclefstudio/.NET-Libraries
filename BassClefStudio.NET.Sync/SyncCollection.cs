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

    public abstract class SyncCollection<TItem, TKey> : Observable, ISyncCollection<IKeyedSyncItem<TItem, TKey>> where TItem : IIdentifiable<TKey> where TKey : IEquatable<TKey>
    {
        private ObservableCollection<IKeyedSyncItem<TItem, TKey>> item;
        /// <inheritdoc/>
        public ObservableCollection<IKeyedSyncItem<TItem, TKey>> Item { get => item; set => Set(ref item, value); }

        /// <inheritdoc/>
        public bool Initialized => true;

        /// <summary>
        /// Gets an <see cref="ILink{T}"/> connection for a child item with the given key.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> key of the item.</param>
        protected abstract ILink<TItem> GetLink(TKey key);

        /// <summary>
        /// Gets the generic collection info which is used to sync and build the collection.
        /// </summary>
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
            if (info is ISyncInfo<TItem> syncInfo)
            {
                await Item.Select(i => i.PushAsync(syncInfo)).RunParallelAsync();
            }
            else
            {
                await Item.Select(i => i.PushAsync()).RunParallelAsync();
            }
        }
    }
}
