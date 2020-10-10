using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    public interface ISyncItem<T>
    {
        /// <summary>
        /// The locally cached <typeparamref name="T"/> item.
        /// </summary>
        T Item { get; }

        Task UpdateAsync();

        Task PushAsync();
    }
}
