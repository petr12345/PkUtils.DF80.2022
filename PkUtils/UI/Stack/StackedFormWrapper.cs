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
using PK.PkUtils.Utils;

namespace PK.PkUtils.UI.Stack;

/// <summary>
/// The wrapper around any Form class, providing IStackedForm functionality
/// to that form. <br/>
/// When using the FormStack stack, any pushed item must implement IStackedForm 
/// interface. The base class StackedForm implements that interface,
/// but you may not be able to derive your Form from StackedForm.
/// In that case, normally you would have to re-implement whole IStackedForm 
/// interface functionality in your new Form class, derived for that purpose.
/// The StackedFormWrapper helps with that issue, by wrapping the form, 
/// so instead of declaring an extra class and coding
/// <code>
/// class MyTFormStacked : MyTForm, IStackedForm
/// {
///   // mandatory implementation of all IStackedForm members
/// }
/// FormStack fStack = ...;
/// fStack.Push(typeof(MyTFormStacked), args);
/// </code>
/// you can just call 
/// <code>
/// <![CDATA[
/// fStack.Push(typeof(StackedFormWrapper<MyTForm>), args);
/// ]]>
/// </code>
/// </summary>
/// <typeparam name="TForm">The type of the Form which you want to 'wrap' in FormStack</typeparam>
/// 
/// <remarks>
/// <para>
/// Note how the event handling chain works:    <br/>
/// ------------------------------------------  <br/>
///    1. Form.OnFormClosing =>                 <br/>
/// => 2. fires the Closing event =>            <br/>
/// => 3. StackedForm.FormClosingHandler =>     <br/>
/// => 4. StackedForm.ClosingHandler =>         <br/>
/// => 5. calls FireEventStackItemClosed which fires 
///       EventHandler{EventFormStackItemClosedArgs} StackedForm.evStackItemClosed => <br/>
/// => 6. invokes subscribed FormStack.On_evStackItemClosed =>                        <br/>
/// => 7. calls FormStack.Pop                   <br/>
/// <br/></para>
/// </remarks>
[CLSCompliant(true)]
public class StackedFormWrapper<TForm> : IStackedForm, IDisposable where TForm : Form
{
    #region Fields
    /// <summary>
    /// The Form being created by the constructor.
    /// </summary>
    protected TForm _form;

    /// <summary>
    ///  The backing field of the property <see cref="EventPreloadDone"/>
    /// </summary>
    protected ManualResetEvent _eventPreloadDone;

    /// <summary>
    /// The backing field of the property <see cref="EventStackItemClosed"/>
    /// </summary>
    protected EventHandler<EventFormStackItemClosedArgs> _eventStackItemClosed;

    /// <summary>
    /// The backing field of the property <see cref="IsModalState"/>
    /// </summary>
    private bool _bModal;
    #endregion  // Fields

    #region Constructor(s)

    /// <summary>
    /// Classic argument-less constructor, creates the Form by calling Activator.CreateInstance with no additional arguments.
    /// </summary>
    public StackedFormWrapper()
    {
        _form = (TForm)Activator.CreateInstance(FormType);
        InitializeClosingHandler();
    }

    /// <summary>
    /// Constructor used for creation the Form through Activator
    /// </summary>
    /// <param name="arg">Additional argument provided to Activator.CreateInstance for Form creation.</param>
    public StackedFormWrapper(object arg)
    {
        object[] args = [arg];

        _form = (TForm)Activator.CreateInstance(FormType, args);

        InitializeClosingHandler();
    }

