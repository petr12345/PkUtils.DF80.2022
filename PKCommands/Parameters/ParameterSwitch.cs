using PK.Commands.Interfaces;

namespace PK.Commands.Parameters;

/// <summary>  A bool switch. </summary>
public class ParameterSwitch : ParameterBase, IParameterSwitch
{
    /// <summary>  Default argument-less constructor. </summary>
    public ParameterSwitch()
    { }

    /// <inheritdoc/>
    public bool switchValue { get; set; }
}