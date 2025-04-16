/***************************************************************************************************************
*
* FILE NAME:   .\UI\Stack\StackedForm.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class StackedForm
*
**************************************************************************************************************/
///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// MSDN license agreement notice
// 
// This software is a Derivative Work based upon a MSDN article
// "Creating a Multiple Form Application Framework for the Microsoft .NET Compact Framework"
// http://msdn.microsoft.com/en-us/library/aa446546.aspx
//
// The End-user license agreement ("EULA") text is available at
// ..\..\..\_Lic\EULA.rtf
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Ignore Spelling: Utils, Preload
//
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using PK.PkUtils.Reflection;
using PK.PkUtils.UI.General;
using PK.PkUtils.Utils;

namespace PK.PkUtils.UI.Stack;

/// <summary> The abstract class adding to its base class Form the functionality of <see cref="IStackedForm"/>
/// interface, which is needed for usability with FormStack.
/// <see cref="PK.PkUtils.UI.Stack.IStackedForm"/>
/// <see cref="PK.PkUtils.UI.Stack.FormStack"/> </summary>
///
/// <remarks> <para>
/// Note how the event handling chain works:   <br/>
/// ------------------------------------------ <br/>
///    1. Form.OnClosing =&gt;                    <br/>
/// =&gt; 2. fires the Closing event =&gt;           <br/>
/// =&gt; 3. StackedForm.FormClosingHandler =&gt;    <br/>
/// =&gt; 4. StackedForm.ClosingHandler =&gt;        <br/>
/// =&gt; 5. calls FireEventStackItemClosed which fires
///       EventHandler{EventFormStackItemClosedArgs} StackedForm.evStackItemClosed =&gt; <br/>
/// =&gt; 6. invokes subscribed FormStack.On_evStackItemClosed =&gt; <br/>
/// =&gt; 7. calls FormStack.Pop                                 <br/>
/// </para>
/// 
/// <para>
/// The class should be declared as abstract from architecture point of view;
/// however in that case the derived Form could not be designed by WinForms Designer, displaying an
/// error message "The designer must create an instance of type '{type name}', "The designer must
/// create an instance of type '{type name}', but it can't because the type is declared as
/// abstract".
/// </para>
/// <para>
/// So, instead of making the method StackedInitializeComponent as abstract, it is virtual, and is
/// using reflection. The derived class may override that method if needed.<br/>
/// 
/// For more info about this subject, see for instance an MSDN help topic
/// <a href="http://msdn.microsoft.com/en-us/library/2xh82hxd.aspx">
/// The designer must create an instance of type '&lt;type name&gt;', but it can't because the type
/// is declared as abstract</a>
/// </para> </remarks>
[CLSCompliant(true)]
public partial class StackedForm : Form, IStackedForm
{
    #region Fields

    /// <summary>
    /// The backing field for the <see cref="EventPreloadDone"/> property.
    /// </summary>
    protected ManualResetEvent _evPreloadDone;

    /// <summary>
    /// The backing field for the <see cref="evStackItemClosed"/> property.
    /// </summary>
    protected EventHandler<EventFormStackItemClosedArgs> _evStackItemClosed;

    /// <summary> 
    /// Affects the behavior of overwritten DestroyHandle method.
    /// For more detailed explanation, see the comments on CacheModalHandle property.
    /// </summary>
    protected readonly bool _bCacheModalHandle;

    /// <summary> Is true during StackedShowModal() method call. See the property IsModalState</summary>
    private bool _bModal;
    /// <summary> Assigned to true during disposing.</summary>
    private bool _bIsDisposing;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor. All derived classes must call this constructor, or the overloaded one!
    /// </summary>
    protected StackedForm()
    {
        // The DesignMode property itself isn't sufficient - isn't set to true in the constructor.
        if (!this.IsDesignMode())
        {
            InitializeClosingHandler();
        }
    }

