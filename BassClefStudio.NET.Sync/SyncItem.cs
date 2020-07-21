using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    /// <summary>
    /// Represents an item in the object model of type <typeparamref name="T"/> that is connected and can be synced using a given <see cref="ILink{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item stored in the <see cref="SyncItem{T}"/>.</typeparam>
    public class SyncItem<T> : Observable
    {
        private T item;
        /// <summary>
        /// The locally stored instance of the item, of type <typeparamref name="T"/>.
        /// </summary>
        public T Item
        {
            get => item;
            set
            {
                Set(ref item, value);
                Initialized = item != null;
                ItemChanged?.Invoke(this, new EventArgs());
            }
        }

        private ILink<T> itemLink;
        /// <summary>
        /// The <see cref="ILink{T}"/> between the data store and the <see cref="Item"/>.
        /// </summary>
        public ILink<T> ItemLink
        {
            get => itemLink;
            set
            {
                Set(ref itemLink, value);
            }
        }

        private bool initialized;
        /// <summary>
        /// A <see cref="bool"/> value indicating whether the <see cref="Item"/> property has a stored value, either created locally or synced from the data store.
        /// </summary>
        public bool Initialized { get => initialized; private set => Set(ref initialized, value); }

        /// <summary>
        /// An event that is fired whenever an item is changed.
        /// </summary>
        public event EventHandler ItemChanged;

        /// <summary>
        /// Creates a new <see cref="SyncItem{T}"/> from the given <see cref="ILink{T}"/>.
        /// </summary>
        /// <param name="link">The link between the stored <see cref="Item"/> and the related item in the data store.</param>
        public SyncItem(ILink<T> link = null)
        {
            ItemLink = link;
        }
        
        /// <summary>
        /// Saves the value in <see cref="Item"/> to the data store via the <see cref="ItemLink"/>.
        /// </summary>
        public async Task SaveAsync()
        {
            await ItemLink?.SaveItem(Item);
        }

        /// <summary>
        /// Retrieves the stored value from the <see cref="ItemLink"/> and sets the <see cref="Item"/> property.
        /// </summary>
        public async Task UpdateAsync()
        {
            Item = await ItemLink?.GetItem();
        }
    }
}
