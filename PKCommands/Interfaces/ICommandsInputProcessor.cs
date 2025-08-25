using System;
using System.Collections.Generic;
using System.Reflection;
using PK.PkUtils.Interfaces;
using ILogger = log4net.ILog;

namespace PK.Commands.Interfaces;

/// <summary> Interface for text input processor, extends IObserver{string}. 
/// A class implementing this interface is a text input consumer; 
/// i.e. IObserver performing the functionality it is asked for.
/// </summary>
/// <typeparam name="TErrorCode">Type of error details.</typeparam>
/// 
/// <remarks>
/// 
/// - The first important concept here is that a message consumer knows nothing about how messages are produced.
/// The consumer simply reacts to one of the three events (not CLR events) of <see cref="IObserver{T}"/>.
/// 
/// - Besides this, some kind of logic and cross-event ability is available within the consumer itself.
/// If a complete message puts the consumer in a finished state (by signaling the finished flag),
/// any other message that comes on the OnNext method will be automatically routed to the error one.
/// Likewise, any other complete message that reaches the consumer will produce another error once the
/// consumer is already in the finished state.
/// </remarks>
/// 
/// <seealso href="https://www.packtpub.com/web-development/reactive-programming-net-developers">
/// Reactive Programming for .NET Developers
/// </seealso>
[CLSCompliant(true)]
public interface ICommandsInputProcessor<out TErrorCode> : IObserver<string>
{
    /// <summary>
    /// Gets a value indicating whether this object has been initialized by any of InitializeCommandContainer overloads.
    /// </summary>
    bool HasBeenInitialized { get; }

    /// <summary> Gets a value indicating whether this observer has finished processing commands. </summary>
    bool HasFinished { get; }

    /// <summary>   Initializes CommandContainer. </summary>
    ///
    /// <param name="logger"> The logger, used for commands initialization. Can't be null. </param>
    /// <param name="assembly"> (Optional) The assembly where command types to be registered should be searched for. <br/>
    /// If null, will be used Assembly.GetExecutingAssembly. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    bool InitializeCommandContainer(ILogger logger, Assembly assembly = null);

    /// <summary>   Initializes CommandContainer. </summary>
    ///
    /// <param name="logger"> The logger, used for commands initialization. Can't be null. </param>
    /// <param name="cmdTypes"> List of types of the commands to be registered. Can't be null. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    bool InitializeCommandContainer(ILogger logger, IEnumerable<Type> cmdTypes);

    /// <summary>   Process the next input text. </summary>
    ///
    /// <param name="input">            The input, either from the command line or anywhere else. </param>
    /// <param name="shouldContinue">   [out] True if should continue, false otherwise. </param>
    ///
    /// <returns>   An IComplexErrorResult wrapping possible errors. </returns>
    IComplexErrorResult<TErrorCode> ProcessNextInput(string input, out bool shouldContinue);

    /// <summary>   Process the next input of already separated command-line arguments. </summary>
    ///
    /// <param name="args"> The command arguments, including command name as first. Can't be null.</param>
    /// <param name="shouldContinue">   [out] True if should continue, false otherwise. </param>
    ///
    /// <returns>   An IComplexErrorResult wrapping possible errors. </returns>
    IComplexErrorResult<TErrorCode> ProcessNextInput(IEnumerable<string> args, out bool shouldContinue);
}
