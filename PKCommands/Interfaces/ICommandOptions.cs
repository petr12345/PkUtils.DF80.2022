using System;
using PK.Commands.CommandUtils;

namespace PK.Commands.Interfaces;

/// <summary>   Interface for class encapsulating several command options. </summary>
/// 
/// <remarks>  This interface is empty because of rather specific requests on the CommandOptions object.
///            Instead of expecting a concrete list of properties or methods, the engine in PK.Commands.CommandUtils
///            expects that general option object has zero, one or more fields of type ParameterOption{T} : IOption{T}.
///            The field name represents the argument (option) name; and all such fields are detected by reflection.
///            See for instance <see cref="CommandHelper.ValidateCommand"/> for details.
/// </remarks>
[CLSCompliant(true)]
public interface ICommandOptions
{
}
