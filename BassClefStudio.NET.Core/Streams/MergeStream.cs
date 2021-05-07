using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// An <see cref="IStream{T}"/> that maps the most recent outputs from a collection of parent <see cref="IStream{T}"/>s to produce a new output value.
    /// </summary>
    /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
    /// <typeparam name="T2">The type of the transformed values this <see cref="IStream{T}"/> returns.</typeparam>
    public class MergeStream<T1, T2> : IStream<T2>
    {
        /// <inheritdoc/>
        public bool Started { get; private set; } = false;

        /// <summary>
        /// A collection of <see cref="IStream{T}"/> parent streams, from which emitted values are passed to the <see cref="ConcatStream{T}"/>.
        /// </summary>
        public IStream<T1>[] ParentStreams { get; }

        /// <summary>
        /// A <see cref="Func{T, TResult}"/> that produces a <typeparamref name="T2"/> result from the most recent <typeparamref name="T1"/> values produced by each of the <see cref="ParentStreams"/>.
        /// </summary>
        public Func<T1[], T2> TransformFunc { get; }

        /// <inheritdoc/>
        public event EventHandler<StreamValue<T2>> ValueEmitted;

        /// <summary>
        /// Creates a new <see cref="MergeStream{T1, T2}"/>.
        /// </summary>
        /// <param name="parents">A collection of <see cref="IStream{T}"/> parent streams, from which emitted values are passed to the <see cref="ConcatStream{T}"/>.</param>
        /// <param name="transformFunc">A <see cref="Func{T, TResult}"/> that produces a <typeparamref name="T2"/> result from the most recent <typeparamref name="T1"/> values produced by each of the <see cref="ParentStreams"/>.</param>
        public MergeStream(Func<T1[], T2> transformFunc, params IStream<T1>[] parents)
        {
            TransformFunc = transformFunc;
            ParentStreams = parents;
        }

        /// <summary>
        /// Creates a new <see cref="MergeStream{T1, T2}"/>.
        /// </summary>
        /// <param name="parents">A collection of <see cref="IStream{T}"/> parent streams, from which emitted values are passed to the <see cref="ConcatStream{T}"/>.</param>
        /// <param name="transformFunc">A <see cref="Func{T, TResult}"/> that produces a <typeparamref name="T2"/> result from the most recent <typeparamref name="T1"/> values produced by each of the <see cref="ParentStreams"/>.</param>
        public MergeStream(Func<T1[], T2> transformFunc, IEnumerable<IStream<T1>> parents)
        {
            TransformFunc = transformFunc;
            ParentStreams = parents.ToArray();
        }

        /// <inheritdoc/>
        public void Start()
        {
            CachedValues = new T1[ParentStreams.Length];
            if (!Started)
            {
                Started = true;
                for (int i = 0; i < ParentStreams.Length; i++)
                {
                    CachedValues[i] = default(T1);
                    ParentStreams[i].ValueEmitted += (s, e) => ParentValueEmitted(s as IStream<T1>, e);
                }

                foreach (var parent in ParentStreams)
                {
                    parent.Start();
                }
            }
        }

        private T1[] CachedValues { get; set; }

        private void ParentValueEmitted(IStream<T1> parent, StreamValue<T1> e)
            => ParentValueEmitted(Array.IndexOf(ParentStreams, parent), e);
        private void ParentValueEmitted(int index, StreamValue<T1> e)
        {
            if (e.DataType == StreamValueType.Result)
            {
                try
                {
                    CachedValues[index] = e.Result;
                    var output = TransformFunc(CachedValues);
                    ValueEmitted?.Invoke(this, new StreamValue<T2>(output));
                }
                catch(Exception ex)
                {
                    ValueEmitted?.Invoke(this, new StreamValue<T2>(ex));
                }
            }
            else if(e.DataType == StreamValueType.Completed)
            {
                ValueEmitted?.Invoke(this, new StreamValue<T2>());
                CachedValues[index] = default(T1);
            }
            else if(e.DataType == StreamValueType.Error)
            {
                ValueEmitted?.Invoke(this, new StreamValue<T2>(e.Error));
                CachedValues[index] = default(T1);
            }
        }
    }
}
