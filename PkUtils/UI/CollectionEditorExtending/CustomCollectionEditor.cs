// Ignore Spelling: Utils, validator, listbox
//
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Reflection;


#pragma warning disable IDE0290 // Use primary constructor

namespace PK.PkUtils.UI.CollectionEditorExtending;

/// <summary> Custom collection editor to be used on property grid. </summary>
/// <example>
/// This is how to defined the usage of editor on collection in a class
/// <code>
/// <![CDATA[
/// 
/// public class SpecializedCollectionEditor : CustomCollectionEditor
/// {
///   protected override bool ValidateItemsOnClosing(IReadOnlyList<object> items)
///   {
///     
///   }
/// }
///  
/// [Editor(typeof(CustomCollectionEditor), typeof(UITypeEditor))]
/// [Description("My collection")]
/// public List<SectionBasicInfo> MyCollection
/// {
///   get { return this.m_myCollection; }
///   set { this.m_myCollection = value; }
/// }
/// 
/// ]]>
/// </code>
/// </example>
public class CustomCollectionEditor<T> : CollectionEditor
{
    #region Events

    /// <summary>
    /// Declares a public static event to expose the inner PropertyGrid's PropertyValueChanged event.
    /// </summary>
    public static event EventHandler<PropertyValueChangedEventArgs> PropertyValueChanged;

    /// <summary>
    /// Declares a public static event to expose the inner CollectionForm FormClosed event.
    /// </summary>
    public static event EventHandler<FormClosedEventArgs> PropertyEditorFormClosed;
    #endregion // Events

    #region Fields

    private CollectionForm _collectionForm;
    private Button _okButton;
    private event EventHandler CustomButtonClickEvent;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Inherit the default constructor from the standard... </summary>
    /// <param name="type">The collection type.</param>
    public CustomCollectionEditor(Type type) : base(type)
    { }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets the type of the collection form. </summary>
    /// <remarks> This is needed to be able to "publish" that type in <see cref="CollectionEditorUtils.CollectionFormType"/> </remarks>
    protected internal static Type CollectionFormType { get { return typeof(CollectionForm); } }

    /// <summary>
    /// Gets a list of possible names of the private bool field in CollectionEditorCollectionForm, indicating
    /// "dirty" status. In older .NET versions, the field was called 'dirty'.
    /// </summary>
    protected static IEnumerable<string> PossibleCollectionFormDirtyFieldNames
    {
        get => ["_dirty", "dirty"];
    }

    /// <summary> Gets the hooked collection form. </summary>
    protected CollectionForm HookedCollectionForm { get => _collectionForm; }

    /// <summary> Gets the hooked OK button. </summary>
    protected Button HookedOkButton { get => _okButton; }

    /// <summary> Gets a value indicating whether collection form is hooked. </summary>
    protected bool IsCollectionFormHooked { get => HookedCollectionForm != null; }

    /// <summary> Gets a value indicating whether the OK button is hooked. </summary>
    protected bool IsOkButtonHooked { get => HookedOkButton != null; }

    /// <summary>
    /// Returns true if the collection form is hooked, and the private bool CollectionEditorCollectionForm._dirty is true.
    /// </summary>
    protected bool IsHookedCollectionFormDirty
    {
        get => IsCollectionFormHooked && IsCollectionFormDirty(HookedCollectionForm);
    }
    #endregion // Properties

    #region Methods

    #region Infrastructure

    /// <summary>
    /// Returns true if the <paramref name="collectionForm"/> is indeed CollectionForm, 
    /// and the private bool CollectionEditorCollectionForm._dirty is true.
    /// </summary>
    /// <remarks>
    /// The actual hierarchy is: 
    /// class CollectionEditorCollectionForm : CollectionForm;
    /// class CollectionForm : Form;
    /// In older .NET versions, the field was called 'dirty'.
    /// </remarks>
    /// <param name="collectionForm"> The collection form. </param>
    /// <returns> True if collection form dirty, false if not. </returns>
    public static bool IsCollectionFormDirty(Form collectionForm)
    {
        bool result = false;

        if (collectionForm is CollectionForm cf)
        {
            foreach (string fieldName in PossibleCollectionFormDirtyFieldNames)
            {
                if (FieldsUtils.GetAllFieldsAssignableTo(cf.GetType(), typeof(bool), FieldsUtils.AnyInstance, fieldName).Any())
                {
                    result = cf.GetInstanceFieldValueEx<bool>(fieldName);
                    break;
                }
            }
        }

        return result;
    }

