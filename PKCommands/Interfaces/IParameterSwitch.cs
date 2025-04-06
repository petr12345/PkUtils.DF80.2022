namespace PK.Commands.Interfaces;

#pragma warning disable IDE1006  // Naming rule violation: These words must begin with upper case characters


/// <summary>  
/// Interface representing a boolean switch command parameter.  
/// Inherits from <see cref="IParameterBase"/> to add the boolean switch value.  
/// </summary>
/// 
/// <seealso cref="IParameterBase"/>
/// <seealso cref="IParameterOption{T}"/>
public interface IParameterSwitch : IParameterBase
{
    /// <summary>  
    /// Gets or sets the value of the boolean switch.  
    /// </summary>
    bool switchValue { get; set; }
}


#pragma warning restore IDE1006