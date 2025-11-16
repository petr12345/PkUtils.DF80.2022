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

// Ignore Spelling: Utils
//
using System;
using System.Threading;
using System.Windows.Forms;

#pragma warning disable IDE0290     // Use primary constructor

namespace PK.PkUtils.UI.StackedForms;

/// <summary>
/// Interface that is mandatory for any class which works as an StackId - an identifier of stacked Form.
/// The class <see cref="FormStack"/> uses the StackId to identify the Form ( see FormStack.FindForm(IStackId id) ).
/// In the most simple case, the stacked Form is identified by just its type.
/// For the basic implementation, see the class FormStack.IStackId. 
/// <remarks>
/// Note: In the most common case, any two different forms in your application have different types, 
/// hence the basic implementation of IStackId in public class StackId is quite sufficient.
/// However, there could be more complicated cases, if you application could more forms of the same type,
/// that differ just by their child controls (aka 'views').
/// In that case, you should derive your new identifier, like 
/// <code>
/// class StackIdEx : StackId
/// </code>
/// and override its two methods 
/// <code>
/// public override bool Equals(object obj)
/// public override int GetHashCode()
/// </code>
/// </remarks>
/// </summary>
[CLSCompliant(true)]
public interface IStackId : IEquatable<IStackId>
{
    /// <summary> Gets the type of the Form. </summary>
    ///
    /// <value> The type of the Form. </value>
    Type FormType { get; }
}

/// <summary>
/// The event arguments for event that is raised when something interesting
/// is happening with the Form ( item ) in the FormStack.
/// </summary>
[CLSCompliant(true)]
public class EventFormStackItemActionArgs : EventArgs
{
    /// <summary> Gets or sets the Form this event relates to. </summary>
    ///
    /// <value> The Form related. </value>
    public IStackedForm FormRelated { get; protected set; }

    /// <summary> The constructor. </summary>
    ///
    /// <param name="iForm">  The stacked Form this event relates to.</param>
    public EventFormStackItemActionArgs(IStackedForm iForm)
      : base()
    {
        FormRelated = iForm;
    }
}

/// <summary>
/// The event arguments for event that is raised when the FormStack item is closed.
/// </summary>
[CLSCompliant(true)]
public class EventFormStackItemClosedArgs : EventFormStackItemActionArgs
{
    /// <summary> The constructor. </summary>
    /// <param name="iForm">  The stacked Form this event relates to.</param>
    public EventFormStackItemClosedArgs(IStackedForm iForm)
      : this(iForm, !iForm.PermitsCaching)
    {
    }

    /// <summary> The constructor. </summary>
    /// <param name="iForm">        The stacked Form this event relates to. </param>
    /// <param name="bShouldRemove"> True if the Form should be Form removed from the cache upon
    ///                             popping; false otherwise. 
    ///                             Will be used as value of <see cref="ShouldRemove"/> property.
    ///                             </param>
    public EventFormStackItemClosedArgs(IStackedForm iForm, bool bShouldRemove)
      : base(iForm)
    {
        ShouldRemove = bShouldRemove;
    }

    /// <summary>
    /// Returns true if the Form should be Form removed from the cache upon popping;
    /// false otherwise.
    /// </summary>
    public bool ShouldRemove
    {
        get;
        protected set;
    }
}

/// <summary>
/// The functionality assumed as mandatory in any stacked Form class.
/// For more details, see the class <see cref="FormStack"/>.
/// </summary>
[CLSCompliant(true)]
public interface IStackedForm
{
    /// <summary>
    /// The event raised when the Form is closed
    /// </summary>
    event EventHandler<EventFormStackItemClosedArgs> EventStackItemClosed;

    /// <summary>
    /// The event used to signalize when the Form data preloading has finished.,
    /// See the StackedForm.DataThread() method.
    /// </summary>
    ManualResetEvent EventPreloadDone { get; }

    /// <summary>
    /// The Form ID
    /// </summary>
    IStackId FormId { get; }

    /// <summary>
    /// Get the actual Form
    /// </summary>
    Form MyForm { get; }

    /// <summary>
    /// Should return true if the Form could be cached; false otherwise.
    /// </summary>
    bool PermitsCaching { get; }

    /// <summary>
    /// Should return true if the method InitializeComponent is NOT called by constructor,
    /// but needs to be called explicitly later.
    /// </summary>
    bool PermitsPostInitialize { get; }

    /// <summary>
    /// Should return true while ShowModal has been called and had not returned yet
    /// </summary>
    bool IsModalState { get; }

    /// <summary>
    /// Encapsulates the classic WinForms .Visible property.
    /// On purpose we have chosen a different property name, 
    /// with getter only, and separate <see cref="StackedSetVisible"/>method.
    /// </summary>
    bool IsFormVisible { get; }

    /// <summary> Encapsulates the classic WinForms .Visible property. For more info see
    /// <seealso cref="IsFormVisible"/>. </summary>
    ///
    /// <param name="bValue">The caller will set to true to make the form visible; false otherwise. </param>
    void StackedSetVisible(bool bValue);

    /// <summary>
    /// Encapsulates WinForms method ShowDialog();
    /// </summary>
    /// <returns>The result of the ShowDialog() call.</returns>
    DialogResult StackedShowModal();

    /// <summary>
    /// Form activation.
    /// Encapsulates the classic WinForms.Activate method,
    /// together with initializing by optional extra data argument.
    /// </summary>
    /// <param name="extraArg">Encapsulates an optional data, provided by the caller.</param>
    void StackedActivate(object extraArg);

    /// <summary>
    /// A wrapper around "classic" InitializeComponent,
    /// making it accessible from IStackedForm interface.
    /// </summary>
    void StackedInitializeComponent();

    /// <summary> Loads the Form initial data; it may start a separate thread to allow the Form to appear sooner
    /// before that loading is complete. </summary>
    ///
    /// <param name="bThreadStarted"> An output argument; a value becomes true if the method
    /// implementation has started a separate thread loading the data. </param>
    void LoadData(out bool bThreadStarted);

    /// <summary>
    /// Populate the Form controls with any retrieved data 
    /// ( that were previously loaded by LoadData )
    /// </summary>
    /// <returns>True on success, false on failure. </returns>
    bool Populate();
}
