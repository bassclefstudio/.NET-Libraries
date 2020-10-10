using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    public class SyncItem<T> : ISyncItem<T>
    {
        /// <inheritdoc/>
        public T Item { get; }

        /// <summary>
        /// The <see cref="ILink{T}"/> to the data store.
        /// </summary>
        public ILink<T> Link { get; set; }

        public SyncItem(T item, ILink<T> link = null)
        {
            Item = item;
            Link = link;
        }

        public async Task UpdateAsync()
        {
            await Link.UpdateAsync(Item);
        }

        public async Task PushAsync()
        {
            await Link.PushAsync(Item);
        }
    }
}
