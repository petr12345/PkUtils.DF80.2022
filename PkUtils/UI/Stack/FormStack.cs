/***************************************************************************************************************
*
* FILE NAME:   .\UI\Stack\FormStack.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class FormStack 
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


// Ignore Spelling: Bts, Preload, Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Reflection;

namespace PK.PkUtils.UI.Stack;

/// <summary> The class supporting "Multiple Form Application Framework". <br/>
/// The idea is based on the MSDN article <a href="http://msdn.microsoft.com/en-us/library/aa446546.aspx">
/// "Creating a Multiple Form Application Framework for the Microsoft .NET Compact Framework"</a>,
/// but the implementation here fixed several bugs (primarily wrong thread synchronization), and
/// involved other improvements. </summary>
[CLSCompliant(true)]
public class FormStack : List<IStackedForm>, IDisposable, ISuspendable, ICompactable
{
    #region Typedefs

    /// <summary>
    /// Identifier of the stacked item, implementing IStackId interface. All functionality for
    /// recognizing the key ( GetHashCode and Equals methods )
    /// relies on the wrapped type of Form. Hence, just one instance of such Form type could be cached.
    /// </summary>
    ///
    /// <remarks>
    /// In case you need more granularity and to keep two Forms of the same type ( but with somehow
    /// different details) in the cache, you may use derived classes ( keys )
    /// and overwrite their GetHashCode and Equals methods. The related Form will return the value of
    /// such key in its IStackedForm.FormId property. See more details about this in the comments for
    /// interface IStackId.
    /// </remarks>
    public class StackId : IStackId
    {
        #region Fields
        /// <summary> Type of the Form. </summary>
        protected readonly Type _formType;
        #endregion // Fields

        #region Constructor(s)

        /// <summary> Constructs a new instance of StackId, with a given underlying Form type. </summary>
        ///
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="formType"/> is null.</exception>
        /// <exception cref="ArgumentException"> Thrown when <paramref name="formType"/> argument does not
        ///   support the <see cref="IStackedForm"/> interface. </exception>
        ///
        /// <param name="formType"> The type of represented Form. </param>
        public StackId(Type formType)
        {
            ArgumentNullException.ThrowIfNull(formType);

            if (!typeof(IStackedForm).IsAssignableFrom(formType))
            {
                string strErr = string.Format(CultureInfo.InvariantCulture,
                  "The type {0} does not support interface {1}", formType, typeof(IStackedForm));
                throw new ArgumentException(strErr);
            }
            _formType = formType;
        }
        #endregion // Constructor(s)

        #region Methods

        /// <summary>
        /// Overwrites (implements) the virtual method of the base class. Delegates the functionality to
        /// IEquatable{IStackId} method. </summary>
        ///
        /// <param name="obj">The object to compare with the current object. </param>
        ///
        /// <returns> true if the objects are considered equal, false if they are not. </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as IStackId);
        }

        /// <summary>
        /// Overwrites (implements) the virtual method of the base class. Returns the hash code of this StackId.
        /// </summary>
        ///
        /// <returns> A hash code for this object. </returns>
        public override int GetHashCode()
        {
            return this.FormType.GetHashCode();
        }

        /// <summary>
        /// Overwrites (implements) the virtual method of the base class. 
        /// Returns the <see cref="FormType"/> converted to string, without the namespace.
        /// </summary>
        ///
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            string strTemp = this.FormType.TypeToReadable();

            return string.Format(CultureInfo.InvariantCulture, "<{0}>", strTemp);
        }
        #endregion // Methods

        #region Static Operators

        /// <summary> Converts a Form Type to a StackId. </summary>
        ///
        /// <param name="t"> The Type to process. </param>
        /// <returns> A new StackId constructed with <paramref name="t"/> argument.</returns>
        public static explicit operator StackId(Type t)
        {
            return new StackId(t);
        }

        /// <summary> Converts a StackId to a Form Type. </summary>
        ///
        /// <param name="id"> The <see cref="StackId"/> identifier. </param>
        ///
        /// <returns> For a non-null <paramref name="id"/> returns a id.FormType; otherwise null. </returns>
        public static explicit operator Type(StackId id)
        {
            Type result;

            if (id == null)
                result = null;
            else
                result = id.FormType;
            return result;
        }
        #endregion // Static Operators

        #region implementation of IStackId
        #region implementation of IEquatable<IStackId>

        /// <summary>
        /// Overwrites (implements) the virtual method of the base class. Delegates the functionality to
        /// IEquatable{IStackId} method.
        /// </summary>
        ///
        /// <param name="other">  The stack identifier to compare to this object. </param>
        ///
        /// <returns> true if the objects are considered equal, false if they are not. </returns>
        public virtual bool Equals(IStackId other)
        {
            bool result = false;

            if (other is null)
            {
                /* result = false; already is */
            }
            else if (object.ReferenceEquals(other, this))
            {
                result = true;
            }
            else if (object.ReferenceEquals(other.GetType(), typeof(StackId)))
            {   // the other object type is directly StackId, just compare values
                result = this.FormType.Equals(other.FormType);
            }
            else
            {   // the other object is somehow derived, let him decide
                result = other.Equals((object)this);
            }
            return result;
        }
        #endregion // implementation of IEquatable<IStackId>

        /// <summary> Gets the type of the Form. </summary>
        ///
        /// <value> The type of the Form. </value>
        public Type FormType
        {
            get { return this._formType; }
        }
        #endregion // implementation of IStackId
    }

    /// <summary> The event arguments for event that is raised when a Form is activated. </summary>
    public class EventFormActivatedEventArgs : EventFormStackItemActionArgs
    {
        /// <summary> The constructor. </summary>
        ///
        /// <param name="iForm"> The related Form implementing
        /// <see cref="PK.PkUtils.UI.Stack.IStackedForm "/> interface. </param>
        public EventFormActivatedEventArgs(IStackedForm iForm)
          : base(iForm)
        {
        }
    }

    /// <summary>
    /// The event arguments for event that is raised when a Form is going to modal state.
    /// </summary>
    public class EventFormGoingModalEventArgs : EventFormStackItemActionArgs
    {
        /// <summary> The constructor. </summary>
        /// <param name="iForm"> The related Form implementing
        /// <see cref="PK.PkUtils.UI.Stack.IStackedForm "/> interface. </param>
        public EventFormGoingModalEventArgs(IStackedForm iForm)
          : base(iForm)
        {
        }
    }

    /// <summary>
    /// Describes the execution state of the FormStack
    /// </summary>
    public enum ExecState
    {
        /// <summary>
        /// The case the FormStack is not running
        /// </summary>
        NotRunning = 0,

        /// <summary>
        /// Represents the kind of execution through public virtual void Run(),
        /// which calls periodically Application.DoEvents(); 
        /// </summary>
        RunsOwnMsgPump = 1,

        /// <summary>
        /// Represents the kind of execution through public virtual void Run(),
        /// which just calls Application.Run().
        /// </summary>
        RunsAppRun = 2
    }
    #endregion // Typedefs

    #region Fields

    /// <summary> The event raised when the Form is activated or going to modal state. </summary>
    public event EventHandler<EventFormStackItemActionArgs> evFormAction;

    /// <summary>
    /// The stack of descriptions of actually displayed Forms ( do not confuse with the entire
    /// collection of Forms, which keeps both visible and invisible (just cached) Forms).
    /// </summary>
    protected List<IStackId> _TypeStack = [];

    /// <summary> Stop request variable. See IsStopRequest, StopRequest() </summary>
    protected bool _bStopRequest;

    /// <summary> Thread safety locker object. </summary>
    private readonly object _locker = new();

    /// <summary> The count of suspend requests. </summary>
    private int _nSuspended;

    /// <summary>
    /// The boolean flag indication whether the Forms below the top of stack should hide.
    /// </summary>
    private bool _bHideBts;

    /// <summary>
    /// The current execution state
    /// </summary>
    private ExecState _execState;

