namespace PK.Commands.Interfaces;

/// <summary> Extends the base <see cref="ICommandsInputProcessor"/> by providing more error information. </summary>
///
/// <typeparam name="TErrorCode">   The command execution error code, that extends error information. </typeparam>
public interface ICommandsInputProcessorEx<TErrorCode> : ICommandsInputProcessor<TErrorCode>
{
    /// <summary>
    /// Gets the last error that occurred during either command validation or execution ( if any ).
    /// </summary>
    /// <returns>   The last error. </returns>
    TErrorCode GetLastError();
}
