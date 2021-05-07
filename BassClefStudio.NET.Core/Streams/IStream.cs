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
        /// An event produced by the <see cref="IStream{T}"/> every time a new value is available.
        /// </summary>
        event EventHandler<StreamValue<T>> ValueEmitted;

        /// <summary>
        /// If this <see cref="IStream{T}"/> contains data that is ready to be emitted, this will start the <see cref="IStream{T}"/>. Call this method after all binding and transformations to desired <see cref="IStream{T}"/>s have occurred.
        /// </summary>
        void Start();
    }

    /// <summary>
    /// A struct that provides some value of type <typeparamref name="T"/> if successful, else contains information about the error that occurred.
    /// </summary>
    /// <typeparam name="T">The type of value this <see cref="StreamValue{T}"/> could encapsulate.</typeparam>
    public struct StreamValue<T>
    {
        /// <summary>
        /// A <see cref="StreamValueType"/> indicating what type of result this <see cref="StreamValue{T}"/> encapsulates.
        /// </summary>
        public StreamValueType DataType { get; private set; }

        private T result;
        /// <summary>
        /// The <typeparamref name="T"/> result, if <see cref="DataType"/> is set to <see cref="StreamValueType.Result"/> true.
        /// </summary>
        public T Result
        {
            get
            {
                if(DataType == StreamValueType.Result)
                {
                    return result;
                }
                else
                {
                    throw new InvalidOperationException("Cannot retrieve the value of Maybe<T>.Result if DataType is not 'Result'.");
                }
            }
        }

        private Exception error;
        /// <summary>
        /// If <see cref="DataType"/> is set to <see cref="StreamValueType.Error"/>, contains the information about the <see cref="Exception"/> that was thrown.
        /// </summary>
        public Exception Error
        {
            get
            {
                if (DataType == StreamValueType.Error)
                {
                    return error;
                }
                else
                {
                    throw new InvalidOperationException("Cannot retrieve the value of Maybe<T>.Error if DataType is not 'Error'.");
                }
            }
        }

        /// <summary>
        /// Creates a successful <see cref="StreamValue{T}"/> object that contains a <see cref="Result"/>.
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> result.</param>
        public StreamValue(T value)
        {
            DataType = StreamValueType.Result;
            result = value;
            error = null;
        }

        /// <summary>
        /// Creates a failed <see cref="StreamValue{T}"/> object that provides <see cref="Exception"/> information.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> that was thrown.</param>
        public StreamValue(Exception ex)
        {
            DataType = StreamValueType.Error;
            result = default(T);
            error = ex;
        }
    }

    /// <summary>
    /// For a <see cref="StreamValue{T}"/> produced by an <see cref="IStream{T}"/>, indicates the type of data being sent.
    /// </summary>
    public enum StreamValueType
    {
        /// <summary>
        /// The default type contains no value or error, and simply indicates the <see cref="IStream{T}"/> has completed producing values.
        /// </summary>
        Completed = 0,
        /// <summary>
        /// This <see cref="StreamValue{T}"/> will contain a value as its <see cref="StreamValue{T}.Result"/>.
        /// </summary>
        Result = 1,
        /// <summary>
        /// An error occurred, and information can be found in <see cref="StreamValue{T}.Error"/>.
        /// </summary>
        Error = 2
    }
}
