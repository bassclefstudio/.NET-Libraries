using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    /// <summary>
    /// A base class for <see cref="ISyncItem{T}"/> that supports syncing using a single <see cref="ILink{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item to sync.</typeparam>
    public class SyncItem<T> : Observable, ISyncItem<T>
    {
        private T item;
        /// <inheritdoc/>
        public T Item { get => item; set { Set(ref item, value); ItemChanged?.Invoke(this, new EventArgs()); } }

        /// <summary>
        /// An event that is called when the <see cref="Item"/> property is changed.
        /// </summary>
        public event EventHandler ItemChanged;

        private bool isLoading;
        /// <inheritdoc/>
        public bool IsLoading { get => isLoading; set => Set(ref isLoading, value); }

        /// <summary>
        /// The <see cref="ILink{T}"/> to the data store.
        /// </summary>
        public ILink<T> Link { get; set; }

        /// <summary>
        /// Creates a new <see cref="SyncItem{T}"/>.
        /// </summary>
        /// <param name="item">The currently cached or created <typeparamref name="T"/> item.</param>
        /// <param name="link">An <see cref="ILink{T}"/> connecting this <see cref="SyncItem{T}"/> to a remote data source.</param>
        public SyncItem(T item, ILink<T> link = null)
        {
            Item = item;
            Link = link;
        }

        /// <summary>
        /// Creates a new <see cref="SyncItem{T}"/>.
        /// </summary>
        /// <param name="link">An <see cref="ILink{T}"/> connecting this <see cref="SyncItem{T}"/> to a remote data source.</param>
        public SyncItem(ILink<T> link = null)
        {
            Link = link;
            IsLoading = true;
        }

        private Task updateTask;
        /// <inheritdoc/>
        public async Task UpdateAsync()
        {
            if(updateTask == null)
            {
                IsLoading = true;
                updateTask = Link.UpdateAsync(this);
            }
            await updateTask;
            updateTask = null;
            IsLoading = false;
        }

        private Task pushTask;
        /// <inheritdoc/>
        public async Task PushAsync()
        {
            if (pushTask == null)
            {
                IsLoading = true;
                pushTask = Link.PushAsync(this);
            }
            await pushTask;
            pushTask = null;
            IsLoading = false;
        }
    }

    /// <summary>
    /// A <see cref="SyncItem{T}"/> that supports keyed <see cref="IIdentifiable{T}"/> values for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of item being synced.</typeparam>
    /// <typeparam name="TKey">The key which <typeparamref name="T"/> implements as an <see cref="IIdentifiable{T}"/>.</typeparam>
    public class KeyedSyncItem<T, TKey> : SyncItem<T>, IKeyedSyncItem<T, TKey> where T : IIdentifiable<TKey> where TKey : IEquatable<TKey>
    {
        /// <inheritdoc/>
        public TKey Id { get; private set; }

        /// <summary>
        /// Creates a new <see cref="KeyedSyncItem{T, TKey}"/>
        /// </summary>
        /// <param name="item">The currently cached or created <typeparamref name="T"/> item.</param>
        /// <param name="link">An <see cref="ILink{T}"/> connecting this <see cref="SyncItem{T}"/> to a remote data source.</param>
        public KeyedSyncItem(T item, ILink<T> link = null) : base(item, link)
        {
            ItemChanged += KeyedItemChanged;
        }

        /// <summary>
        /// Creates a new <see cref="KeyedSyncItem{T, TKey}"/>
        /// </summary>
        /// <param name="link">An <see cref="ILink{T}"/> connecting this <see cref="SyncItem{T}"/> to a remote data source.</param>
        public KeyedSyncItem(ILink<T> link = null) : base(link)
        {
            ItemChanged += KeyedItemChanged;
        }

        private void KeyedItemChanged(object sender, EventArgs e)
        {
            if(Item != null)
            {
                Id = Item.Id;
            }
            else
            {
                Id = default(TKey);
            }
        }
    }
}
