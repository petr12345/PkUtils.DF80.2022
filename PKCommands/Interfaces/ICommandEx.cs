using System;
using System.Collections.Generic;

namespace PK.Commands.Interfaces;

/// <summary> Interface for the extended command. </summary>
///
/// <typeparam name="TErrorCode"> The command execution error code, that extends error information. </typeparam>
[CLSCompliant(true)]
public interface ICommandEx<out TErrorCode> : ICommand<TErrorCode>
{
    /// <summary>   Gets the validated arguments, that were passed to <see cref="ICommand{TErrorCode}.Validate"/>. </summary>
    IReadOnlyDictionary<string, string> ValidatedArguments { get; }

    /// <summary>   Gets the last error that occurred during either validation or execution ( if any ). </summary>
    /// <remarks>
    /// We could use single getting method for both, because it does not come to execution if the
    /// validation fails. For more details, see CommandRegister.Execute,
    /// For more details, see CommandRegister.Execute, that calls first ValidateCommand, then ExecuteValidatedCommand.
    /// </remarks>
    ///
    /// <returns> The last error of either <see cref="ICommand{TErrorCode}.Validate"/> 
    /// or <see cref="ICommand{TErrorCode}.Execute"/>. </returns>
    TErrorCode GetLastError();
}
