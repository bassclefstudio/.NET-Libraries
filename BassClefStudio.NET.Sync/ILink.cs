using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    public interface ILink<T>
    {
        Task PushAsync(T item);
        Task UpdateAsync(T item);
    }
}
