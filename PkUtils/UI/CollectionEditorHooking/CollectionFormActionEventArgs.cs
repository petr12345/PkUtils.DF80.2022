// Ignore Spelling: Utils, Dict
//
using System;
using PK.PkUtils.Extensions;

#pragma warning disable IDE0290 // Use primary constructor

namespace PK.PkUtils.UI.CollectionEditorHooking;

/// <summary>
/// The event arguments for event that is raised when the CollectionEditorObserver "observes something"
/// What really happened ( window creating, activation or destroying)
/// is specified by EventMsgBoxActionEventArgs.CurrentAction property value.
/// </summary>
public class CollectionFormActionEventArgs : EventArgs
{
    /// <summary>
    /// The enum is used as a property of EventMsgBoxActionEventArgs
    /// expressing what is "going on".
    /// </summary>
    public enum CollectionFormAction
    {
        /// <summary> Used when window is being created </summary>
        Creating = 0,

        /// <summary> Used when window is being activated </summary>
        Activating = 1,

        /// <summary> Used when window is being destroyed </summary>
        Destroying = 2,
    }

    /// <summary> The constructor. </summary>
    /// <param name="action"> A value describing in more detail the current action of the system. </param>
    /// <param name="info"> An information about related window. </param>
    public CollectionFormActionEventArgs(CollectionFormAction action, CollectionFormInfo info)
      : base()
    {
        CurrentAction = action;
        Info = info;
    }

    /// <summary> The current action as set by the constructor </summary>
    public CollectionFormAction CurrentAction { get; protected set; }

    /// <summary> The window information, as set by the constructor </summary>
    public CollectionFormInfo Info { get; protected set; }

    /// <summary> Returns the window handle. </summary>
    public IntPtr Hwnd { get { return Info.Hwnd; } }

    /// <summary> Returns a string that represents the current object. </summary>
    /// <returns> A string that represents the current object. </returns>
    public override string ToString()
    {
        string[] items = [
            $"{nameof(CurrentAction)}: {CurrentAction}",
            $"{nameof(Info.Title)}: {Info.Title}",
        ];

        return $"{GetType().Name}({items.Join()})";
    }
}
#pragma warning restore IDE0290 // Use primary constructor