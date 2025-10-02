using System;

namespace PK.Commands.Interfaces;

/// <summary>
/// Defines a specialized command register for commands implementing <see cref="ICommandEx{TErrorCode}"/>.
/// Provides access to the last validated and last executed commands.
/// </summary>
/// <typeparam name="TCommand">
/// The type of supported commands. Must implement <see cref="ICommandEx{TErrorCode}"/>.
/// </typeparam>
/// <typeparam name="TErrorCode">
/// The type representing extended error information for command execution.
/// </typeparam>
[CLSCompliant(true)]
public interface ICommandRegisterEx<TCommand, TErrorCode> : ICommandRegister<TCommand, TErrorCode>
    where TCommand : class, ICommandEx<TErrorCode>
{
    /// <summary> Gets the last command that was subject to validation. </summary>
    /// <remarks>
    /// Command validation occurs before execution. This property returns the most recent command
    /// that underwent validation, regardless of whether validation succeeded or failed,
    /// or whether the command was subsequently executed.
    /// </remarks>
    TCommand LastValidatedCommand { get; }

    /// <summary> Gets the last command that was executed. </summary>
    /// <remarks> Command execution follows validation. </remarks>
    TCommand LastExecutedCommand { get; }

    // Removed unnecessary GetLastError().
    // It is superfluous and not needed, because of existence of ICommandEx.GetLastError().
}
