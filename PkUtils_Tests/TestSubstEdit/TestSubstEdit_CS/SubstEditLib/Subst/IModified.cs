// Ignore Spelling: Utils
//
using System;

namespace PK.SubstEditLib.Subst;

/// <summary>
/// A delegate used by <see cref="IModified.EventModified"/>event.
/// </summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">The vent-specific data.</param>
[CLSCompliant(true)]
public delegate void ModifiedEventHandler(IModified sender, EventArgs e);

/// <summary>
/// Interface IModified defines functionality that should be supported any modifiable object.
/// </summary>
[CLSCompliant(true)]
public interface IModified
{
    /// <summary>
    /// The event raised when the property <see cref="IsModified"/> becomes true.
    /// </summary>
    event ModifiedEventHandler EventModified;

    /// <summary>
    /// Returns true if the object has been modified; false otherwise.
    /// </summary>
    bool IsModified { get; }

    /// <summary>
    /// Sets the value of related property <see cref="IsModified "/> to given <paramref name="bValue"/>. <br/>
    /// If the new value is true, the event <see cref="EventModified"/> will be raised.
    /// </summary>
    /// <param name="bValue">The new value of related property.</param>
    void SetModified(bool bValue);
}

/// <summary>
/// Extension methods for IModified
/// </summary>
[CLSCompliant(true)]
public static class ModifiedExtensions
{
    #region Methods

    /// <summary>
    /// Calls IModified.SetModified(true)
    /// </summary>
    /// <param name="iMod">The object being modified.</param>
    public static void SetModified(this IModified iMod)
    {
        iMod.SetModified(true);
    }
    #endregion // Methods
}
