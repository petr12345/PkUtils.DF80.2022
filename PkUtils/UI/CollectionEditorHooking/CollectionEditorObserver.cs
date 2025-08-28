// Ignore Spelling: Utils, Dict, prestruct, hwnd, validator
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PK.PkUtils.Reflection;
using PK.PkUtils.SystemEx;
using PK.PkUtils.UI.CollectionEditorExtending;
using PK.PkUtils.WinApi;
using CollectionFormAction = PK.PkUtils.UI.CollectionEditorHooking.CollectionFormActionEventArgs.CollectionFormAction;


namespace PK.PkUtils.UI.CollectionEditorHooking;

/// <summary>
/// CollectionEditorObserver is a system-level hook using Win32 (WH_CALLWNDPROCRET). 
/// The primary goal of CollectionEditorObserver is to facilitate validation and customization of
/// collections edited within CollectionForm instances, offering an alternative approach to achieving
/// validation in the PropertyGrid context.
/// It does not function as a traditional collection editor like <see cref="CustomCollectionEditor{T}"/>,
/// but serves the same purpose, by intercepting events related to the creation
/// and activation of CollectionForm instances.
/// </summary>
/// 
/// <typeparam name="T"> 
/// The type of elements contained in the collections edited by CollectionForm instances.
/// This type parameter specifies the element type of the collections managed by the observer.
/// </typeparam>
/// 
/// <seealso href="https://learn.microsoft.com/en-us/windows/win32/winmsg/about-hooks">
/// Hooks Overview</seealso>
public class CollectionEditorObserver<T> : SystemEventObserver<CollectionFormActionEventArgs>
{
    #region Events

    /// <summary>
    /// Declares an event to expose the inner PropertyGrid's PropertyValueChanged event.
    /// </summary>
    public event EventHandler<PropertyValueChangedEventArgs> PropertyValueChanged;

    /// <summary>
    /// Declares a public event to expose the inner CollectionForm FormClosed event.
    /// </summary>
    public event EventHandler<FormClosedEventArgs> PropertyEditorFormClosed;
    #endregion // Events

    #region Fields

    private readonly ICollectionValidator<T> _validator;

    /// <summary> The Dictionary of currently created collection form hooks. </summary>
    private readonly Dictionary<IntPtr, CollectionFormHook<T>> _dictEditorForms = [];