    /// <summary>
    /// Constructor used for creation the Form through Activator
    /// </summary>
    /// <param name="args">Additional arguments provided to Activator.CreateInstance for Form creation.</param>
    public StackedFormWrapper(params object[] args)
    {
        _form = (TForm)Activator.CreateInstance(FormType, args);

        InitializeClosingHandler();
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// The type of the Form which you want to 'wrap' in FormStack.
    /// </summary>
    protected virtual Type FormType
    {
        get { return typeof(TForm); }
    }

    /// <summary>
    /// The Form that has been created by the constructor.
    /// </summary>
    public TForm MyTForm
    {
        get { return _form; }
    }
    #endregion  // Fields

    #region Methods

    /// <summary>
    /// Fires the event <see cref="EventStackItemClosed"/>
    /// </summary>
    /// <param name="args">Argument containing characteristic data for event that is raised when the FormStack item is closed.</param>
    protected void FireEventStackItemClosed(EventFormStackItemClosedArgs args)
    {
        _eventStackItemClosed?.Invoke(this, args);
    }

    /// <summary>
    /// Method called by constructor. Subscribes to the closing handler of wrapped form.
    /// </summary>
    protected void InitializeClosingHandler()
    {
        MyTForm.FormClosing += new FormClosingEventHandler(FormClosingHandler);
    }

    /// <summary>
    /// The virtual method which is called by the event handler private void FormClosingHandler.
    /// In your derived form, you may overwrite this method
    /// ( and that's what you should do if you want to prevent your form from closing.
    ///  Don't overwrite virtual void OnFormClosing(FormClosingEventArgs args);)
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
                    MyTForm.Visible = false;
                    args.Cancel = true;
                }
            }

            // Following in turn invokes the event handler FormStack.On_evStackItemClosed,
            // which pops the form from the top of the stack
            FireEventStackItemClosed(new EventFormStackItemClosedArgs(this));
        }
    }

    /// <summary>
    /// The event handler called by the MyTForm.Closing event.
    /// Delegates the functionality to the virtual method void ClosingHandler
    /// </summary>
    /// <param name="sender">The sender (originator) of the event.</param>
    /// <param name="args">Provides data for a cancelable event.</param>
    private void FormClosingHandler(object sender, CancelEventArgs args)
    {
        ClosingHandler(sender, args);
    }
    #endregion  // Methods

    #region IStackedForm Members

    /// <inheritdoc/>
    public event EventHandler<EventFormStackItemClosedArgs> EventStackItemClosed
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        add
        {
            _eventStackItemClosed += value;
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        remove
        {
            _eventStackItemClosed -= value;
        }
    }

    /// <inheritdoc/>
    public System.Threading.ManualResetEvent EventPreloadDone
    {
        get
        {
            if (null == _eventPreloadDone)
            {
                _eventPreloadDone = new ManualResetEvent(false);
            }
            return _eventPreloadDone;
        }
    }

    /// <inheritdoc/>
    public virtual IStackId FormId
    {
        get { return new FormStack.StackId(this.GetType()); }
    }

    /// <inheritdoc/>
    public Form MyForm
    {
        get { return MyTForm; }
    }

    /// <inheritdoc/>
    public bool PermitsCaching
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
        get { return (null != MyTForm) && MyTForm.Visible; }
    }

    /// <inheritdoc/>
    public void StackedSetVisible(bool bValue)
    {
        if (null != MyTForm)
        {
            MyTForm.Visible = bValue;
        }
    }

    /// <inheritdoc/>
    public System.Windows.Forms.DialogResult StackedShowModal()
    {
        DialogResult result = DialogResult.None;

        Debug.Assert(!IsModalState);
        if (null != MyTForm)
        {
            _bModal = true;
            result = MyTForm.ShowDialog();
            _bModal = false;
        }
        return result;
    }

    /// <inheritdoc/>
    public virtual void StackedActivate(object extraArg)
    {
        if (null != MyTForm)
        {
            // activation itself
            /* this.ShowInTaskbar = true;  generally may not be suitable */
            MyTForm.Visible = true;
            MyTForm.Activate();
        }
    }

    /// <inheritdoc/>
    public virtual void StackedInitializeComponent()
    {
        // should do nothing, as long as PermitsPostInitialize returns false
    }

    /// <inheritdoc/>
    public virtual void LoadData(out bool bThreadStarted)
    {
        bThreadStarted = false;
    }

    /// <inheritdoc/>
    public virtual bool Populate()
    {
        return true;
    }
    #endregion // IStackedForm Members

    #region IDisposable Members

    /// <summary>
    /// Implements IDisposable.
    /// Do not make this method virtual.
    /// A derived class should not be able to override this method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        // Take yourself off the Finalization queue to prevent finalization code 
        // for this object from executing a second time.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios.
    /// If disposing equals true, the method has been called directly
    /// or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed.
    /// If disposing equals false, the method has been called by the 
    /// runtime from inside the finalizer and you should not reference 
    /// other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. 
    /// Otherwise it is called by finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Disposer.SafeDispose(ref _form);
        }
    }
    #endregion
}

/// <summary>
/// The wrapper around the main application Form.
/// Overrides the virtual method ClosingHandler, 
/// which must just return immediately.
/// </summary>
/// <typeparam name="TForm">The type of the Form which you want to 'wrap' in FormStack</typeparam>
[CLSCompliant(true)]
public class MainStackedFormWrapper<TForm> : StackedFormWrapper<TForm> where TForm : Form
{
    #region Constructor(s)

    /// <summary>
    /// Classic argument-less constructor
    /// </summary>
    public MainStackedFormWrapper()
      : base()
    {
        InitializeClosedHandler();
    }

    /// <summary>
    /// Constructor used for creation through Activator
    /// </summary>
    /// <param name="arg">Additional argument provided to Activator.CreateInstance for Form creation.</param>
    public MainStackedFormWrapper(object arg)
      : base(arg)
    {
        InitializeClosedHandler();
    }

    /// <summary>
    /// Constructor used for creation through Activator
    /// </summary>
    /// <param name="args">Additional arguments provided to Activator.CreateInstance for Form creation.</param>
    public MainStackedFormWrapper(object[] args)
      : base(args)
    {
        InitializeClosedHandler();
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>
    /// Initialization code called by constructor. Subscribes to the event MyTForm.FormClosed
    /// </summary>
    protected void InitializeClosedHandler()
    {
        MyTForm.FormClosed += new FormClosedEventHandler(MyFor_FormClosed);
    }

    /// <summary>
    /// The event handler called by the event MyTForm.FormClosed.
    /// Its code invokes the event handler FormStack.On_evStackItemClosed,
    /// which pops the form from the stack.
    /// Unlike with StackedFormWrapper, that action cannot be done in ClosingHandler,
    /// so it has been moved here.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args"> A FormClosedEventArgs that contains the event data.</param>
    protected void MyFor_FormClosed(object sender, FormClosedEventArgs args)
    {
        // Following in turn invokes the event handler FormStack.On_evStackItemClosed,
        // which pops the form from the stack.
        // Unlike with StackedFormWrapper, it cannot be done in ClosingHandler
        FireEventStackItemClosed(new EventFormStackItemClosedArgs(this));
    }

    /// <summary>
    /// Overrides the behavior of the base class.
    /// Does nothing. Since this is the main form, let someone else handle that event
    /// </summary>
    /// <param name="sender">The sender (originator) of the event.</param>
    /// <param name="args">Provides data for a cancelable event.</param>
    protected override void ClosingHandler(object sender, CancelEventArgs args)
    {
    }
    #endregion // Methods
}
