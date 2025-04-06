/***************************************************************************************************************
*
* FILE NAME:   .\Extensions\Control.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION:
*   The file contains extension class ControlExtension
*
**************************************************************************************************************/

// Ignore Spelling: Utils, ctrl
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace PK.PkUtils.Extensions;

/// <summary>
/// Implements extension methods for Control
/// </summary>
/// <seealso href="http://msdn.microsoft.com/en-us/library/system.windows.forms.control(v=vs.110).aspx">
/// System.Windows.Forms.Control class</seealso>
public static class ControlExtension
{
    #region Non-generic controls search

    /// <summary> Give me all the child controls of given control, recursively </summary>
    /// 
    /// <param name="parent">The parent control whose child controls we want to return.
    /// Can't be null. </param>
    /// 
    /// <returns> Resulting sequence of Controls that can be iterated. </returns>
    public static IEnumerable<Control> AllControls(this Control parent)
    {
        ArgumentNullException.ThrowIfNull(parent);

        foreach (Control control in parent.Controls)
        {
            yield return control;
        }
        foreach (Control control in parent.Controls)
        {
            foreach (Control cc in AllControls(control)) yield return cc;
        }
    }
    #endregion  // Non-generic controls search

    #region Generic controls search

    /// <summary>
    /// Give me all the child controls of given control <paramref name="parent"/>, 
    /// which has given type <typeparamref name="C"/>. 
    /// The search is recursive if the argument <paramref name="recursive"/> is true.
    /// </summary>
    /// <typeparam name="C"> Required type of controls the caller is interested in.
    /// </typeparam>
    /// <param name="parent">The parent control whose child controls we want to return.
    /// Can't be null. </param>
    /// <param name="recursive"> If true (by default) the search is recursive. 
    ///                          Otherwise, only direct child controls are searched. </param> 
    /// <returns> Resulting sequence of Controls that can be iterated. </returns>
    public static IEnumerable<C> AllControls<C>(this Control parent, bool recursive = true)
        where C : Control
    {
        ArgumentNullException.ThrowIfNull(parent);

        return parent.AllControls(x => x is C, recursive).Cast<C>();
    }

    /// <summary> Give me all the child controls of given control, recursively. </summary>
    ///
    /// <param name="parent">The parent control whose child controls we want to return.
    /// Can't be null. </param>
    /// <param name="condition"> The condition to match. Can't be null. </param>
    /// <param name="recursive"> If true (by default) the search is recursive. 
    ///                          Otherwise, only direct child controls are searched. </param>
    /// <returns> Resulting sequence of Controls that can be iterated. </returns>
    public static IEnumerable<Control> AllControls(
        this Control parent,
        Predicate<Control> condition,
        bool recursive = true)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(condition);

        foreach (Control control in parent.Controls)
        {
            if (condition(control))
            {
                yield return control;
            }
        }

        if (recursive)
        {
            foreach (Control control in parent.Controls)
            {
                foreach (Control cc in control.AllControls().Where(cc => condition(cc)))
                {
                    yield return cc;
                }
            }
        }
    }

    /// <summary>
    /// Give me the first child controls of given control <paramref name="parent"/>, 
    /// which has given type <typeparamref name="C"/>.
    /// The search is recursive if the argument <paramref name="recursive"/> is true.
    /// </summary>
    /// <typeparam name="C">
    /// Required type of controls the caller is interested in.
    /// </typeparam>
    /// <param name="parent">The parent control whose child control we 
    /// want to return.</param>
    /// <param name="recursive">If true (by default) the search is recursive.
    /// Otherwise, only direct child controls are searched. </param>
    /// <returns>A firs child control of given type <typeparamref name="C"/>,
    /// or null. </returns>
    public static C FindControl<C>(this Control parent, bool recursive)
        where C : Control
    {
        return AllControls<C>(parent, recursive).FirstOrDefault();
    }

    /// <summary>
    /// Searches for the first parent control of given type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"> Type of searched parent. </typeparam>
    /// <param name="ctrl"> The WinForms control whose parent should be returned. </param>
    /// <returns> The found parent control or null. 
    ///  Returns null if provided <paramref name="ctrl"/> is null. </returns>
    public static T FindParentControl<T>(this Control ctrl) where T : Control
    {
        T result = null;

        if (ctrl != null)
        {
            for (Control ctrlParent = ctrl.Parent; ctrlParent != null;)
            {
                if ((result = ctrlParent as T) != null)
                    break;
                else
                    ctrlParent = ctrlParent.Parent;
            }
        }

        return result;
    }
    #endregion // Generic controls search

    #region Disposal-checks

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> in case the object is null.
    /// Throws <see cref="ObjectDisposedException"/> in case the object is not null, 
    /// but its IsDisposed property returns true.
    /// </summary>
    ///
    /// <remarks> The argument <paramref name="objectName"/> is used as a exception argument. </remarks>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when a supplied object is null. </exception>
    /// <exception cref="ObjectDisposedException"> Thrown when a supplied object has been disposed. </exception>
    ///
    /// <param name="ctrl">  The WinForms control being checked. </param>
    /// <param name="objectName"> The control name (usually the argument or variable name
    /// the object has in the calling code). Will be used for the exception message creation.
    /// If null or empty, the control name is used instead.
    /// </param>
    public static void CheckNotDisposed(
        this Control ctrl,
        [CallerArgumentExpression(nameof(ctrl))] string objectName = null)
    {
        ArgumentNullException.ThrowIfNull(ctrl, objectName);

        if (ctrl.IsDisposed)
        {
            // combine both the control name and its type information
            if (string.IsNullOrEmpty(objectName))
            {
                string compoName = string.IsNullOrEmpty(ctrl.Name) ? "<noname>" : ctrl.Name;
                string typeName = ctrl.GetType().FullName;

                objectName = string.Format(CultureInfo.InvariantCulture,
                  "{0}     [{1}]", compoName, typeName);
            }
            throw new ObjectDisposedException(objectName);
        }
    }
    #endregion // Disposal-checks
}