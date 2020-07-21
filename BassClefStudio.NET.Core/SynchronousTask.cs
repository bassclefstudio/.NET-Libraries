using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Core
{
    /// <summary>
    /// Represents a wrapper around a <see cref="Task"/> that allows it to start without awaiting and callback if an <see cref="Exception"/> is thrown.
    /// </summary>
    public class SynchronousTask
    {
        /// <summary>
        /// The function to create the attached <see cref="Task"/>.
        /// </summary>
        public Func<Task> GetMyTask { get; }

        /// <summary>
        /// The <see cref="Action"/> to take if the task throws an <see cref="Exception"/>.
        /// </summary>
        public Action<Exception> ExceptionAction { get; }

        /// <summary>
        /// Creates a new <see cref="SynchronousTask"/>.
        /// </summary>
        /// <param name="myTask">The task to attach.</param>
        /// <param name="exceptionAction">The action to take if the <see cref="Task"/> throws an <see cref="Exception"/>. Defaults to <see cref="DefaultExceptionAction(Exception)"/>.</param>
        public SynchronousTask(Func<Task> myTask, Action<Exception> exceptionAction = null)
        {
            GetMyTask = myTask;
            ExceptionAction = exceptionAction ?? DefaultExceptionAction;
        }

        /// <summary>
        /// Runs the <see cref="Task"/> on the background thread in a try/catch block and calls <see cref="ExceptionAction"/> if an <see cref="Exception"/> is thrown.
        /// </summary>
        public async Task RunTaskAsync()
        {
            try
            {
                await GetMyTask();
            }
            catch (Exception ex)
            {
                ExceptionAction(ex);
            }
        }

        /// <summary>
        /// Runs the attached <see cref="Task"/> on the background thread in a try/catch block and calls <see cref="ExceptionAction"/> only if an <see cref="Exception"/> of type <typeparamref name="T"/> is thrown.
        /// </summary>
        public async Task RunTaskAsync<T>() where T : Exception
        {
            try
            {
                await GetMyTask();
            }
            catch (T ex)
            {
                ExceptionAction(ex);
            }
        }

        /// <summary>
        /// Runs the attached <see cref="Task"/> on the background thread with the <see cref="ExceptionAction"/> attached. To simply start the <see cref="Task"/> without waiting, use _ = <see cref="RunTask"/>;
        /// </summary>
        public async Task RunTask()
        {
            await Task.Run(async () =>
                await RunTaskAsync());
        }

        public static void DefaultExceptionAction(Exception ex)
        {
            Debug.WriteLine($"Exception thrown in synchronous task:\r\n{ex}");
            throw ex;
        }
    }
}
