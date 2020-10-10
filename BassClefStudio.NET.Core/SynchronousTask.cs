using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        /// A <see cref="bool"/> value indicating whether multiple calls to a <see cref="RunTaskAsync"/> or <see cref="RunTask"/> method while the task is running will use the same instance of the task (true), or create a different instance each time (false).
        /// </summary>
        public bool IsLocked { get; }

        /// <summary>
        /// The <see cref="Action"/> to take if the task throws an <see cref="Exception"/>.
        /// </summary>
        public Action<Exception> ExceptionAction { get; }

        /// <summary>
        /// Creates a new <see cref="SynchronousTask"/>.
        /// </summary>
        /// <param name="myTask">The task to attach.</param>
        /// <param name="exceptionAction">The action to take if the <see cref="Task"/> throws an <see cref="Exception"/>. Defaults to <see cref="DefaultExceptionAction(Exception)"/>.</param>
        /// <param name="isLocked">A <see cref="bool"/> value indicating whether multiple calls to a <see cref="RunTaskAsync"/> or <see cref="RunTask"/> method while the task is running will use the same instance of the task (true), or create a different instance each time (false).</param>
        public SynchronousTask(Func<Task> myTask, Action<Exception> exceptionAction = null, bool isLocked = false)
        {
            GetMyTask = myTask;
            ExceptionAction = exceptionAction ?? DefaultExceptionAction;
            IsLocked = isLocked;
        }

        /// <summary>
        /// Runs the attached <see cref="Task"/> on the background thread in a try/catch block and calls <see cref="ExceptionAction"/> if an <see cref="Exception"/> is thrown.
        /// </summary>
        public async Task RunTaskAsync()
        {
            var task = GetTaskWithLock(GetMyTask);
            if (task != null)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    ExceptionAction(ex);
                }
                finally
                {
                    CompleteTask();
                }
            }
        }

        /// <summary>
        /// Runs the attached <see cref="Task"/> on the background thread in a try/catch block and calls <see cref="ExceptionAction"/> only if an <see cref="Exception"/> of type <typeparamref name="T"/> is thrown.
        /// </summary>
        public async Task RunTaskAsync<T>() where T : Exception
        {
            var task = GetTaskWithLock(GetMyTask);
            if (task != null)
            {
                try
                {
                    await task;
                }
                catch (T ex)
                {
                    ExceptionAction(ex);
                }
                finally
                {
                    CompleteTask();
                }
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

        private Task lockedTask = null;
        private object taskLock = new object();
        private Task GetTaskWithLock(Func<Task> getTask)
        {
            if (IsLocked)
            {
                lock (taskLock)
                {
                    if (lockedTask == null)
                    {
                        lockedTask = getTask();
                    }
                }

                return lockedTask;
            }
            else
            {
                return getTask();
            }
        }

        private void CompleteTask()
        {
            lock (taskLock)
            {
                if (IsLocked)
                {
                    lockedTask = null;
                }
            }
        }

        /// <summary>
        /// The default exception action for a <see cref="SynchronousTask"/> - writes the exception to the debug console and then throws the exception.
        /// </summary>
        /// <param name="ex">The exception caught by the <see cref="SynchronousTask"/>.</param>
        public static void DefaultExceptionAction(Exception ex)
        {
            Debug.WriteLine($"Exception thrown in synchronous task:\r\n{ex}");
            throw ex;
        }
    }

    /// <summary>
    /// Contains extension methods for awaiting collections of <see cref="Task"/>s.
    /// </summary>
    public static class ParallelTaskExtensions
    {
        /// <summary>
        /// Starts and awaits a collection of <see cref="Task"/>s in parallel.
        /// </summary>
        /// <param name="tasks">The collection of <see cref="Task"/>s.</param>
        public static async Task RunParallelAsync(this IEnumerable<Func<Task>> tasks)
            => await RunParallelAsync(tasks.Select(t => t()));
        /// <summary>
        /// Awaits a collection of <see cref="Task"/>s in parallel.
        /// </summary>
        /// <param name="tasks">The collection of <see cref="Task"/>s.</param>
        public static async Task RunParallelAsync(this IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
            {
                await task;
            }
        }

        /// <summary>
        /// Starts and awaits a collection of <see cref="Task"/>s in parallel, returning an <see cref="IEnumerable{T}"/> of the results.
        /// </summary>
        /// <param name="tasks">The collection of <see cref="Task"/>s.</param>
        public static async Task<IEnumerable<T>> RunParallelAsync<T>(this IEnumerable<Func<Task<T>>> tasks)
            => await RunParallelAsync(tasks.Select(t => t()));
        /// <summary>
        /// Awaits a collection of <see cref="Task"/>s in parallel, returning an <see cref="IEnumerable{T}"/> of the results.
        /// </summary>
        /// <param name="tasks">The collection of <see cref="Task"/>s.</param>
        public static async Task<IEnumerable<T>> RunParallelAsync<T>(this IEnumerable<Task<T>> tasks)
        {
            List<T> results = new List<T>();
            foreach (var task in tasks)
            {
                results.Add(await task);
            }

            return results;
        }
    }
}
