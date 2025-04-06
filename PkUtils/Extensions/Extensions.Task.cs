/***************************************************************************************************************
*
* FILE NAME:   .\Extensions\Extensions.Task.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION:
*   The file contains extension-methods class AssemblyExtensions
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;

namespace PK.PkUtils.Extensions;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
#pragma warning disable VSTHRD104 // Offer async methods

/// <summary>
/// Implements extension methods for <see cref="Assembly"/>.
/// </summary>
public static class TaskEx
{
    #region Extension Methods

    /// <summary>
    /// A TaskExtensions extension method that executes synchronously the task operation
    /// and returns the result of the task.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied task is null. </exception>
    ///
    /// <typeparam name="T"> The type of task result. </typeparam>
    /// <param name="t"> The task to act on. Can't be null. </param>
    ///
    /// <returns> A task result. </returns>
    public static T ExecuteSynchronously<T>(this Task<T> t)
    {
        ArgumentNullException.ThrowIfNull(t);

        // The code conditionally-compiled below with Task.Run is workable,
        // but at the heavy cost of a second thread. Instead, one should utilize 
        // JoinableTaskfactory from vs-threading.
        // 
        // JoinableTaskFactory internally sets a custom SynchronizationContext 
        // that will pass the synchronous continuations back to the blocking thread 
        // that was used to initially wait with. This results in a single threaded execution, 
        // which is more economical.
        // For more info, see https://www.anthonysteele.co.uk/AsyncResync.html
#if OLD_SOLUTION
        T result = default;
        Task<T> newTask = Task.Run(async () => result = await t);
        newTask.Wait();
#else
        T result;
        using (var context = new JoinableTaskContext())
        {
            var jtf = new JoinableTaskFactory(context);
            result = jtf.Run(async () => await t);
        }
#endif // OLD_SOLUTION

        return result;
    }

    /// <summary> A TaskExtensions extension method that executes synchronously the task operation. </summary>
    /// <exception cref="ArgumentNullException">    Thrown when a supplied task is null. </exception>
    ///
    /// <param name="t">    The task to act on. Can't be null. </param>
    public static void ExecuteSynchronously(this Task t)
    {
        ArgumentNullException.ThrowIfNull(t);

        using var context = new JoinableTaskContext();
        var jtf = new JoinableTaskFactory(context);
        jtf.Run(async () => await t);
    }

    #endregion // Extension Methods
}
#pragma warning restore VSTHRD104 // Offer async methods
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
#pragma warning restore IDE0079