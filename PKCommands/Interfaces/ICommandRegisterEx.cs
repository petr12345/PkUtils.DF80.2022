using System;

namespace PK.Commands.Interfaces;

/// <summary>
/// Defines more specialized CommandRegister, working only with commands which implement
/// <see cref="ICommandEx{TErrorCode}"/>.
/// </summary>
///
/// <typeparam name="TCommand">   Generic argument representing type of supported commands. </typeparam>
/// <typeparam name="TErrorCode"> The command execution error code, that extends error
/// information. </typeparam>
[CLSCompliant(true)]
public interface ICommandRegisterEx<TCommand, TErrorCode> : ICommandRegister<TCommand, TErrorCode>
    where TCommand : class, ICommandEx<TErrorCode>
{
    /// <summary>
    /// Gets the last error that occurred during either command validation or execution ( if any ).
    /// </summary>
    ///
    /// <returns>   The last error ( if any ). </returns>
    TErrorCode GetLastError();
}