    /// <summary>
    /// The constructor providing the value of _bCacheModalHandle read-only field.
    /// </summary>
    /// <param name="bCacheModalHandle">The initial value of property <see cref="CacheModalHandle"/> </param>
    /// <remarks>
    /// Since the read-only field can be initialized only in the constructor of the same class,
    /// one has to provide a protected constructor to give the derived classes the ability to initialize that.
    /// See also http://social.msdn.microsoft.com/Forums/en-US/csharplanguage/thread/4932e0a6-efbb-4e35-bed6-44edbc150884/
    /// </remarks>
    protected StackedForm(bool bCacheModalHandle)
      : this()
    {
        _bCacheModalHandle = bCacheModalHandle;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> 
    /// Get the value of _bCacheModalHandle field. <br/>
    /// If this field is assigned to true by constructor, the method ShowDialog ( ergo StackedShowModal ) 
    /// does NOT destroy the Form handle in its end, as it normally does by calling DestroyHandle.
    /// Thanks to this, the new ShowDialog will display the Form exactly in the same state 
    /// as the last ShowDialog has left it. <br/>
    /// You may need this behaviour in case when re-creating window handles and all child controls handles 
    /// causes complicated initialization, with difficult to achieving the same state of controls.
    /// </summary>
    /// <remarks> 
    /// To make this working, we have overwritten the virtual method DestroyHandle.
    /// The Form handle will be eventually destroyed by the Dispose method.
    /// </remarks>
    public bool CacheModalHandle
    {
        get { return _bCacheModalHandle; }
    }

    /// <summary>
    /// Returns true if the Dispose method is just running. Note this relates just to implementation
    /// of Dispose in this class, but not to any overrides.
    /// </summary>
    /// <returns></returns>
    private bool IsDisposing()
    {
        return _bIsDisposing;
    }
    #endregion // Properties

    #region Methods
    #region Protected Methods

    /// <summary>
    /// Method called by constructor. Subscribes to the closing handler of wrapped form.
    /// </summary>
    protected void InitializeClosingHandler()
    {
        this.Closing += new System.ComponentModel.CancelEventHandler(FormClosingHandler);
    }

    /// <summary>
    /// Fires the event EventFormClosed
    /// </summary>
    /// <param name="args">Argument containing characteristic data for event that is raised when the FormStack item is closed.</param>
    protected void FireEventStackItemClosed(EventFormStackItemClosedArgs args)
    {
        if (null != _evStackItemClosed)
        {
            _evStackItemClosed(this, args);
        }
    }

    /// <summary>
    /// The method called by worker thread ( which is created in LoadData() )
    /// Overload this method if you need to load data specific way,
    /// but you MUST call this base method to set this event.
    /// </summary>
    protected virtual void DataThread()
    {
        EventPreloadDone.Set();
    }

    /// <summary>
    /// The virtual method which is called by the event handler private void FormClosingHandler.
    /// In your derived form, you may overwrite this method
    /// ( and that's what you should do if you want to prevent your form from closing.
    ///  Don't override virtual void OnClosing(CancelEventArgs args);)
    /// </summary>
    /// <param name="sender">The sender (originator) of the event.</param>
    /// <param name="args">Provides data for a cancelable event.</param>
    protected virtual void ClosingHandler(object sender, CancelEventArgs args)
    {
        if (!args.Cancel)
        {
            if (this.PermitsCaching)
            {
                if (!this.IsModalState)
                {
                    this.Visible = false;
                    args.Cancel = true;
                }
                else
                { // For modal forms (dialogs) must do the default processing, to get rid of their modal state.
                  // Note the form will still remain in the cache, 
                  // where it has been added by IStackedForm FormStack.Preload(IStackId id, ..
                }
            }

            // Following in turn invokes the event handler FormStack.On_evStackItemClosed,
            // which pops the form from the stack
            FireEventStackItemClosed(new EventFormStackItemClosedArgs(this));
        }
    }

    /// <summary>
    /// Calls Control.DestroyHandle(), regardless what is the current value of _bCacheModalHandle.
    /// </summary>
    protected virtual void ExplicitDestroyHandle()
    {
        base.DestroyHandle();
    }

    /// <summary>
    /// Overrides the implementation of the base class, in order to preserve window handle if we want to.
    /// See more the comments 
    /// </summary>
    protected override void DestroyHandle()
    {
        if (!CacheModalHandle || IsDisposing())
        {
            base.DestroyHandle();
        }
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios.
    /// If disposing equals true, the method has been called directly or indirectly by a user's code. 
    /// Managed and unmanaged resources can be disposed.
    /// If disposing equals false, the method has been called by the runtime from inside the finalizer
    /// and you should not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. 
    /// Otherwise it is called by finalizer.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                _bIsDisposing = true;
                Disposer.SafeDispose(ref _evPreloadDone);
            }
            base.Dispose(disposing);
        }
        finally
        {
            _bIsDisposing = false;
        }
    }
    #endregion // Protected Methods

    #region Private Methods
    #endregion // Private Methods
    #endregion // Method(s)

    #region IStackedForm members

    /// <inheritdoc/>
    public event EventHandler<EventFormStackItemClosedArgs> evStackItemClosed
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        add { _evStackItemClosed += value; }
        [MethodImpl(MethodImplOptions.Synchronized)]
        remove { _evStackItemClosed -= value; }
    }

    /// <summary>
    /// The vent being set by <see cref="DataThread"/> method called by the worker thread,
    /// when that worker thread is finished. <br/>
    /// The worker thread itself is created in <see cref="LoadData"/> method.
    /// </summary>
    public ManualResetEvent EventPreloadDone
    {
        get
        {
            if (null == _evPreloadDone)
            {
                _evPreloadDone = new ManualResetEvent(false);
            }
            return _evPreloadDone;
        }
    }

    /// <summary>
    /// Property implementing the interface member IStackedForm.FormId.
    /// This is just a very "basic" implementation; derived classes should override it if needed.
    /// </summary>
    public virtual IStackId FormId
    {
        get { return new FormStack.StackId(this.GetType()); }
    }

    /// <inheritdoc/>
    public Form MyForm
    {
        get { return this; }
    }

    /// <inheritdoc/>
    public virtual bool PermitsCaching
    {
        get { return true; }
    }

    /// <inheritdoc/>
    public virtual bool PermitsPostInitialize
    {
        get { return false; }
    }

    /// <inheritdoc/>
    public bool IsModalState
    {
        get { return _bModal; }
    }

    /// <inheritdoc/>
    public bool IsFormVisible
    {
        get { return base.Visible; }
    }

    /// <inheritdoc/>
    public virtual void StackedSetVisible(bool bValue)
    {
        if (bValue)
        {
            /* this.ShowInTaskbar = true;  generally may not be suitable */
        }
        base.Visible = bValue;
    }

    /// <inheritdoc/>
    public virtual DialogResult StackedShowModal()
    {
        DialogResult result = DialogResult.None;

        Debug.Assert(!IsModalState);
        _bModal = true;
        result = base.ShowDialog();
        _bModal = false;
        return result;
    }

    /// <inheritdoc/>
    public virtual void StackedActivate(object extraArg)
    {
        // activation itself
        /* this.ShowInTaskbar = true;  generally may not be suitable */
        base.Visible = true;
        base.Activate();
    }

    /// <summary>
    /// This method is called from FormStack.Prelod in case the form supports loading the data 
    /// on a separate thread ( the "data thread").
    /// The implementation in this call just calls InitializeComponent through reflection.
    /// It is a virtual method, so the derived class (Form) can overwrite.
    /// </summary>
    public virtual void StackedInitializeComponent()
    {
        this.RunInstanceMethod("InitializeComponent", null);
    }

    /// <summary> Loads the Form initial data; it may start a separate thread to allow the Form to
    /// appear sooner before that loading is complete. </summary>
    ///
    /// <remarks> A worker thread loading the data should call the <see cref="DataThread"/> virtual
    /// method for its startup. </remarks>
    ///
    /// <param name="bThreadStarted"> An output argument; a value becomes true if the method
    /// implementation has started a separate thread loading the data. </param>
    public virtual void LoadData(out bool bThreadStarted)
    {
        bThreadStarted = false;
        // in derived class which would create a separate thread do following:
        /* 
        Thread workerThread = new Thread(new ThreadStart(DataThread));

        // set out synchronization object
        EventPreloadDone.Reset();

        // start the thread
        workerThread.Start();
        bThreadStarted = true;
        */
    }

    /// <summary>
    /// Implements IStackedForm.Populate().
    /// You should overload this method if you need to populate controls with data
    /// retrieved in the "data thread".
    /// </summary>
    /// <returns>True on success, false on failure. </returns>
    public virtual bool Populate()
    {
        return true;
    }
    #endregion // IStackedForm members

    #region Event handlers

    /// <summary>
    /// The handler that subscribed to event this.Closing 
    /// </summary>
    /// <param name="sender">The sender (originator) of the event.</param>
    /// <param name="args">Provides data for a cancelable event.</param>
    private void FormClosingHandler(object sender, CancelEventArgs args)
    {
        ClosingHandler(sender, args);
    }
    #endregion // Event handlers
}
