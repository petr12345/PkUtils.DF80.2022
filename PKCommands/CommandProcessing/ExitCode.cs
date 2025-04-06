namespace PK.Commands.CommandProcessing;

/// <summary>
/// Represents an immutable exit code for the program or commands, 
/// allowing dynamic extension and unified handling.
/// </summary>
public class ExitCode
{
    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="ExitCode"/> class with a specific code and description.
    /// </summary>
    /// <param name="code">The numeric exit code.</param>
    /// <param name="description">The description of the exit code.</param>
    private ExitCode(int code, string description)
    {
        Code = code;
        Description = description;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets the numeric exit code.
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Gets the description of the exit code.
    /// </summary>
    public string Description { get; }

    #endregion // Properties

    #region Predefined program exit codes

    /// <summary>
    /// Indicates successful execution.
    /// </summary>
    public static readonly ExitCode Success = new(0, "Success");

    /// <summary>
    /// Indicates a failure in registering commands.
    /// </summary>
    public static readonly ExitCode FailedToRegisterCommands = new(1, "Failed to register commands");

    /// <summary>
    /// Indicates insufficient privileges for the operation.
    /// </summary>
    public static readonly ExitCode InsufficientPrivileges = new(2, "Insufficient privileges");

    /// <summary>
    /// Indicates that the execution has been canceled.
    /// </summary>
    public static readonly ExitCode ExecutionHasBeenCanceled = new(3, "Execution has been canceled");

    /// <summary>   (Immutable) the i/o error. </summary>
    public static readonly ExitCode IOError = new(4, "IO Error");

    /// <summary>
    /// Indicates a general error during command processing.
    /// </summary>
    public static readonly ExitCode GeneralCommandProcessingError = new(5, "General command processing error");

    /// <summary>
    /// Indicates an unknown error.
    /// </summary>
    public static readonly ExitCode UnknownError = new(1024, "Unknown error");

    /// <summary>
    /// Creates a new exit code for a specific command or custom scenario.
    /// </summary>
    /// <param name="code">The numeric exit code.</param>
    /// <param name="description">The description of the custom exit code.</param>
    /// <returns>A new instance of <see cref="ExitCode"/>.</returns>
    public static ExitCode CreateCommandExitCode(int code, string description)
    {
        return new ExitCode(code, description);
    }
    #endregion // Predefined program exit codes

    #region Methods

    /// <summary>
    /// Determines whether two <see cref="ExitCode"/> instances are equal based on their numeric codes.
    /// </summary>
    /// <param name="left">The first <see cref="ExitCode"/> to compare.</param>
    /// <param name="right">The second <see cref="ExitCode"/> to compare.</param>
    /// <returns><c>true</c> if both exit codes are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(ExitCode left, ExitCode right) => left?.Code == right?.Code;

    /// <summary>
    /// Determines whether two <see cref="ExitCode"/> instances are not equal based on their numeric codes.
    /// </summary>
    /// <param name="left">The first <see cref="ExitCode"/> to compare.</param>
    /// <param name="right">The second <see cref="ExitCode"/> to compare.</param>
    /// <returns><c>true</c> if the exit codes are different; otherwise, <c>false</c>.</returns>
    public static bool operator !=(ExitCode left, ExitCode right) => !(left == right);

    /// <summary>
    /// Returns a string representation of the exit code.
    /// </summary>
    /// <returns>A string containing the code and its description.</returns>
    public override string ToString() => $"{Code}: {Description}";

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ExitCode other && Code == other.Code;

    /// <inheritdoc/>
    public override int GetHashCode() => Code.GetHashCode();

    #endregion // Methods
}

/// <summary> A utility class implementing exit code extensions. </summary>
public static class CommandCodeExtensions
{
    /// <summary>   An ExitCode extension method that query if 'code' is ok. </summary>
    /// <param name="code"> The exit code to act on. </param>
    /// <returns>   True if ok, false if not. </returns>
    public static bool IsOK(this ExitCode code)
    {
        return (code == ExitCode.Success);
    }

    /// <summary>   An ExitCode extension method that query if 'code' has failed. </summary>
    /// <param name="code"> The exit code to act on. </param>
    /// <returns>   True if failed, false if not. </returns>
    public static bool HasFailed(this ExitCode code)
    {
        return (!code.IsOK());
    }

    /// <summary>   An ExitCode extension method that query if has been canceled. </summary>
    ///
    /// <param name="code"> The exit code to act on. </param>
    ///
    /// <returns>   True if been canceled, false if not. </returns>
    public static bool HasBeenCanceled(this ExitCode code)
    {
        return (code == ExitCode.ExecutionHasBeenCanceled);
    }
}
