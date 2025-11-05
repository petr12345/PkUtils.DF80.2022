// Ignore Spelling: Utils, ctrl
//
using System;
using System.Windows.Forms;
using PK.PkUtils.Reflection;

namespace PK.PkUtils.Extensions;

/// <summary>
/// Implements extension methods for ContainerControl class.
/// </summary>
/// <seealso href="http://msdn.microsoft.com/en-us/library/system.windows.forms.containercontrol(v=vs.110).aspx">
/// System.Windows.Forms.ContainerControl class</seealso>
public static class ContainerControlExtensions
{
    /// <summary> A ContainerControl extension method that returns the value of WinForms non-public field
    /// ContainerControl.focusedControl.
    /// </summary>
    /// <remarks>
    /// The field focusedControl caches the value of the Current focused control; apparently for accessing 
    /// the focused child in case there is currently no control actually having the focus.
    /// This method is needed if the calling code somehow wants to "mimic" the behaviour of WinForms code
    /// that uses that field; for instance the validation code.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="ctrl"/> is null. </exception>
    /// <exception cref="InvalidOperationException"> Thrown in case the ContainerControl class
    /// implementation no longer has internal field "focusedControl" of type Control. </exception>
    ///
    /// <param name="ctrl"> The ContainerControl to act on. </param>
    /// <param name="match"> A predicate that specifies matching condition for control. 
    /// May equal to null. </param>
    /// <returns> A resulting Control, or null. </returns>
    /// <seealso href="http://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/ContainerControl.cs.html">
    /// ContainerControl.cs</seealso>
    public static Control InternalFocusedControl(this ContainerControl ctrl, Predicate<Control> match)
    {
        ArgumentNullException.ThrowIfNull(ctrl);
        Control result = FieldsUtils.GetInstanceFieldValueEx<Control>(ctrl, "focusedControl");

        if ((match != null) && (result != null) && !match(result))
            result = null;

        return result;
    }

    /// <summary>
    /// A ContainerControl extension method that searches the hierarchy of controls to get the "most-deep"
    /// focused control.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="ctrl"/> is null. </exception>
    /// <param name="ctrl"> The container control to act on. </param>
    /// <param name="match"> A predicate that specifies matching condition for control. 
    /// May equal to null. </param>
    /// <returns> Found focused control, or null if there is no such thing. </returns>
    public static Control InternalTopFocusedControl(this ContainerControl ctrl, Predicate<Control> match)
    {
        Control last;
        Control result = InternalFocusedControl(ctrl, match);

        for (ContainerControl tempCC = result as ContainerControl; tempCC != null;)
        {
            if ((last = InternalFocusedControl(tempCC, match)) == null)
                break;
            result = last;
            tempCC = result as ContainerControl;
        }

        return result;
    }
}