    /// <summary> Thread safety locker. </summary>
    private readonly object _locker = new();
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default constructor. If <paramref name="install"/> is true, installs a system hook WH_CALLWNDPROCRET,
    /// using specific filter function.
    /// </summary>
    /// <param name="validator"> (Optional) The validator of collection of <typeparamref name="T"/>. </param>
    /// <param name="install"> (Optional) True to install the hook immediately. </param>
    public CollectionEditorObserver(
        ICollectionValidator<T> validator = null,
        bool install = true)
        : base(Win32.HookType.WH_CALLWNDPROCRET)
    {
        _validator = validator;
        _filterFunc = new User32.HookProc(FnCallWndProcHook);
        if (install)
        {
            Install();
        }
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets collection validator used for items validation. 
    /// You may override this property in derived collection editor.
    /// </summary>
    public virtual ICollectionValidator<T> CollectionValidator
    {
        get => _validator;
    }

    /// <summary> Get the dictionary of collection form hooks. </summary>
    protected IDictionary<IntPtr, CollectionFormHook<T>> DictOfCollectionFormHooks
    {
        get => _dictEditorForms;
    }

    /// <summary> Sync object. </summary>
    protected object Locker { get => _locker; }
    #endregion // Properties

    #region Methods
    #region Protected Methods
    #region Infrastructure

    /// <summary> Adds a new window to internal dictionary, creating its EditorInfo value. </summary>
    /// <remarks>
    /// Derived class could overwrite this method in case it wants as as a value in the dictionary use a class
    /// derived from EditorInfo.
    /// </remarks>
    /// <param name="collectionForm"> The instance of CollectionForm. </param>
    /// <returns> True if new key has been added, false otherwise. </returns>
    protected virtual bool AddNewCollectionForm(Form collectionForm)
    {
        ArgumentNullException.ThrowIfNull(collectionForm);

        IntPtr hwnd = collectionForm.Handle;
        bool result;

        lock (Locker)
        {
            // does not contain that key yet, some new window is being created
            if (result = !DictOfCollectionFormHooks.ContainsKey(hwnd))
            {
                DictOfCollectionFormHooks.Add(hwnd, new CollectionFormHook<T>(hwnd, this));
            }
        }
        return result;
    }

    /// <summary> Updates the CollectionFormHook related to given collection form handle.<br/> </summary>
    /// <remarks>
    /// This could not be done when handling its Win32.WM_CREATE, but only later when handling its
    /// Win32.WM.WM_ACTIVATE message.
    /// </remarks>
    /// <param name="hwnd"> The window handle. </param>
    /// <returns> True if valid handle from the dictionary has been provided, false otherwise. </returns>
    protected virtual bool UpdateCollectionFormHook(IntPtr hwnd)
    {
        bool result;

        lock (Locker)
        {
            if (result = DictOfCollectionFormHooks.TryGetValue(hwnd, out CollectionFormHook<T> info))
            {
                HookCollectionForm(info);
            }
        }

        return result;
    }

    /// <summary>
    /// Making an opposite action to <see cref="AddNewCollectionForm"/> - removing a window handle from internal
    /// dictionary,.
    /// </summary>
    /// <remarks>   Derived class could overwrite this method in case any additional functionality needed. </remarks>
    /// <param name="hwnd"> The window handle. </param>
    /// <returns> True if valid handle from the dictionary has been provided, false otherwise. </returns>
    protected virtual bool RemoveCollectionForm(IntPtr hwnd)
    {
        lock (Locker)
        {
            return DictOfCollectionFormHooks.Remove(hwnd);
        }
    }
    #endregion // Infrastructure

    #region Hooking_the_Form

    /// <summary> Query if 'collectionForm' is collection editor form the <typeparamref name="T"/>. </summary>
    /// <param name="collectionForm"> The collection form. Can't be null. </param>
    /// <returns> True if collection editor form for type T, false if not. </returns>
    protected virtual bool IsCollectionEditorFormForTargetType(Form collectionForm)
    {
        ArgumentNullException.ThrowIfNull(collectionForm);
        Type validatorType = typeof(T);

        Type[] newItemTypes = collectionForm.GetInstancePropertyValueEx<Type[]>("NewItemTypes");
        Type collectionFormEditedType = newItemTypes?.FirstOrDefault();
        bool result = ((collectionFormEditedType != null) && validatorType.IsAssignableFrom(collectionFormEditedType));

        return result;
    }

    /// <summary> Hook the collection form, plus the OK button. </summary>
    /// <param name="hook"> The hook to which the functionality is delegated. Can't be null. </param>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected virtual bool HookCollectionForm(CollectionFormHook<T> hook)
    {
        ArgumentNullException.ThrowIfNull(hook);
        bool result = hook.IsCollectionFormHooked;

        if (!result)
        {
            if (result = hook.HookCollectionForm())
            {
                result = hook.HookOkButton();
            }
        }

        return result;
    }

    /// <summary> Unhook the collection form, plus the OK button. </summary>
    /// <param name="hook"> The hook to which the functionality is delegated. Can't be null. </param>
    protected virtual void UnhookCollectionForm(CollectionFormHook<T> hook)
    {
        ArgumentNullException.ThrowIfNull(hook);

        // unsubscribe in an opposite order
        hook.UnhookOkButton();
        hook.UnhookCollectionForm();
    }
    #endregion // Hooking_the_Form

    #region The_Rest

    /// <summary> Raise the event <see cref="PropertyValueChanged"/>. </summary>
    /// <param name="sender"> Source of the event. </param>
    /// <param name="args"> Event information to send to registered event handlers. </param>
    protected internal void RaisePropertyGrid_ValueChanged(object sender, PropertyValueChangedEventArgs args)
    {
        PropertyValueChanged?.Invoke(sender, args);
    }

    /// <summary> Raise the event <see cref="PropertyEditorFormClosed"/>. </summary>
    /// <param name="sender"> Source of the event. </param>
    /// <param name="args"> Event information to send to registered event handlers. </param>
    protected internal void RaiseCollectionForm_FormClosed(object sender, FormClosedEventArgs args)
    {
        PropertyEditorFormClosed?.Invoke(sender, args);
    }

    /// <summary>
    /// Raises the event ObservedAction with given action value. Delegates the call to the overloaded method.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    ///
    /// <param name="action">   A value describing in more detail the current action of the system. </param>
    /// <param name="hwnd"> A window handle of related window. </param>
    protected void RaiseEventCollectionEditorFormAction(
        CollectionFormActionEventArgs.CollectionFormAction action,
        IntPtr hwnd)
    {
        lock (Locker)
        {
            if (DictOfCollectionFormHooks.TryGetValue(hwnd, out CollectionFormHook<T> info))
            {
                RaiseObservedEvent(new CollectionFormActionEventArgs(action, info));
            }
            else
            {
                string errorMessage = string.Format(CultureInfo.InvariantCulture, "Invalid handle = 0x{0:x8}", hwnd);
                throw new ArgumentException(errorMessage, nameof(hwnd));
            }
        }
    }

    /// <summary> Validates the collection form items. </summary>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected virtual bool ValidateCollectionFormItems()
    {
        return true;
    }
    #endregion // The_Rest
    #endregion // Protected Methods

    #region Private Methods

    /// <summary> The callback for the system hook WH_CALLWNDPROCRET. </summary>
    ///
    /// <param name="code"> If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROCRET hooks will not receive hook notifications and may behave incorrectly as a result. If the hook procedure does not call CallNextHookEx, the return value should be zero.. </param>
    /// <param name="wParam"> Specifies whether the message is sent by the current process. If the message is sent
    /// by the current process, it is nonzero; otherwise, it is zero. </param>
    /// <param name="lParam">  A pointer to a CWPRETSTRUCT structure that contains details about the message. </param>
    ///
    /// <returns> A result of User32.CallNextHookEx. </returns>
    private IntPtr FnCallWndProcHook(int code, IntPtr wParam, IntPtr lParam)
    {
        IntPtr nResult;
        WindowsSystemHookBase callWndProc_hook = this;

        if (code < 0 || code == Win32.HC_NOREMOVE)
        {
            // For these values, must limit itself to just calling next hook in chain
            nResult = callWndProc_hook.CallNextHook(code, wParam, lParam);
        }
        else
        {
            IntPtr hwnd;
            Win32.CWPRETSTRUCT prestruct;

            nResult = callWndProc_hook.CallNextHook(code, wParam, lParam);
            prestruct = (Win32.CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32.CWPRETSTRUCT));
            hwnd = prestruct.hwnd;

            switch (prestruct.message)
            {
                case (int)Win32.WM.WM_CREATE:
                    if (CollectionEditorUtils.IsCollectionEditorForm(hwnd, out Form form))
                    {
                        if (IsCollectionEditorFormForTargetType(form))
                        {
                            lock (Locker)
                            {
                                if (AddNewCollectionForm(form))
                                {
                                    RaiseEventCollectionEditorFormAction(CollectionFormActionEventArgs.CollectionFormAction.Creating, hwnd);
                                }
                            }
                        }
                    }
                    break;

                case (int)Win32.WM.WM_ACTIVATE:  // yup, someone is being activated
                    lock (Locker)
                    {
                        if (UpdateCollectionFormHook(hwnd))
                        {
                            RaiseEventCollectionEditorFormAction(CollectionFormAction.Activating, hwnd);
                        }
                    }
                    break;

                case (int)Win32.WM.WM_DESTROY:
                    lock (Locker)
                    {
                        if (DictOfCollectionFormHooks.TryGetValue(hwnd, out CollectionFormHook<T> hook))
                        {
                            RaiseEventCollectionEditorFormAction(CollectionFormAction.Destroying, hwnd);
                            if (RemoveCollectionForm(hwnd))
                            {
                                UnhookCollectionForm(hook);
                            }
                        }
                    }
                    break;
            }
        }

        return nResult;
    }
    #endregion // Private Methods
    #endregion // Methods
}
