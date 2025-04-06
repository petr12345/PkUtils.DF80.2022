/***************************************************************************************************************
*
* FILE NAME:   .\SystemEx\SystemEventObserver.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class SystemEventObserver
*
**************************************************************************************************************/


// Ignore Spelling: Utils
//
using System;
using PK.PkUtils.WinApi;


namespace PK.PkUtils.SystemEx;

/// <summary> A generic base class, that defines general "observer" of something happening in Win32,
/// and afterwards raising the related event. <br/>
/// More concrete observers are derived from this one, with usage of specific system hook type. <br/>
/// The details of event are specified by <typeparamref name="T"/> argument. </summary>
///
/// <typeparam name="T"> The type of event arguments of  <see cref="ObservedAction"/> event. </typeparam>
[CLSCompliant(false)]
public abstract class SystemEventObserver<T> : WindowsSystemHookBase where T : EventArgs
{
    #region Fields

    /// <summary>
    /// The event raised when the class "observes something" in the system.
    /// </summary>
    public event EventHandler<T> ObservedAction;
    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor accepting a system hook type as an input argument.
    /// </summary>
    /// <remarks>It is protected, so only the derived class decides which concrete hook type is used.</remarks>
    /// <param name="hook"> A system hook type.</param>
    protected SystemEventObserver(Win32.HookType hook)
      : base(hook)
    { }

    /// <summary>
    /// The constructor accepting a hook type and hook method as input arguments
    /// </summary>
    /// <param name="hook"> A system hook type.</param>
    /// <param name="func">A delegate that will be used as system filter function, after the hook is installed .</param>
    protected SystemEventObserver(Win32.HookType hook, User32.HookProc func)
      : base(hook, func)
    { }
    #endregion // Constructors

    #region Methods

    /// <summary> Raises the event <see cref="ObservedAction"/>. </summary>
    /// <param name="args"> The event arguments. </param>
    protected virtual void RaiseObservedEvent(T args)
    {
        ObservedAction?.Invoke(this, args);
    }
    #endregion // Methods
}