#if DEBUG
    /// <summary> A match specifying the warning top does not. </summary>
    private const string _strWarnTopDoesNotMatch = "Does the top Form properly implement OnClosing, including calling the base.OnClosing ?";
#endif // DEBUG
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default argument-less constructor. </summary>
    public FormStack()
      : this(false)
    {
    }

    /// <summary> The constructor. </summary>
    ///
    /// <param name="bHideBts"> The boolean flag indication whether the Forms below the top of stack
    ///   should hide. </param>
    public FormStack(bool bHideBts)
    {
        _bHideBts = bHideBts;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Get <see cref="IStackId"/> of the Form currently on the top of displayed Forms stack. </summary>
    ///
    /// <value> The top stack item. </value>
    public IStackId TopItem
    {
        get
        {
            lock (Locker)
            {
                return TypeStack.LastOrDefault();
            }
        }
    }

    /// <summary>
    /// Return the current top Form, based on the information from property TopItem.
    /// </summary>
    ///
    /// <value> The top stacked Form. </value>
    /// <seealso cref="TopItem"/>
    public IStackedForm TopStackedForm
    {
        get
        {
            return TopItem.NullSafe(id => FindForm(id));
        }
    }

    /// <summary>
    /// Get the top Form object, if there is any. Internally delegates the functionality to
    /// TopStackedForm property.
    /// </summary>
    ///
    /// <value> The top Form. </value>
    public Form TopForm
    {
        get
        {
            return TopStackedForm.NullSafe(iform => iform.MyForm);
        }
    }

    /// <summary> Returns true if there is actually displayed Forms. </summary>
    ///
    /// <value> true if a stack is empty, false if not. </value>
    public bool IsStackEmpty
    {
        get { return (0 == StackDepth); }
    }

    /// <summary> Returns count of actually displayed Forms. </summary>
    ///
    /// <value> The depth of the stack. </value>
    public int StackDepth
    {
        get { return TypeStack.Count; }
    }

    /// <summary>
    /// The current execution state
    /// </summary>
    public ExecState ExecutionState
    {
        get { return _execState; }
        protected set { _execState = value; }
    }


    /// <summary>
    /// Get the list of all cached Forms. Despite the class FormStack is directly derived from the List
    /// of StackForms, for better abstraction the code should use the property FormList; i.e. always
    /// call the property FormList.Something instead of just this.Something.
    /// </summary>
    ///
    /// <value> A List of Forms. </value>
    protected IList<IStackedForm> FormList
    {
        get { return this; }
    }

    /// <summary>
    /// The stack of descriptions of actually displayed Forms ( do not confuse with the entire
    /// collection of Forms ).
    /// </summary>
    ///
    /// <value> A Stack of stack Ids. </value>
    protected IList<IStackId> TypeStack
    {
        get { Debug.Assert(null != _TypeStack); return _TypeStack; }
    }

    /// <summary> Get the list of Forms that are currently cached but not displayed. </summary>
    ///
    /// <value> The Forms that are cached but not displayed. </value>
    /// 
    /// <remarks>
    /// The implementation must call .ToList() to avoid delayed execution, and completely populate the
    /// returned list before further processing, which could involved calls like FormList.Remove etc.
    /// </remarks>
    protected IList<IStackedForm> NotDisplayedForms
    {
        get { lock (Locker) { return FormList.Where(sf => !_TypeStack.Contains(sf.FormId)).ToList(); } }
    }

    /// <summary> Has someone requested stop ( called StopRequest() ) </summary>
    ///
    /// <value> true if someone has called <see cref="StopRequest"/>, false if not. </value>
    protected bool IsStopRequest
    {
        get { return _bStopRequest; }
    }

    /// <summary> Returns the current value of boolean flag _bHideBts. </summary>
    ///
    /// <value> true if we should hide below stack, false if not. </value>
    protected bool ShouldHideBelowStack
    {
        get { return _bHideBts; }
        set { _bHideBts = value; }
    }

    /// <summary> Sync object. </summary>
    ///
    /// <value> The locker. </value>
    protected object Locker
    {
        get { return _locker; }
    }
    #endregion // Properties

    #region Methods

    #region Public Methods

    #region Form-pushing

    #region Non-modal-invoking

    // Note: 
    // To reduce the list of overloads below, I could get rid of all overloads with the Type argument
    //   Push(Type tFormType, ...
    // if only I sacrifice the code generality, and instead of overloads Push(IStackId id, ...
    // the code implements just methods requiring directly the StackId argument 
    //   Push(StackId id, ...).
    // Besides, to enable to call such method with the actual argument just Type 
    // also requires changing the current StackId static explicit operator to the implicit one
    // ( from: public static explicit operator StackId(Type t)
    //   to: public static implicit operator StackId(Type t) )
    // 
    // Unfortunately, I cannot write such implicit operator for IStackId interface,
    // because of a language limitation on interfaces.

    /// <summary> Push the Form of given type to the top of the stack. </summary>
    ///
    /// <remarks>
    /// If the Form with StackId representing this Form type is not cached yet, this overload will
    /// invoke argumnent-less constructor of the Form. </remarks>
    ///
    /// <param name="tFormType"> The type of the Form. </param>
    public void Push(Type tFormType)
    {
        Push(new StackId(tFormType));
    }

    /// <summary>
    /// Push the Form of given type to the top of the stack, initialize that with extra argument. </summary>
    ///
    /// <remarks>
    /// If the Form with StackId representing this Form type is not cached yet, this overload will
    /// invoke constructor with single argument of matching type. </remarks>
    ///
    /// <param name="tFormType"> The type of the Form. </param>
    /// <param name="extraArg">  An extra argument that will be used for new Form creation. </param>
    public void Push(Type tFormType, object extraArg)
    {
        Push(new StackId(tFormType), extraArg);
    }

    /// <summary>
    /// Push the Form of given type to the top of the stack, initialize that with extra arguments. </summary>
    ///
    /// <remarks>
    /// If the Form with StackId representing this Form type is not cached yet, this overload will
    /// invoke constructor with arguments of matching count and matching types. </remarks>
    ///
    /// <param name="tFormType">  The type of the Form. </param>
    /// <param name="args"> Extra arguments that will be used for new Form creation.</param>
    public void Push(Type tFormType, params object[] args)
    {
        Push(new StackId(tFormType), args);
    }

    /// <summary> Push the Form of the type specified by given IStackId to the top of the stack. </summary>
    ///
    /// <remarks>
    /// If the Form with StackId representing this Form type is not cached yet, this overload will
    /// invoke argumnent-less constructor of the Form. </remarks>
    /// 
    /// <param name="id"> The <see cref="StackId"/> identifier. </param>
    public void Push(IStackId id)
    {
        DoPush(false, id, null, null);
    }

    /// <summary>
    /// Push the Form of given IStackId to the top of the stack, initialize that with extra argument.
    /// </summary>
    /// 
    /// <remarks>
    /// If the Form with StackId representing this Form type is not cached yet, this overload will
    /// invoke constructor with single argument of matching type. </remarks>
    ///
    /// <param name="id"> The Form Id </param>
    /// <param name="extraArg"> An extra argument that will be used for new Form creation. </param>
    public void Push(IStackId id, object extraArg)
    {
        DoPush(false, id, null, [extraArg]);
    }

    /// <summary>
    /// Push the Form of given IStackId to the top of the stack, and initialize that with extra
    /// arguments. </summary>
    ///
    /// <remarks>
    /// If the Form with StackId representing this Form type is not cached yet, this overload will
    /// invoke constructor with arguments of matching count and matching types. </remarks>
    ///
    /// <param name="id"> The Form Id </param>
    /// <param name="args"> Extra arguments that will be used for new Form creation.</param>
    public void Push(IStackId id, params object[] args)
    {
        DoPush(false, id, null, args);
    }
    #endregion // Non-modal-invoking

    #region Modal-invoking

    /// <summary>
    /// Push the Form of given IStackId to the top of the stack, and display as modal. </summary>
    ///
    /// <remarks>
    /// Note: if the StackedShowModal is implemented by calling ShowModal, the Form by default does NOT
    /// preserve the position and size upon the second call. Rather than trying to deal with such
    /// restoring here, I think the better option is to handle this by a specialized Form code ( the
    /// could used FormLayoutPersister ). </remarks>
    ///
    /// <param name="tFormType"> The type of the Form. </param>
    ///
    /// <returns> The DialogResult returned from StackedShowModal. </returns>
    public DialogResult PushModal(Type tFormType)
    {
        return PushModal(new StackId(tFormType));
    }

    /// <summary>
    /// Push the Form of given Form type to the top of the stack, and display as modal. The Form will
    /// be initialized by args. </summary>
    ///
    /// <remarks>
    /// See the remark for the most-simple overloaded PushModal regarding size/position persistence. </remarks>
    ///
    /// <param name="tFormType">  The type of the Form. </param>
    /// <param name="args"> Extra arguments that will be used for new Form creation.</param>
    ///
    /// <returns> The DialogResult returned from StackedShowModal. </returns>
    public DialogResult PushModal(Type tFormType, params object[] args)
    {
        return PushModal(new StackId(tFormType), args);
    }

    /// <summary>
    /// Push the Form of given Form type to the top of the stack, and display as modal. The Form will
    /// be initialized by <paramref name="args"/>, with the exact constructor being selected with the
    /// help of supplied argument types array <paramref name="argTypes"/>. <br/>
    /// This may be needed in case that one or more argument values are null, hence the exact argument
    /// type cannot be determined from the value itself. For more details, see the code of involved
    /// class ActivatorEx. </summary>
    ///
    /// <remarks>
    /// See the remark for the most-simple overloaded PushModal regarding size/position persistence. </remarks>
    ///
    /// <param name="tFormType">  The type of the Form. </param>
    /// <param name="argTypes">  Supplied types of arguments <paramref name="args"/></param>
    /// <param name="args"> Extra arguments that will be used for new Form creation.</param>
    ///
    /// <returns> The DialogResult returned from StackedShowModal. </returns>
    public DialogResult PushModal(Type tFormType, Type[] argTypes, params object[] args)
    {
        return PushModal(new StackId(tFormType), argTypes, args);
    }

    /// <summary>
    /// Push the Form of given IStackId to the top of the stack, and display as modal. </summary>
    ///
    /// <remarks>
    /// See the remark for the most-simple overloaded PushModal regarding size/position persistence. </remarks>
    ///
    /// <param name="id"> The Form Id </param>
    ///
    /// <returns> The DialogResult returned from StackedShowModal. </returns>
    public DialogResult PushModal(IStackId id)
    {
        return DoPush(true, id, null, null);
    }

    /// <summary>
    /// Push the Form of given IStackId to the top of the stack, and display as modal. The Form will be
    /// initialized by <paramref name="args"/> </summary>
    ///
    /// <remarks>
    /// See the remark for the most-simple overloaded PushModal regarding size/position persistence. </remarks>
    ///
    /// <param name="id">   The Form Id. </param>
    /// <param name="args"> Extra arguments that will be used for new Form creation.</param>
    ///
    /// <returns> The DialogResult returned from StackedShowModal. </returns>
    public DialogResult PushModal(IStackId id, params object[] args)
    {
        return DoPush(true, id, null, args);
    }

    /// <summary>
    /// Push the Form of given IStackId to the top of the stack, and display as modal. The Form will be
    /// initialized by args, with the exact constructor being selected with the help of supplied
    /// argument types array argTypes. This may be needed in case that one or more argument values are
    /// null, hence the exact argument type cannot be determined from the value itself. For more
    /// details, see the code of involved class ActivatorEx. </summary>
    ///
    /// <remarks>
    /// See the remark for the most-simple overloaded PushModal regarding size/position persistence. </remarks>
    ///
    /// <param name="id">       The Form Id. </param>
    /// <param name="argTypes">  Supplied types of arguments <paramref name="args"/></param>
    /// <param name="args">     Extra arguments that will be used for new Form creation. </param>
    ///
    /// <returns> The DialogResult returned from StackedShowModal. </returns>
    public DialogResult PushModal(IStackId id, Type[] argTypes, params object[] args)
    {
        return DoPush(true, id, argTypes, args);
    }
    #endregion // Modal-invoking
    #endregion // Form-pushing

    #region Form-popping

    /// <summary>
    /// Pop the given amount of Forms from the top of the stack. If the input argument
    /// <paramref name="nFormsToPop"/> actually required to pop the complete stack,
    /// the method calls <see cref="Stop"/>. </summary>
    ///
    /// <exception cref="ArgumentException"> Thrown when the argument <paramref name="nFormsToPop"/>
    ///   requires to pop more Forms than available in the stack. </exception>
    ///
    /// <param name="nFormsToPop"> An amount of Forms that should be removed the top of Form stack. </param>
    public void Pop(int nFormsToPop)
    {
        lock (Locker)
        {
            int nStackDepth;
            IStackedForm iNewTop;
            Form fNewTop;

            if (nFormsToPop < 0)
            {
                throw new ArgumentException(
                  string.Format(CultureInfo.InvariantCulture, "The value of nFormsToPop = '{0}' is negative", nFormsToPop),
                  nameof(nFormsToPop));
            }
            else if (nFormsToPop > (nStackDepth = StackDepth))
            {
                throw new ArgumentException(
                  string.Format(CultureInfo.InvariantCulture, "nFormsToPop = {0}, nStackDepth = {1}. You cannot Pop more Forms than available in the stack!", nFormsToPop, nStackDepth),
                  nameof(nFormsToPop));
            }
            else if (nFormsToPop == nStackDepth)
            {   // handling the case popping of all Forms
                Stop();
            }
            else
            {
                Debug.Assert(nFormsToPop > 0);
                // Remove from stack, but NOT from the cache;
                // loop backwards to prevent automatic adjustment to mess us up
                for (int i = 0; i < nFormsToPop; i++)
                {
                    _TypeStack.RemoveAt(--nStackDepth);
                }

                // find the last Form in the stack and make it visible and active
                if (null != (iNewTop = TopStackedForm))
                {
                    if (null != (fNewTop = iNewTop.MyForm))
                    {
                        fNewTop.Enabled = true;
                    }
                    // Activation needed for case the popped Form was in 
                    // a modal state. Otherwise, on NETCF for some reason 
                    // the Form does mot repaint itself.
                    // Note: Anyway, making the Form visible by assigning 
                    // .Visible =  true
                    // must be a part of its activation.

                    /* sf.FormVisible = true; */
                    iNewTop.StackedActivate(null);
                    RaiseEventFormAction(new EventFormActivatedEventArgs(iNewTop));
                }
                else
                {
                    Debug.Fail("Could not find the top Form");
                }
            }
        }
    }

    /// <summary>
    /// If the input argument <paramref name="id"/> equals to current top Form on the stack, pops this
    /// single Form from the stack. Otherwise has no effect ( just the debug version asserts a bug ). </summary>
    ///
    /// <param name="id">  The Form Id. </param>
    ///
    /// <returns> true if succeeds, false if fails. </returns>
    public bool Pop(IStackedForm id)
    {
        bool result;

        if (result = (id != null) && object.ReferenceEquals(id, TopStackedForm))
        {
            Pop(1);
        }
        else
        {   // should not happen
            Debug.Fail("The argument value must match the top Form");
        }
        return result;
    }

    /// <summary>
    /// Attempts to pop a single item from the stack, and returns that item. Returns null if there was
    /// nothing on the stack; </summary>
    ///
    /// <returns> The Fprm id <see cref="StackId"/> identifier.</returns>
    public IStackedForm Pop1()
    {
        IStackedForm top;
        IStackedForm result = null;

        if (null != (top = TopStackedForm))
        {
            if (Pop(top))
            {
                result = top;
            }
        }
        return result;
    }
    #endregion // Form-popping

    #region Other-public-methods

    /// <summary>
    /// <para>
    /// Runs the User Interface, periodically processing all Windows messages currently in the message
    /// queue. <br/>
    /// The processing stops temporary, if <see cref="Suspend"/> has been called;
    /// and stops for good if <see cref="StopRequest"/> has been called. 
    /// </para>
    /// <para>
    /// The other way how to run the User Interface is the method <see cref="RunApplication"/>,
    /// which just delegates to <see cref="Application.Run()"/>.
    /// </para>
    /// </summary>
    ///
    /// <remarks>
    /// In case you observe "LoaderLock was detected" MDA assert here, you may change Visual Studio
    /// debugging settings, in Debug/Exceptions/Managed Debugging Assistants/Loader Lock and switch off
    /// checkbox for 'Thrown'. 
    /// </remarks>
    /// <seealso cref="RunApplication"/>
    public virtual void Run()
    {
        this.ExecutionState = ExecState.RunsOwnMsgPump;
        for (; ; )
        {
            if (!this.IsSuspended)
            {
                if (IsStopRequest)
                {
                    Stop();
                    break;
                }
                else if (FormList.Count == 0)
                {
                    break;
                }
                else
                {
                    Application.DoEvents();
                }
            }
            else
            {
                System.Threading.Thread.Sleep(64);
            }
        }
        this.ExecutionState = ExecState.NotRunning;
    }

    /// <summary> An alternate way how to run the User Interface. <br/>
    /// Unlike the method <see cref="Run"/>, this method just delegates all functionality to
    /// Application.Run(). Consequently, methods <see cref="StopRequest()"/> and
    /// <see cref="Suspend()"/> have no effect in this scenario. </summary>
    ///
    /// <seealso cref="Run"/>
    public virtual void RunApplication()
    {
        Application.Idle += new EventHandler(Application_Idle);

        try
        {
            this.ExecutionState = ExecState.RunsAppRun;
            Application.Run();
        }
        finally
        {
            Application.Idle -= Application_Idle;
            this.ExecutionState = ExecState.NotRunning;
        }
    }

    /// <summary> Set the stop request to true. </summary>
    ///
    /// <remarks>
    /// You can stop this engine actual by two ways: i/ just by calling the Stop method will delete all
    /// Forms;
    ///    hence the message loop will recognize it no longer has any Forms and should stop processing
    ///    events
    /// ii/ by assigning boolean stop request to true, by StopRequest();
    ///     in this case it is the message loop who will stop processing the message loop, and
    ///     afterwards will delete all Forms
    ///  The second approach is actually better, as its stops message processing asap. </remarks>
    public void StopRequest()
    {
        _bStopRequest = true;
    }

    /// <summary> Stop the UI; get rid of all Forms in the cache. </summary>
    /// <seealso cref="Run"/>
    /// <seealso cref="RunApplication"/>
    public void Stop()
    {
        var prevState = this.ExecutionState;

        RemoveContents();
        if (prevState == ExecState.RunsAppRun)
        {
            Application.Exit();
        }
        this.ExecutionState = ExecState.NotRunning;
    }

    /// <summary>
    /// Suspends message queue processing and removes all contents ( cached Forms and types stack ), by
    /// calling <see cref="RemoveContents"/>. Next, depending on the argument <paramref name="bRevive"/>, it
    /// leave the Form stack either in suspended or revived (running) state. </summary>
    /// 
    /// <param name="bRevive"> If false, the Form Stack is left in suspended state after leaving this
    ///   method. If true, the method calls <see cref="Revive"/> </param>
    public void Reset(bool bRevive)
    {
        this.Suspend();
        this.RemoveContents();
        if (bRevive)
        {
            this.Revive();
        }
    }

    /// <summary>
    /// Removing and disposing all such Forms which are not referenced in the current Form types stack
    /// ( hence they do not appear just after either single "Pop" command
    ///  or after more consequent 'Pop' steps. ) <br/>
    /// 
    ///  The supplied predicate might be null. In case it is not null, the Form also has to match the
    ///  predicate to be removed from the stack and disposed. </summary>
    ///
    /// <param name="match"> If not null, specifies an additional criteria the removed Forms must
    ///   match. </param>
    public void RemoveUnusedForms(Predicate<IStackedForm> match)
    {
        lock (Locker)
        {
            IDisposable iDisp;
            // Prepare the list of 'candidates' for removal ( present in FormList but not in the stack )
            // Must use .ToList() to avoid delayed execution and completely populate the list before further calls of FormList.Remove(sf);
            List<IStackedForm> listToRemove = NotDisplayedForms.ToList();

            if (null != match)
            {
                listToRemove = listToRemove.FindAll(match);
                /* or listToRemove = listToRemove.Where(sf => match(sf)).ToList(); */
            }

            // now actually remove no longer used Forms
            foreach (IStackedForm sf in listToRemove)
            {
                if (!sf.IsModalState)
                {
                    FormList.Remove(sf);
                    if (null != (iDisp = sf as IDisposable))
                    {
                        iDisp.Dispose();
                    }
                }
                else
                {   // This should not happen - all Forms in modal state should remain in the _TypeStack stack
                    // during compacting, hence they should not be contained in listToRemove.
                    Debug.Fail("Possible error in usage of RemoveUnusedForms logic");
                }
            }
        }
    }

    /// <summary>
    /// "Aggressive" compact - removing unused Forms that match the given <paramref name="match"/>
    /// predicate, and compacting the rest of Forms. </summary>
    ///
    /// <param name="match"> If not null, specifies the criteria the removed Forms must match. </param>
    /// <seealso cref="FriendlyCompact"/>
    public void AggressiveCompact(Predicate<IStackedForm> match)
    {
        lock (Locker)
        {
            RemoveUnusedForms(match);
            // if there is anything left, compact that
            foreach (ICompactable iComp in FormList.OfType<ICompactable>())
            {
                iComp.Compact();
            }
        }
    }

    /// <summary>
    /// Less-aggressive compacting method. The method compacts only cached Forms supporting
    /// ICompactable; as it compacts existing Forms without deleting them. It does not remove any
    /// cached Forms.
    /// </summary>
    ///
    /// <seealso cref="AggressiveCompact"/>
    public void FriendlyCompact()
    {
        lock (Locker)
        {
            foreach (ICompactable iComp in FormList.OfType<ICompactable>())
            {
                iComp.Compact();
            }
        }
    }

    /// <summary>
    /// Returns an existing IStackedForm Form of given id, if such Form exists, of null of there is no
    /// such Form cached in the Form list. </summary>
    /// <param name="id"> The <see cref="StackId"/> identifier. </param>
    /// <returns> Existing Form identifier <see cref="IStackedForm "/> or null. </returns>
    /// <exception cref="System.ArgumentNullException"> Thrown when the input argument
    ///   <paramref name="id"/> is null. </exception>
    public IStackedForm FindForm(IStackId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        lock (Locker)
        {
            return FormList.FirstOrDefault(sf => sf.FormId.Equals(id));
        }
    }

    /// <summary> Find the Form of a given Type <paramref name="tFormType"/>in the stack. </summary>
    ///
    /// <param name="tFormType">  The type of the Form. </param>
    ///
    /// <returns> Existing Form identifier <see cref="IStackedForm "/> or null. </returns>
    ///
    /// <exception cref="System.ArgumentNullException"> Thrown when the input argument
    ///   <paramref name="tFormType"/> is null. </exception>
    public IStackedForm FindForm(Type tFormType)
    {
        return FindForm(new StackId(tFormType));
    }

    /// <summary> Overrides ( implements ) the virtual method of the base class. </summary>
    ///
    /// <returns> A string that represents this object. </returns>
    public override string ToString()
    {
        StringBuilder sbMessage = new();

        sbMessage.AppendFormat(
          CultureInfo.InvariantCulture,
          "There are {0} Form instances cached.{1}",
          FormList.Count, Environment.NewLine);

        sbMessage.AppendFormat(
          CultureInfo.InvariantCulture,
          "Stacked types ({0}):",
          TypeStack.Count);

        for (int ii = TypeStack.Count - 1; ii >= 0; ii--)
        {
            sbMessage.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}",
              Environment.NewLine, TypeStack[ii]);
        }

        return sbMessage.ToString();
    }
    #endregion // Other-public-methods
    #endregion // Public Methods

    #region Protected Methods

    #region Non-virtual Utility Methods

    /// <summary>
    /// Utility method called by CreateFormInstance, for case when the required Form type being created
    /// is actually a wrapper around a Form - either a <see cref="PK.PkUtils.UI.Stack.StackedFormWrapper{TForm}"/> or
    /// <see cref="PK.PkUtils.UI.Stack.MainStackedFormWrapper{TForm}"/> or any type derived from these types. </summary>
    ///
    /// <param name="fType">    Required type of the wrapper. </param>
    /// <param name="argTypes"> Supplied types of arguments <paramref name="args"/> </param>
    /// <param name="args">     Extra arguments that will be used for new Form creation. </param>
    ///
    /// <returns> The new wrapper around a Form instance. </returns>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="fType"/> is null. </exception>
    protected static IStackedForm CreateWrappedFormInstance(
      Type fType, Type[] argTypes, object[] args)
    {
        ArgumentNullException.ThrowIfNull(fType);

        object[] argSubstitute = null;
        Type[] argTypesSubstritute = null;
        object objRes = null;

        if (null != argTypes)
        {
            if (1 == argTypes.Length)
            {
                argSubstitute = args;
                argTypesSubstritute = [typeof(object)];
            }
            else
            {
                argSubstitute = new object[] { args };
                argTypesSubstritute = [typeof(object[])];
            }
        }

        // Must call IsSubclassOfRawGeneric, since simple methods like IsAssignableFrom etc. 
        // do not work in this case.
        // For more info how to analyze generic types with reflection see       
        // http://msdn.microsoft.com/en-us/library/b8ytshk6.aspx
        // 
        if ((null == objRes) && (fType.IsSubclassOfRawGeneric(typeof(MainStackedFormWrapper<>))))
        {
            objRes = ActivatorEx.CreateInstance(fType, argTypesSubstritute, argSubstitute);
        }

        if ((null == objRes) && (fType.IsSubclassOfRawGeneric(typeof(StackedFormWrapper<>))))
        {
            objRes = ActivatorEx.CreateInstance(fType, argTypesSubstritute, argSubstitute);
        }
        else
        {
            string strErr = string.Format(CultureInfo.InvariantCulture, "This type '{0}' is not supported", fType);
            throw new ArgumentException(strErr, nameof(fType));
        }

        return objRes as IStackedForm;
    }

    /// <summary>
    /// Implements the ultimate Form-pushing functionality. All the Push... methods calls eventually go
    /// here. </summary>
    ///
    /// <param name="bModal">   True if the new Form should be created in a modal state, false
    ///   otherwise. </param>
    /// <param name="id">       The identifier of the Form on the stack. </param>
    /// <param name="argTypes"> Supplied types of arguments <paramref name="args"/> </param>
    /// <param name="args">     Extra arguments that will be used for new Form creation. </param>
    ///
    /// <returns>
    /// If <paramref name="bModal"/> is true, returns the DialogResult returned from
    /// <see cref="IStackedForm.StackedShowModal"/>. otherwise ( for non-modal forms) returns
    /// DialogResult.None. </returns>
    protected DialogResult DoPush(
        bool bModal, IStackId id, Type[] argTypes, object[] args)
    {
        ArgumentNullException.ThrowIfNull(id);

        DialogResult result = DialogResult.None;

        // only allow one Push at a time to maintain cache and stack integrity
        lock (Locker)
        {
            IStackedForm iNewTop;
            IStackedForm iOldTop = this.TopStackedForm;
            bool bStarting = IsStackEmpty;
            bool bFound = false;

            if (null != (iNewTop = FindForm(id)))
            { // nothing specific to do at the moment
                bFound = true;
            }
            else
            {   // the Form wasn't cached, so create it
                if (null == (iNewTop = CreateAndPreload(id, argTypes, args)))
                {   // premature return because of some error
                    return DialogResult.None;
                }
                // add a close event handler
                iNewTop.evStackItemClosed += new EventHandler<EventFormStackItemClosedArgs>(On_evStackItemClosed);
            }

            // Add it to the stack of displayed Forms.
            // Note: Must add to _TypeStack BEFORE setting visible.
            // The order is important, because event handlers invoked by 
            // setting visible may need the correct value returned by property TopStackedForm.
            TypeStack.Add(id);

            if (bModal)
            {  // now show modal
                RaiseEventFormAction(new EventFormGoingModalEventArgs(iNewTop));
                result = iNewTop.StackedShowModal();
                if (!iNewTop.PermitsCaching)
                {
                    Debug.Assert(!iNewTop.IsModalState);
                    RemoveUnusedForms(item => object.ReferenceEquals(item, iNewTop));
                }
                RaiseEventFormAction(new EventFormActivatedEventArgs(this.TopStackedForm));
            }
            else
            {
                bool bEquals = object.Equals(iOldTop, iNewTop);

                // additional prologue for desktop
                if (!bEquals && (null != iOldTop) && (null != iOldTop.MyForm))
                {
                    iOldTop.MyForm.Enabled = false;
                }

                // simple change
                if (bFound || bStarting)
                    iNewTop.StackedActivate(args);
                else
                    iNewTop.StackedSetVisible(true);

                if (!bEquals && (null != iOldTop) && ShouldHideBelowStack)
                {
                    iOldTop.StackedSetVisible(false);
                }

                this.RaiseEventFormAction(new EventFormActivatedEventArgs(iNewTop));
            }
        }
        return result;
    }

    /// <summary>
    /// Raises the event evFormAction, with provided <see cref="EventFormStackItemActionArgs "/>. </summary>
    ///
    /// <param name="args"> The event data generated by the event. </param>
    protected void RaiseEventFormAction(EventFormStackItemActionArgs args)
    {
        evFormAction?.Invoke(this, args);
    }

    /// <summary>
    /// Creating a new Form StackForm of a given type, with optionally other type of its child controls,
    /// and loading its data ( by LoadData() ), and initializing its controls  ( calling Populate() ).
    /// </summary>
    ///
    /// <remarks>
    /// After creating the Form by calling virtual method <see cref="CreateFormInstance"/>,
    /// delegates its further functionality to overload virtual <see cref="Preload"/>,
    /// </remarks>
    ///
    /// <param name="id">       The identifier of the Form on the stack. </param>
    /// <param name="argTypes"> Supplied types of arguments <paramref name="args"/> </param>
    /// <param name="args">     Extra arguments that will be used for new Form creation. </param>
    ///
    /// <returns> Resulting <see cref="IStackedForm "/> on success, or null on failure.</returns>
    protected IStackedForm CreateAndPreload(IStackId id, Type[] argTypes, object[] args)
    {
        IDisposable iDisp;
        IStackedForm iForm;

        lock (Locker)
        {
            if (null != (iForm = this.CreateFormInstance(id, argTypes, args)))
            {
                if (Preload(iForm))
                {  // add it to the cache
                    FormList.Add(iForm);
                }
                else
                {   // get rid of that
                    if (null != (iDisp = iForm as IDisposable))
                    {
                        iDisp.Dispose();
                    }
                    iForm = null;
                }
            }
        }

        return iForm;
    }

    /// <summary> Remove all cached Forms and cleans the Form stack. </summary>
    protected void RemoveContents()
    {
        lock (Locker)
        {
            // nicely destroy each Form
            foreach (IDisposable iDisp in FormList.OfType<IDisposable>())
            {
                iDisp.Dispose();
            }

            // clear the list to kill the message pump in Run()
            FormList.Clear();
            TypeStack.Clear();
        }
    }
    #endregion // Non-virtual Utility Methods

    #region Virtual Methods

    /// <summary>
    /// Create the instance of IStackedForm, given its types ( specified by IStackId )
    /// and extra initialize arguments extraArg. </summary>
    ///
    /// <param name="id">       The Form identifier. </param>
    /// <param name="argTypes"> Supplied types of arguments <paramref name="args"/> </param>
    /// <param name="args">     Extra arguments that will be used for new Form creation. </param>
    ///
    /// <returns> The new Form instance. </returns>
    protected virtual IStackedForm CreateFormInstance(IStackId id, Type[] argTypes, object[] args)
    {
        Type fType = id.FormType;
        int nArgs = (args == null) ? 0 : args.Length;
        IStackedForm iForm = null;
#if DEBUG
        string strFormType = fType.TypeToReadable();
        Trace.WriteLine(string.Format(CultureInfo.InvariantCulture,
          "Creating the Form {0}", strFormType));
#endif // DEBUG

        // Handle separately the case of generics wrappers StackedFormWrapper and MainStackedFormWrapper,
        // since their constructor cannot be determined by set arguments going to the wrapped Forms.
        // If following code is not present, eventually ActivatorEx.CreateInstance
        // throws the MissingMethodException.
        if (fType.IsGenericType && !fType.ContainsGenericParameters && (nArgs > 1))
        {
            // do the MainStackedFormWrapper first, since it derives from StackedFormWrapper
            if ((null == iForm) && (fType.IsSubclassOfRawGeneric(typeof(MainStackedFormWrapper<>))))
            {
                iForm = CreateWrappedFormInstance(fType, argTypes, args);
            }

            if ((null == iForm) && (fType.IsSubclassOfRawGeneric(typeof(StackedFormWrapper<>))))
            {
                iForm = CreateWrappedFormInstance(fType, argTypes, args);
            }
        }

        if (null == iForm)
        {
            if (nArgs == 0)
                iForm = (IStackedForm)Activator.CreateInstance(fType);
            else if (argTypes == null)
                iForm = (IStackedForm)ActivatorEx.CreateInstance(fType, args);
            else
                iForm = (IStackedForm)ActivatorEx.CreateInstance(fType, argTypes, args);
        }

        return iForm;
    }

    /// <summary> Preloads already created Form, that has not been initialized yet. </summary>
    ///
    /// <param name="iForm"> The related Form implementing
    ///   <see cref="PK.PkUtils.UI.Stack.IStackedForm "/> interface. </param>
    ///
    /// <returns> True on success, false on failure. </returns>
    protected virtual bool Preload(IStackedForm iForm)
    {
        bool result;

        // get data on a separate thread
        iForm.LoadData(out bool bThreadStarted);

        // build the Form
        if (iForm.PermitsPostInitialize)
        {
            iForm.StackedInitializeComponent();
        }

        if (bThreadStarted)
        {   // wait for the data thread to finish
            iForm.EventPreloadDone.WaitOne();
        }

        // now populate the controls with any retrieved data
        result = iForm.Populate();

        return result;
    }
    #endregion // Virtual Methods

    #endregion // Protected Methods

    #region Private Methods

    /// <summary> Event handler called by Application for idle events. </summary>
    /// <remarks> Handles stopping the application for case the for stack is executed by <see cref="RunApplication"/>;
    ///           i.e.  when this.ExecutionState == ExecState.RunsAppRun </remarks>
    /// <param name="sender"> The source of the event. </param>
    /// <param name="args">      The specific event data </param>
    private void Application_Idle(object sender, EventArgs args)
    {
        if (this.IsStopRequest)
        {
            Stop();
        }
    }

    /// <summary>
    /// The event handler called when the Form is closed. Is invoked by
    /// <see cref="IStackedForm.evStackItemClosed"/> </summary>
    ///
    /// <param name="sender"> The sender of the event. </param>
    /// <param name="e">      The event arguments. </param>
    private void On_evStackItemClosed(object sender, EventFormStackItemClosedArgs e)
    {
        IStackedForm popped = null;

        if (this.IsStackEmpty)
        {
            // don't even bother, if the direct Stop() call already removed all contents
        }
        else
        {
#if DEBUG
            IStackedForm top = this.TopStackedForm;
            IStackedForm iSender = sender as IStackedForm;
            StringBuilder sbErrMsg = new();
            bool senderIsWrong = false;

            if (null == top)
            {
                sbErrMsg.AppendFormat(CultureInfo.InvariantCulture,
                  "Could not find the top stacked Form");
                Debug.Fail(sbErrMsg.ToString());
            }
            else
            {
                if (null != iSender)
                {
                    senderIsWrong = !object.ReferenceEquals(sender, top);
                }
                else if (!object.ReferenceEquals(sender, top.MyForm))
                {
                    senderIsWrong = true;
                }
                if (senderIsWrong)
                {
                    sbErrMsg.AppendFormat(CultureInfo.InvariantCulture,
                      "The sender is not the top Form!{0}sender = {1}, top = {2}",
                      Environment.NewLine,
                      sender.GetType().TypeToReadable(),
                      top.GetType().TypeToReadable());
                    sbErrMsg.AppendFormat(CultureInfo.InvariantCulture,
                      "{0}{1}", Environment.NewLine, _strWarnTopDoesNotMatch);
                    Debug.Fail(sbErrMsg.ToString());
                }
            }
#endif // DEBUG
            popped = Pop1();
            if (e.ShouldRemove && (popped != null) && !popped.IsModalState)
            { // I cannot do this for a Form that is still in a modal state
                RemoveUnusedForms(item => object.ReferenceEquals(item, popped));
            }
        }
    }
    #endregion // Private Methods
    #endregion // Methods

    #region IDisposable members

    /// <summary>
    /// Implements IDisposable. Do not make this method virtual. A derived class should not be able to
    /// override this method. </summary>
    public void Dispose()
    {
        Dispose(true);
        // Take yourself off the Finalization queue to prevent finalization code 
        // for this object from executing a second time.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the
    /// method has been called directly or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed. If disposing equals false, the method has been called by the runtime from
    /// inside the finalizer and you should not reference other objects. Only unmanaged resources can
    /// be disposed. </summary>
    ///
    /// <remarks> The implementation calls the <see cref="Stop"/> method. </remarks>
    ///
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by
    ///   finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.Stop();
        }
    }
    #endregion // IDisposable members

    #region ISuspendable members

    /// <summary> Is the Form stack suspended ? </summary>
    /// <remarks> While suspended, the <see cref="Run"/> method does not process any messages.</remarks>
    /// <value> true if this object is suspended, false if not. </value>
    public bool IsSuspended
    {
        get { return (_nSuspended > 0); }
    }

    /// <summary> Suspend the Form stack. </summary>
    /// <remarks> While suspended, the <see cref="Run"/> method does not process any messages.</remarks>
    /// <returns> The amount of total suspends acquired. </returns>
    public int Suspend()
    {
        return System.Threading.Interlocked.Increment(ref _nSuspended);
    }

    /// <summary> Revive the Form stack. </summary>
    /// <remarks> While suspended, the <see cref="Run"/> method does not process any messages.</remarks>
    /// <returns> The amount of remaining suspends. </returns>
    public int Revive()
    {
        return System.Threading.Interlocked.Decrement(ref _nSuspended);
    }
    #endregion // ISuspendable members

    #region ICompactable Members

    /// <summary> Compacts this Form stack. </summary>
    ///
    /// <remarks> Calls <see cref="AggressiveCompact"/> and <see cref="FriendlyCompact"/>.</remarks>
    public void Compact()
    {
        AggressiveCompact(null);
        FriendlyCompact();
    }
    #endregion // ICompactable Members
}
