using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    public class SyncItem<T> : Observable, ISyncItem<T>
    {
        private T item;
        /// <inheritdoc/>
        public T Item { get => item; set { Set(ref item, value); ItemChanged?.Invoke(this, new EventArgs()); } }

        public event EventHandler ItemChanged;

        /// <inheritdoc/>
        public bool Initialized => Item != null;

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
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(ISyncInfo<T> info = null)
        {
            await Link.UpdateAsync(this, info);
        }

        /// <inheritdoc/>
        public async Task PushAsync(ISyncInfo<T> info = null)
        {
            await Link.PushAsync(this, info);
        }
    }

    public class KeyedSyncItem<T, TKey> : SyncItem<T>, IKeyedSyncItem<T, TKey> where T : IIdentifiable<TKey> where TKey : IEquatable<TKey>
    {
        /// <inheritdoc/>
        public TKey Id { get; private set; }

        public KeyedSyncItem(T item, ILink<T> link = null) : base(item, link)
        {
            ItemChanged += KeyedItemChanged;
        }

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
