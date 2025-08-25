namespace PK.Commands.Interfaces;

#pragma warning disable IDE1006  // Naming rule violation: These words must begin with upper case characters



/// <summary>
/// Interface representing a generic command option.  
/// Inherits from <see cref="IParameterBase"/> to add the specific option value and possible values.  
/// </summary>
/// <typeparam name="T"> The type of the value for this option.  </typeparam>
/// 
/// <seealso cref="IParameterBase"/>
/// <seealso cref="IParameterSwitch"/>
public interface IParameterOption<T> : IParameterBase
{
    /// <summary>  
    /// Gets or sets the possible values for this option.  
    /// Null indicates no validation required by the CommandHelper processing.  
    /// </summary>
    string[] possibleValues { get; init; }

    /// <summary>  
    /// Gets or sets the value of this option.  
    /// </summary>
    T optionValue { get; init; }
}


#pragma warning restore IDE1006
