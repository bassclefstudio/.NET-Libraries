using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Core
{
    /// <summary>
    /// Represents an object that can be identified by a <typeparamref name="T"/> key.
    /// </summary>
    /// <typeparam name="T">The type of key.</typeparam>
    public interface IIdentifiable<T> where T : IEquatable<T>
    {
        /// <summary>
        /// The (usually unique) <typeparamref name="T"/> key of the <see cref="IIdentifiable{T}"/> object.
        /// </summary>
        T Id { get; }
    }

    /// <summary>
    /// Extension methods for <see cref="IIdentifiable{T}"/> objects.
    /// </summary>
    public static class IdExtensions
    {
        /// <summary>
        /// Returns a <typeparamref name="TItem"/> value from a collection of <typeparamref name="TItem"/> <see cref="IIdentifiable{T}"/> objects by its unique <typeparamref name="TKey"/> ID.
        /// </summary>
        /// <typeparam name="TItem">The type of <see cref="IIdentifiable{T}"/> to look through.</typeparam>
        /// <typeparam name="TKey">The key of the <typeparamref name="TItem"/> objects.</typeparam>
        /// <param name="items">The collection of <typeparamref name="TItem"/> to search.</param>
        /// <param name="id">The unique <typeparamref name="TKey"/> of the desired item.</param>
        public static TItem Get<TItem, TKey>(this IEnumerable<TItem> items, TKey id) where TItem : IIdentifiable<TKey> where TKey : IEquatable<TKey>
        {
            return items.First(t => t.Id.Equals(id));
        }

        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether a <typeparamref name="TItem"/> with a given <typeparamref name="TKey"/> ID exists in a collection of <typeparamref name="TItem"/> <see cref="IIdentifiable{T}"/> objects.
        /// </summary>
        /// <typeparam name="TItem">The type of <see cref="IIdentifiable{T}"/> to look through.</typeparam>
        /// <typeparam name="TKey">The key of the <typeparamref name="TItem"/> objects.</typeparam>        /// <param name="items">The collection of <typeparamref name="TItem"/> to search.</param>
        /// <param name="id">The unique <typeparamref name="TKey"/> of the desired item.</param>
        public static bool Contains<TItem, TKey>(this IEnumerable<TItem> items, TKey id) where TItem : IIdentifiable<TKey> where TKey : IEquatable<TKey>
        {
            return items.Any(t => t.Id.Equals(id));
        }
    }
}
