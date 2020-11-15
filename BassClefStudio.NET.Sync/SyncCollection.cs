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
    /// <typeparam name="T">The type of items which are being synced in this collection.</typeparam>
    /// <typeparam name="TKey">The unique key which this table of items is built on.</typeparam>
    public interface ISyncCollectionInfo<T, TKey> : ISyncInfo<T> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets a collection of <typeparamref name="TKey"/> keys for every item found in the collection.
        /// </summary>
        IEnumerable<TKey> GetKeys();
    }

    /// <summary>
    /// Represents a synced keyed <see cref="ISyncCollection{T}"/> of <see cref="ISyncItem{T}"/>s of type <typeparamref name="TItem"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    /// <typeparam name="TKey">The type of the key used to sync each item in the collection.</typeparam>
    public abstract class SyncCollection<TItem, TKey> : Observable, ISyncCollection<IKeyedSyncItem<TItem, TKey>> where TItem : IIdentifiable<TKey> where TKey : IEquatable<TKey>
    {
        private IList<IKeyedSyncItem<TItem, TKey>> item;
        /// <inheritdoc/>
        public IList<IKeyedSyncItem<TItem, TKey>> Item { get => item; set => Set(ref item, value); }

        private bool isLoading;
        /// <inheritdoc/>
        public bool IsLoading { get => isLoading; set => Set(ref isLoading, value); }

        private Func<IList<IKeyedSyncItem<TItem, TKey>>> InitList;

        /// <summary>
        /// Creates a new empty <see cref="SyncCollection{TItem, TKey}"/>.
        /// </summary>
        /// <param name="initList">A <see cref="Func{T, TResult}"/> that should return the default <see cref="IList{T}"/> that should be initialized at construction and whenever the <see cref="Item"/> collection is null while syncing. Defaults to creating an empty <see cref="ObservableCollection{T}"/>.</param>
        public SyncCollection(Func<IList<IKeyedSyncItem<TItem, TKey>>> initList = null)
        {
            InitList = initList ?? (() => new ObservableCollection<IKeyedSyncItem<TItem, TKey>>());
            Item = InitList();
        }

        /// <summary>
        /// Gets an <see cref="ILink{T}"/> connection for a child item with the given key.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> key of the item.</param>
        protected abstract ILink<TItem> GetLink(TKey key);

        /// <summary>
        /// Gets the generic collection info which is used to sync and build the collection.
        /// </summary>
        protected abstract Task<ISyncCollectionInfo<TItem, TKey>> GetCollectionInfo();

        /// <summary>
        /// Creates an <see cref="IKeyedSyncItem{TItem, TKey}"/> item to populate a new item in the collection.
        /// </summary>
        /// <param name="link">The <see cref="ILink{T}"/> created for the syncing of the item.</param>
        protected abstract IKeyedSyncItem<TItem, TKey> CreateSyncItem(ILink<TItem> link);

        /// <inheritdoc/>
        public async Task UpdateAsync(ISyncInfo<IList<IKeyedSyncItem<TItem, TKey>>> info = null)
        {
            IsLoading = true;
            var collectionInfo = await GetCollectionInfo();
            if(Item == null)
            {
                Item = InitList();
            }
            Item.Sync(collectionInfo.GetKeys(), i => CreateSyncItem(GetLink(i)));
            await Item.Select(i => i.UpdateAsync(collectionInfo)).RunParallelAsync();
            IsLoading = false;
        }

        /// <inheritdoc/>
        public async Task PushAsync(ISyncInfo<IList<IKeyedSyncItem<TItem, TKey>>> info = null)
        {
            IsLoading = true;
            if (info is ISyncInfo<TItem> syncInfo)
            {
                await Item.Select(i => i.PushAsync(syncInfo)).RunParallelAsync();
            }
            else
            {
                await Item.Select(i => i.PushAsync()).RunParallelAsync();
            }
            IsLoading = false;
        }
    }
}