    /// <summary> Sets collection form dirty. Is an additive to <see cref="IsCollectionFormDirty"/> </summary>
    /// <param name="collectionForm"> The collection form. </param>
    /// <param name="value"> (Optional) True to set dirty. </param>
    public static void SetCollectionFormDirty(Form collectionForm, bool value = true)
    {
        if (collectionForm is CollectionForm cf)
        {
            foreach (string fieldName in PossibleCollectionFormDirtyFieldNames)
            {
                if (FieldsUtils.GetAllFieldsAssignableTo(cf.GetType(), typeof(bool), FieldsUtils.AnyInstance, fieldName).Any())
                {
                    cf.SetInstanceFieldValueEx<bool>(fieldName, value);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Overrides <see cref="CollectionEditor.CreateCollectionForm()"/> to enable custom modifications.
    /// </summary>
    /// <remarks>
    /// The actual type of created instance will be even the derived CollectionEditorCollectionForm.
    /// </remarks>
    /// <returns>
    /// A <see cref="CollectionEditor.CollectionForm"/> to provide an user interface for editing the collection.
    /// </returns>
    protected override CollectionForm CreateCollectionForm()
    {
        // Getting the default layout of the Collection Editor...
        CollectionForm collectionForm = base.CreateCollectionForm();

        if (HookCollectionForm(collectionForm))
        {
            HookOkButton();
        }

        return collectionForm;
    }

    /// <summary>
    /// Gets collection validator used for items validation. 
    /// You will override this method in derived collection editor.
    /// </summary>
    /// <returns> The collection validator, or null if none present. </returns>
    protected virtual ICollectionValidator<T> GetCollectionValidator()
    {
        return null;
    }

    /// <summary> Gets the collection of items the CollectionForm  is to display. </summary>
    protected virtual IReadOnlyList<T> GetCollectionFormItems()
    {
        IReadOnlyList<T> result = null;

        if (IsCollectionFormHooked)
        {
            // Retrieve the value of the Items property from the Form by reflection
            result = CollectionEditorUtils.GetCollectionFormItems<T>(HookedCollectionForm);
        }

        return result;
    }

    /// <summary> Searches for the first parent collection form. </summary>
    /// <returns> The found parent collection form, or null if none found. </returns>
    protected virtual CollectionForm FindParentCollectionForm()
    {
        CollectionForm currentForm = HookedCollectionForm;
        CollectionForm parentCollectionForm = currentForm.ParentForm as CollectionForm;

        parentCollectionForm ??= currentForm.Owner as CollectionForm;

        return parentCollectionForm;
    }

    /// <summary>
    /// Should be called before <see cref="ValidateCollectionFormItems"/>
    /// Updates the contents of CollectionForm.Items property, if that form is 'dirty'.
    /// </summary>
    /// <remarks>
    /// The implementation here is just needed part of code taken from
    /// CollectionEditorCollectionForm.OKButton_Click handler. Must be called separately, since our validation
    /// here precedes that event handler.
    /// </remarks>
    /// <returns> True if was updated, false if not. </returns>
    protected virtual bool UpdateCollectionFormItemsIfDirty()
    {
        bool result = false;

        if (IsHookedCollectionFormDirty)
        {
            Form form = HookedCollectionForm;
            ListBox listBox = form.AllControls<ListBox>().FirstOrDefault();

            if (listBox != null)
            {
                CollectionEditorUtils.UpdateCollectionFormItemsFromListBox(form, listBox);
                result = true;
            }
        }

        return result;
    }
    #endregion // Infrastructure

    #region Validation

    /// <summary>
    /// Validates the items on CollectionForm closing. 
    /// In derived class, override this validation to prevent closing for invalid collection.
    /// </summary>
    /// <param name="items"> The items to validate. Can't be null. </param>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected virtual bool ValidateItemsOnClosing(IReadOnlyList<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        ICollectionValidator<T> validator = GetCollectionValidator();
        bool result = true;

        if (validator != null)
        {
            try
            {
                validator.Validate(items);
            }
            catch (SystemException ex)
            {
                string errorMessage = ex.Message;
                // remove from message the second sentence in brackets
                int index = errorMessage.IndexOf("Parameter ", StringComparison.InvariantCulture);
                if (index >= 0)
                {
                    errorMessage = errorMessage[..index].TrimEnd('(');
                }

                MessageBox.Show(errorMessage, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                result = false;
            }
        }

        return result;
    }

    /// <summary> Validates the items taken from hooked CollectionForm. </summary>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected virtual bool ValidateCollectionFormItems()
    {
        IReadOnlyList<T> listItems = GetCollectionFormItems();
        bool result = (listItems == null) || ValidateItemsOnClosing(listItems);

        return result;
    }
    #endregion // Validation

    #region Hooking

    /// <summary> Hook the collection form. </summary>
    /// <param name="collectionForm"> The collection form. Can't be null. </param>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected virtual bool HookCollectionForm(CollectionForm collectionForm)
    {
        ArgumentNullException.ThrowIfNull(collectionForm);
        PropertyGrid propertyGrid = CollectionEditorUtils.FindPropertyGrid(collectionForm);

        // Get a reference to the inner PropertyGrid and hook an event handler to it
        if (propertyGrid != null)
        {
            propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(OnPropertyGrid_PropertyValueChanged);

            // also subscribe to CollectionForm FormClosing and FormClosed event
            collectionForm.FormClosing += CollectionForm_FormClosing;
            collectionForm.FormClosed += CollectionForm_FormClosed;

            _collectionForm = collectionForm;
        }

        return IsCollectionFormHooked;
    }

    /// <summary> Unhook previously hooked collection form. </summary>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected virtual bool UnhookCollectionForm()
    {
        CollectionForm form;

        if ((form = HookedCollectionForm) != null)
        {
            PropertyGrid propertyGrid = CollectionEditorUtils.FindPropertyGrid(form);

            form.FormClosing -= CollectionForm_FormClosing;
            form.FormClosed -= CollectionForm_FormClosed;
            propertyGrid.PropertyValueChanged -= OnPropertyGrid_PropertyValueChanged;

            _collectionForm = null;
        }

        return !IsCollectionFormHooked;
    }

    /// <summary>
    /// Unsubscribe(s) the _okButton from the previous handler CollectionEditorCollectionForm.OKButton_Click, 
    /// and adds the handler OKButton_Click defined here.
    /// This is needed, since the method CollectionEditorCollectionForm.OKButton_Click
    /// for unknown reason calls _listbox.Items.Clear() even before any possible validation.
    /// </summary>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected virtual bool HookOkButton()
    {
        bool result = false;

        if (IsCollectionFormHooked)
        {
            if (HookedCollectionForm.AcceptButton is Button okButton)
            {
                List<EventHandler> removedHandlers = CollectionEditorUtils.RemoveAllClickEventHandlers(okButton);
                // Store originally subscribed event handlers, in custom event
                if (removedHandlers?.Count > 0)
                {
                    removedHandlers.ForEach(handler => CustomButtonClickEvent += handler);
                }

                _okButton = okButton;
                okButton.Click += OkButton_Click;
                result = true;
            }
        }

        return result;
    }

    /// <summary> Unhook previously hooked _okButton. </summary>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected virtual bool UnhookOkButton()
    {
        if (IsOkButtonHooked)
        {
            HookedOkButton.Click -= OkButton_Click;
            _okButton = null;
            CustomButtonClickEvent = null;
        }

        return !IsOkButtonHooked;
    }
    #endregion//  Hooking
    #endregion // Methods

    #region Event_handlers

    private void OnPropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs args)
    {
        // Fire our customized collection event...
        PropertyValueChanged?.Invoke(this, args);
    }

    private void CollectionForm_FormClosing(object sender, FormClosingEventArgs args)
    {
        if (!args.Cancel)
        {
            CollectionForm collectionForm = sender as CollectionForm;
            DialogResult dialogResult = collectionForm.DialogResult;

            switch (args.CloseReason)
            {
                case CloseReason.None:
                case CloseReason.UserClosing:
                case CloseReason.FormOwnerClosing:
                    if (dialogResult == DialogResult.OK)
                    {
                        UpdateCollectionFormItemsIfDirty();
                        if (!ValidateCollectionFormItems())
                        {
                            args.Cancel = true;
                        }
                    }
                    break;
            }
        }
    }

    private void CollectionForm_FormClosed(object sender, FormClosedEventArgs args)
    {
        // Fire our event...
        PropertyEditorFormClosed?.Invoke(this, args);

        // and unsubscribe, in an opposite order
        UnhookOkButton();
        UnhookCollectionForm();
    }

    private void OkButton_Click(object sender, EventArgs args)
    {
        UpdateCollectionFormItemsIfDirty();
        if (ValidateCollectionFormItems())
        {
            // invoke originally subscribed event handlers
            CustomButtonClickEvent?.Invoke(sender, args);
        }
        else
        {
            // Needed to prevent form from closing with calling the validation second time in CollectionForm_FormClosing
            HookedCollectionForm.DialogResult = DialogResult.None;
        }
    }
    #endregion // Event_handlers
}


#pragma warning restore IDE0290 // Use primary constructor
