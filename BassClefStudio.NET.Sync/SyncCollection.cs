using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    public abstract class SyncCollection<TItem, TKey> : ISyncItem<ObservableCollection<TItem>> where TKey : IEquatable<TKey>
    {
        public ObservableCollection<TItem> Item { get; }

        protected abstract IList<TItem> GetLink(TKey key);

        public Task PushAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
