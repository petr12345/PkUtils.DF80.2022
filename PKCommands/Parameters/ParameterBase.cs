using PK.Commands.Interfaces;

namespace PK.Commands.Parameters;

/// <summary>  A base command parameter class. </summary>
public abstract class ParameterBase : IParameterBase
{
    /// <summary> Specialized default constructor for use only by derived class. </summary>
    protected ParameterBase() { }

    /// <inheritdoc/>
    public string option { get; init; }

    /// <inheritdoc/>
    public string description { get; init; }
}