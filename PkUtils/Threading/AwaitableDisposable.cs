// Ignore Spelling: Awaiter, Utils
// 
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#pragma warning disable IDE0079    // Remove unnecessary suppressions
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
#pragma warning disable IDE0290 // Use primary constructor

namespace PK.PkUtils.Threading;

/// <summary>
/// An awaitable wrapper around a task whose result is disposable. The wrapper is not disposable, so this
/// prevents usage errors like "using (MyAsync())" when the appropriate usage should be "using (await MyAsync())".
/// </summary>
/// 
/// <remarks>
/// In more detail, let's assume you have a awaitable method, returning task returning IDisposable:
/// <code>
/// <![CDATA[
/// public async Task<IDisposable> GetExclusiveLockAsync();
/// ]]>
/// </code>
/// 
/// You can change the method and return AwaitableDisposable{T} from your GetExclusiveLockAsync, 
/// instead of a regular Task{T}. It's a struct for minimal overhead.
/// The implicit conversion and GetAwaiter/ConfigureAwait methods allow "tasklike" usage:
/// <code>
/// <![CDATA[
/// // All of these work.
/// IDisposable key = await GetExclusiveLockAsync();
/// IDisposable key = await GetExclusiveLockAsync().ConfigureAwait(false);
/// Task<TDisposable> lockTask = GetExclusiveLockAsync();
/// using (await GetExclusiveLockAsync()) { }
/// ]]>
/// </code>
/// 
/// Note: There are some situations where the implicit conversion isn't sufficient, 
/// e.g., some Task.WhenAll uses. For these cases, the user can call AsTask:
/// <code>
/// // Not pretty, but doable.
/// await Task.WhenAll(x.GetExclusiveLockAsync().AsTask(), y.GetExclusiveLockAsync().AsTask());
/// </code>
/// 
/// And, of course, the entire purpose of AwaitableDisposable{T} is that it is not disposable, 
/// so this fails at compile-time: using (GetExclusiveLockAsync()) { }
/// 
/// </remarks>
/// <typeparam name="T">  Generic type parameter that is result of wrapped task. </typeparam>
public readonly struct AwaitableDisposable<T> where T : IDisposable
{
    /// <summary> The underlying task. </summary>
    private readonly Task<T> _task;

    /// <summary> Initializes a new awaitable wrapper around the specified task. </summary>
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
    /// <param name="task"> The underlying task to wrap. This may not be null. </param>
    public AwaitableDisposable(Task<T> task)
    {
        _task = task ?? throw new ArgumentNullException(nameof(task));
    }

    /// <summary> Returns the underlying task. </summary>
    /// <returns> A Task. </returns>
    public Task<T> AsTask()
    {
        return _task;
    }

    /// <summary> Implicit conversion to the underlying task. </summary>
    /// <param name="source">   The awaitable wrapper. </param>
    /// <returns> The result of the operation. </returns>
    public static implicit operator Task<T>(AwaitableDisposable<T> source)
    {
        return source.AsTask();
    }

    /// <summary> Infrastructure. Returns the task awaiter for the underlying task. </summary>
    /// <returns> The awaiter. </returns>
    public TaskAwaiter<T> GetAwaiter()
    {
        return _task.GetAwaiter();
    }

    /// <summary> Infrastructure. Returns a configured task awaiter for the underlying task. </summary>
    /// <param name="continueOnCapturedContext"> Whether to attempt to marshal the continuation back to the
    /// captured context. </param>
    /// <returns> A ConfiguredTaskAwaitable{T} </returns>
    public ConfiguredTaskAwaitable<T> ConfigureAwait(bool continueOnCapturedContext)
    {
        return _task.ConfigureAwait(continueOnCapturedContext);

    }
}

#pragma warning restore IDE0290 // Use primary constructor
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
#pragma warning restore IDE0079