using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BassClefStudio.NET.Core.Streams
{
    /// <summary>
    /// Represents an <see cref="IStream{T}"/> that buffers <typeparamref name="T1"/> inputs from a parent stream over a given span of time, and then returns a single <typeparamref name="T2"/> value.
    /// </summary>
    /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
    /// <typeparam name="T2">The type of the transformed values this <see cref="IStream{T}"/> returns.</typeparam>
    public class BufferStream<T1,T2> : IStream<T2>
    {
        /// <inheritdoc/>
        public bool Started { get; private set; } = false;

        /// <summary>
        /// The parent <see cref="IStream{T}"/> that produces parent objects.
        /// </summary>
        public IStream<T1> ParentStream { get; }

        /// <inheritdoc/>
        public event EventHandler<StreamValue<T2>> ValueEmitted;

        /// <summary>
        /// The <see cref="Timer"/> that this <see cref="BufferStream{T1,T2}"/> uses for buffering requests.
        /// </summary>
        public Timer BufferTimer { get; }

        /// <summary>
        /// The <see cref="Func{T, TResult}"/> that gets the <typeparamref name="T2"/> value from a buffered list of <typeparamref name="T1"/> items.
        /// </summary>
        public Func<IEnumerable<T1>, T2> BufferFunc { get; }

        /// <summary>
        /// A <see cref="TimeSpan"/> indicating the amount of time to buffer potential inputs.
        /// </summary>
        public TimeSpan BufferTime { get; }

        /// <summary>
        /// Creates a new <see cref="BufferStream{T1, T2}"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="IStream{T}"/> that produces parent objects.</param>
        /// <param name="bufferFunc">The <see cref="Func{T, TResult}"/> that gets the <typeparamref name="T2"/> value from a buffered list of <typeparamref name="T1"/> items.</param>
        /// <param name="bufferTime">A <see cref="TimeSpan"/> indicating the amount of time to buffer potential inputs.</param>
        public BufferStream(IStream<T1> parent, TimeSpan bufferTime, Func<IEnumerable<T1>, T2> bufferFunc)
        {
            ParentStream = parent;
            BufferTime = bufferTime;
            BufferFunc = bufferFunc;
            BufferTimer = new Timer(bufferTime.TotalMilliseconds);
            BufferTimer.Elapsed += TimerElapsed;
        }

        List<T1> bufferedItems = new List<T1>();

        private void ParentValueEmitted(object sender, StreamValue<T1> e)
        {
            if(e.DataType == StreamValueType.Error)
            {
                ValueEmitted?.Invoke(this, new StreamValue<T2>(e.Error));
            }
            else if(e.DataType == StreamValueType.Completed)
            {
                ValueEmitted?.Invoke(this, new StreamValue<T2>());
            }
            else if(e.DataType == StreamValueType.Result)
            {
                bufferedItems.Add(e.Result);
                if (!BufferTimer.Enabled)
                {
                    BufferTimer.Start();
                }
            }
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                T2 result = BufferFunc(bufferedItems);
                ValueEmitted?.Invoke(this, new StreamValue<T2>(result));
            }
            catch (Exception ex)
            {
                ValueEmitted?.Invoke(this, new StreamValue<T2>(ex));
            }
            BufferTimer.Stop();
        }

        /// <inheritdoc/>
        public void Start()
        {
            if (!Started)
            {
                Started = true;
                ParentStream.ValueEmitted += ParentValueEmitted;
                ParentStream.Start();
            }
        }
    }
}
