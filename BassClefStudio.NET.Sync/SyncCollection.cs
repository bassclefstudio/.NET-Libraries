using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    /// <summary>
    /// A wrapper for a <see cref="SyncItem{T}"/> of an <see cref="IList{T}"/> of syncable <see cref="IKeyedSyncItem{T, TKey}"/> items, with item values of type <typeparamref name="T"/>. Note that this class is only for managing <see cref="IIdentifiable{T}"/> keyed items - to simply sync an <see cref="IList{T}"/> as one item, use <see cref="SyncItem{T}"/> or another implementation of <see cref="ISyncItem{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the <see cref="IList{T}"/> being synced.</typeparam>
    /// <typeparam name="TKey">The type of the <see cref="IIdentifiable{T}.Id"/> on each <typeparamref name="T"/> item.</typeparam>
    public class KeyedSyncCollection<T, TKey> : SyncItem<IList<IKeyedSyncItem<T, TKey>>> where T : IIdentifiable<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Creates a new <see cref="KeyedSyncCollection{T, TKey}"/> with a backing store of an <see cref="ObservableCollection{T}"/> to allow for collection changed notifications.
        /// </summary>
        /// <param name="items">A collection of <see cref="ISyncItem{T}"/> items to initialize the <see cref="ObservableCollection{T}"/> with.</param>
        /// <param name="link">An <see cref="ILink{T}"/> connecting this <see cref="SyncItem{T}"/> to a remote data source.</param>
        public KeyedSyncCollection(IEnumerable<IKeyedSyncItem<T, TKey>> items, ILink<IList<IKeyedSyncItem<T, TKey>>> link = null) : base(new ObservableCollection<IKeyedSyncItem<T, TKey>>(items), link)
        { }

        /// <summary>
        /// Creates a new <see cref="KeyedSyncCollection{T, TKey}"/> with a backing store of an empty <see cref="ObservableCollection{T}"/> to allow for collection changed notifications.
        /// </summary>
        /// <param name="link">An <see cref="ILink{T}"/> connecting this <see cref="SyncItem{T}"/> to a remote data source.</param>
        public KeyedSyncCollection(ILink<IList<IKeyedSyncItem<T, TKey>>> link = null) : base(new ObservableCollection<IKeyedSyncItem<T, TKey>>(), link)
        { }

        /// <summary>
        /// Creates a new <see cref="KeyedSyncCollection{T, TKey}"/>.
        /// </summary>
        /// <param name="list">The currently cached or created <see cref="IList{T}"/> of syncable <see cref="ISyncItem{T}"/> items.</param>
        /// <param name="link">An <see cref="ILink{T}"/> connecting this <see cref="SyncItem{T}"/> to a remote data source.</param>
        public KeyedSyncCollection(IList<IKeyedSyncItem<T, TKey>> list, ILink<IList<IKeyedSyncItem<T, TKey>>> link = null) : base(list, link)
        { }
    }

    /// <summary>
    /// An <see cref="ILink{T}"/> that manages an <see cref="IList{T}"/> collection of keyed items. Designed for use with the <see cref="KeyedSyncCollection{T, TKey}"/> class.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the <see cref="IList{T}"/> being synced.</typeparam>
    /// <typeparam name="TKey">The type of the <see cref="IIdentifiable{T}.Id"/> on each <typeparamref name="T"/> item.</typeparam>
    public class KeyedCollectionLink<T, TKey> : ILink<IList<IKeyedSyncItem<T, TKey>>> where T : IIdentifiable<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// A <see cref="Func{T, TResult}"/> that can create an <see cref="ILink{T}"/> for an item given its <typeparamref name="TKey"/> key.
        /// </summary>
        public Func<TKey, ILink<T>> CreateLinkFunc { get; }

        /// <summary>
        /// A <see cref="Func{TResult}"/> that asynchronously gets a collection of all <typeparamref name="TKey"/> keys in the collection.
        /// </summary>
        public Func<Task<IEnumerable<TKey>>> GetKeysAsync { get; }

        /// <summary>
        /// A <see cref="Func{TResult}"/> that asynchronously deletes the item with the given <typeparamref name="TKey"/> from the backing store.
        /// </summary>
        public Func<TKey, Task> DeleteItemAsync { get; }

        /// <summary>
        /// A <see cref="Func{TResult}"/> that asynchronously creates the item with the given <typeparamref name="TKey"/> in the backing store.
        /// </summary>
        public Func<TKey, Task> CreateItemAsync { get; }

        /// <summary>
        /// Creates a new <see cref="KeyedCollectionLink{T, TKey}"/>.
        /// </summary>
        /// <param name="linkFunc">A <see cref="Func{T, TResult}"/> that can create an <see cref="ILink{T}"/> for an item given its <typeparamref name="TKey"/> key.</param>
        /// <param name="getKeys">A <see cref="Func{TResult}"/> that asynchronously gets a collection of all <typeparamref name="TKey"/> keys in the collection.</param>
        /// <param name="deleteTask">A <see cref="Func{TResult}"/> that asynchronously deletes the item with the given <typeparamref name="TKey"/> from the backing store.</param>
        /// <param name="createTask">A <see cref="Func{TResult}"/> that asynchronously creates the item with the given <typeparamref name="TKey"/> in the backing store.</param>
        public KeyedCollectionLink(Func<TKey, ILink<T>> linkFunc, Func<Task<IEnumerable<TKey>>> getKeys, Func<TKey, Task> deleteTask, Func<TKey, Task> createTask)
        {
            CreateLinkFunc = linkFunc;
            GetKeysAsync = getKeys;
            DeleteItemAsync = deleteTask;
            CreateItemAsync = createTask;
        }

        /// <summary>
        /// Creates a new <see cref="KeyedCollectionLink{T, TKey}"/> which does not perform specific creation or deletion operations on the backing data store.
        /// </summary>
        /// <param name="linkFunc">A <see cref="Func{T, TResult}"/> that can create an <see cref="ILink{T}"/> for an item given its <typeparamref name="TKey"/> key.</param>
        /// <param name="getKeys">A <see cref="Func{TResult}"/> that asynchronously gets a collection of all <typeparamref name="TKey"/> keys in the collection.</param>
        public KeyedCollectionLink(Func<TKey, ILink<T>> linkFunc, Func<Task<IEnumerable<TKey>>> getKeys)
        {
            CreateLinkFunc = linkFunc;
            GetKeysAsync = getKeys;
            DeleteItemAsync = k => Task.CompletedTask;
            CreateItemAsync = k => Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(ISyncItem<IList<IKeyedSyncItem<T, TKey>>> item)
        {
            var keys = await GetKeysAsync();
            IEnumerable<IKeyedSyncItem<T, TKey>> toRemove = item.Item.Where(s => !keys.Contains(s.Id)).ToArray();
            IEnumerable<TKey> toAdd = keys.Where(k => !item.Item.Select(s => s.Id).Contains(k)).ToArray();
            foreach (var rem in toRemove)
            {
                item.Item.Remove(rem);
            }

            foreach (var add in toAdd)
            {
                var newItem = new KeyedSyncItem<T, TKey>(CreateLinkFunc(add));
                item.Item.Add(newItem);
            }

            var updateTasks = item.Item.Select(i => i.UpdateAsync());
            await Task.WhenAll(updateTasks);
        }

        /// <inheritdoc/>
        public async Task PushAsync(ISyncItem<IList<IKeyedSyncItem<T, TKey>>> item)
        {
            var keys = await GetKeysAsync();
            IEnumerable<TKey> toAdd = item.Item.Select(s => s.Id).Where(k => !keys.Contains(k)).ToArray();
            IEnumerable<TKey> toRemove = keys.Where(k => !item.Item.Select(s => s.Id).Contains(k)).ToArray();

            var removeTasks = toRemove.Select(r => DeleteItemAsync(r));
            await Task.WhenAll(removeTasks);

            var addTasks = toAdd.Select(a => CreateItemAsync(a));
            await Task.WhenAll(addTasks);

            var pushTasks = item.Item.Select(i => i.PushAsync());
            await Task.WhenAll(pushTasks);
        }
    }
}
