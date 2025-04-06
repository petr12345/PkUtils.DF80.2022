namespace PK.Commands.Interfaces;

#pragma warning disable IDE1006  // Naming rule violation: These words must begin with upper case characters

/// <summary>  
/// Base interface representing a command parameter, which could be an option or a switch.  
/// </summary>
/// 
/// <seealso cref="IParameterSwitch"/>
/// <seealso cref="IParameterOption{T}"/>
public interface IParameterBase
{
    /// <summary>  
    /// Gets or sets the name of this parameter, as declared in the owner class.  
    /// </summary>
    string option { get; init; }

    /// <summary>  
    /// Gets or sets the description of this parameter.  
    /// </summary>
    string description { get; init; }
}


#pragma warning restore IDE1006