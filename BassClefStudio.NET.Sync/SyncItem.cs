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
        public T Item { get => item; protected set { Set(ref item, value); ItemChanged?.Invoke(this, new EventArgs()); } }

        public event EventHandler ItemChanged;

        /// <inheritdoc/>
        public bool Initialized => Item != null;

        /// <summary>
        /// The <see cref="ILink{T}"/> to the data store.
        /// </summary>
        public ILink<T> Link { get; set; }

        public SyncItem(T item, ILink<T> link = null)
        {
            Item = item;
            Link = link;
        }

        public SyncItem(ILink<T> link = null)
        {
            Link = link;
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(ISyncInfo<T> info = null)
        {
            await Link.UpdateAsync(Item, info);
        }

        /// <inheritdoc/>
        public async Task PushAsync(ISyncInfo<T> info = null)
        {
            await Link.PushAsync(Item, info);
        }
    }

    public class KeyedSyncItem<T, TKey> : SyncItem<T>, IKeyedSyncItem<T, TKey> where T : IIdentifiable<TKey> where TKey : IEquatable<TKey>
    {
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
