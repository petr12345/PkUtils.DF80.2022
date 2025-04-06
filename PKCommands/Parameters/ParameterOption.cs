using PK.Commands.Interfaces;

namespace PK.Commands.Parameters;

/// <summary> An option class - implementation of <see cref="IParameterOption{T}"/>. </summary>
///
/// <typeparam name="T"> Generic type parameter - value of the option. </typeparam>
public class ParameterOption<T> : ParameterBase, IParameterOption<T>
{
    /// <summary>  Default argument-less constructor. </summary>
    public ParameterOption()
    { }

    /// <inheritdoc/>
    public string[] possibleValues { get; init; }

    /// <inheritdoc/>
    public T optionValue { get; init; }
}
