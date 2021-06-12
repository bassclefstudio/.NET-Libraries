using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// Represents a reactive stream, which outputs some series of values asynchronously over time.
    /// </summary>
    /// <typeparam name="T">The type of values emitted by the stream (see <see cref="ValueEmitted"/>).</typeparam>
    public interface IStream<T>
    {
        /// <summary>
        /// A <see cref="bool"/> indicating whether this <see cref="IStream{T}"/> has been started yet (see <see cref="Start"/>).
        /// </summary>
        bool Started { get; }

        /// <summary>
        /// A <see cref="StreamBinding{T}"/> managed by the <see cref="IStream{T}"/> and triggered every time a new value is emitted.
        /// </summary>
        StreamBinding<T> ValueEmitted { get; }

        /// <summary>
        /// If this <see cref="IStream{T}"/> contains data that is ready to be emitted, this will start the <see cref="IStream{T}"/>. Call this method after all binding and transformations to desired <see cref="IStream{T}"/>s have occurred.
        /// </summary>
        void Start();
    }

    /// <summary>
    /// Represents a more open pub/sub binding which <see cref="IStream{T}"/>s call when they emit values.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="StreamValue{T}"/> values this <see cref="StreamBinding{T}"/> handles.</typeparam>
    public class StreamBinding<T>
    {
        /// <summary>
        /// The keyed collection of <see cref="Action{T}"/>s to take when a new <see cref="StreamValue{T}"/> is emitted.
        /// </summary>
        public Dictionary<string, Action<StreamValue<T>>> Actions { get; }

        /// <summary>
        /// Creates a new empty <see cref="StreamBinding{T}"/>.
        /// </summary>
        public StreamBinding()
        {
            Actions = new Dictionary<string, Action<StreamValue<T>>>();
        }

        /// <summary>
        /// Adds a new action with the given key to the bound <see cref="Actions"/>.
        /// </summary>
        /// <param name="key">The <see cref="string"/> key of the action in the <see cref="Actions"/> collection.</param>
        /// <param name="action">The action to take when a <see cref="StreamValue{T}"/> is received.</param>
        public void AddAction(string key, Action<StreamValue<T>> action)
        {
            Actions.Add(key, action);
        }

        /// <summary>
        /// Adds a new action with a <see cref="Guid"/> key to the bound <see cref="Actions"/>.
        /// </summary>
        /// <param name="action">The action to take when a <see cref="StreamValue{T}"/> is received.</param>
        /// <returns>The <see cref="string"/> form of the GUID key.</returns>
        public string AddAction(Action<StreamValue<T>> action)
        {
            string key = Guid.NewGuid().ToString();
            Actions.Add(key, action);
            return key;
        }

        /// <summary>
        /// Removes the action with the given key from the <see cref="Actions"/> collection.
        /// </summary>
        /// <param name="key">The <see cref="string"/> key of the action in the <see cref="Actions"/> collection.</param>
        public void RemoveAction(string key)
        {
            Actions.Remove(key);
        }

        /// <summary>
        /// Internally called by <see cref="IStream{T}"/>s - emits a <see cref="StreamValue{T}"/> and calls all the relevant <see cref="Actions"/>.
        /// </summary>
        /// <param name="value">The <see cref="StreamValue{T}"/> value to emit.</param>
        public void EmitValue(StreamValue<T> value)
        {
            foreach (var a in Actions)
            {
                a.Value(value);
            }
        }
    }
}
