// Ignore Spelling: Utils, Dict, listbox, validator
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.UI.CollectionEditorExtending;


namespace PK.PkUtils.UI.CollectionEditorHooking;

/// <summary>
/// Auxiliary class keeping information about CollectionForm. Is used as a value in internal dictionary in <see cref="CollectionEditorObserver{T}"/>
/// </summary>
/// <remarks>
/// On purpose it is not structure - to permit and descendants of CollectionEditorObserver derive from it.
/// </remarks>
/// <typeparam name="T"> Generic type parameter. </typeparam>
public class CollectionFormHook<T> : CollectionFormInfo
{
    #region Fields

    private Form _collectionForm;
    private Button _okButton;
    private event EventHandler CustomButtonClickEvent;
    private readonly CollectionEditorObserver<T> _parentObserver;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Constructor. </summary>
    /// <param name="hwnd"> The hwnd. </param>
    /// <param name="observer"> The observer. </param>
    protected internal CollectionFormHook(IntPtr hwnd, CollectionEditorObserver<T> observer)
        : base(hwnd)
    {
        _parentObserver = observer;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets the hooked collection form. </summary>
    protected internal Form HookedCollectionForm { get => _collectionForm; }

    /// <summary> Gets the hooked OK button. </summary>
    protected internal Button HookedOkButton { get => _okButton; }

    /// <summary> Gets a value indicating whether collection form is hooked. </summary>
    protected internal bool IsCollectionFormHooked { get => HookedCollectionForm != null; }

    /// <summary> Gets a value indicating whether this object is ok button hooked. </summary>
    protected internal bool IsOkButtonHooked { get => HookedOkButton != null; }

    /// <summary> Gets the parent observer, as initialized by constructor. </summary>
    protected CollectionEditorObserver<T> ParentObserver => _parentObserver;

    /// <summary>
    /// Returns true if the collection form is hooked, and the private bool CollectionEditorCollectionForm._dirty is true.
    /// </summary>
    /// <remarks>
    /// The actual hierarchy is: class CollectionEditorCollectionForm : CollectionForm;
    /// class CollectionForm : Form;
    /// In older .NET versions, the field was called 'dirty'.
    /// </remarks>
    protected bool IsHookedCollectionFormDirty
    {
        get => IsCollectionFormHooked && CustomCollectionEditor<T>.IsCollectionFormDirty(HookedCollectionForm);
    }
    #endregion // Properties

    #region Methods

    #region Infrastructure

    /// <summary>
    /// Gets collection validator used for items validation. 
    /// You may override this method in derived collection editor.
    /// </summary>
    /// <returns> The collection validator, or null if none present. </returns>
    protected virtual ICollectionValidator<T> GetCollectionValidator()
    {
        return ParentObserver.CollectionValidator;
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

    /// <summary>
    /// Should be called before <see cref="ValidateCollectionFormItems"/>
    /// Updates the contents of CollectionForm.Items property, if that form is 'dirty'.
    /// </summary>
    /// <remarks>
    /// The implementation here is just needed part of code taken from
    /// CollectionEditorCollectionForm.OKButton_Click handler. Must be called separately, 
    /// since our validation here precedes that event handler.
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
    /// Validates the items on CollectionForm closing. In derived class, override this validation to prevent
    /// closing for invalid collection.
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

    /// <summary> Determines if we can hook collection form. </summary>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected internal virtual bool HookCollectionForm()
    {
        Form collectionForm;
        bool result = IsCollectionFormHooked;

        if (!result && ((collectionForm = MapCollectionForm) is not null))
        {
            PropertyGrid propertyGrid = CollectionEditorUtils.FindPropertyGrid(collectionForm);

            // Get a reference to the inner PropertyGrid and hook an event handler to it
            if (propertyGrid != null)
            {
                propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(OnPropertyGrid_PropertyValueChanged);

                // also subscribe to CollectionForm FormClosing and FormClosed event
                collectionForm.FormClosing += CollectionForm_FormClosing;
                collectionForm.FormClosed += CollectionForm_FormClosed;

                _collectionForm = collectionForm;
                result = true;
            }
            else
            {
                ParentObserver.ReportError($"Unable to hook {collectionForm.GetType()}, method {nameof(CollectionEditorUtils.FindPropertyGrid)} has failed.");
            }
        }

        return result;
    }

    /// <summary>
    /// Unsubscribe(s) the _okButton from the previous handler CollectionEditorCollectionForm.OKButton_Click,
    /// and adds the handler OKButton_Click defined here.
    /// This is needed, since the method CollectionEditorCollectionForm.OKButton_Click
    /// for unknown reason calls _listbox.Items.Clear() even before any possible validation.
    /// </summary>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected internal virtual bool HookOkButton()
    {
        bool result = false;

        if (IsCollectionFormHooked)
        {
            result = IsOkButtonHooked;
            if ((!result) && (HookedCollectionForm.AcceptButton is Button okButton))
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
            else
            {
                ParentObserver.ReportError($"Unable to hook OK Button of {HookedCollectionForm.GetType()}");
            }
        }

        return result;
    }

    /// <summary> Unhook previously hooked collection form. </summary>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected internal virtual bool UnhookCollectionForm()
    {
        Form form;

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

    /// <summary> Unhook previously hooked _okButton. </summary>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected internal virtual bool UnhookOkButton()
    {
        if (IsOkButtonHooked)
        {
            HookedOkButton.Click -= OkButton_Click;
            _okButton = null;
            CustomButtonClickEvent = null;
        }

        return !IsOkButtonHooked;
    }
    #endregion // Hooking
    #endregion // Methods

    #region Event_handlers

    private void OnPropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs args)
    {
        // Fire our customized event...
        ParentObserver.RaisePropertyGrid_ValueChanged(sender, args);
    }

    private void CollectionForm_FormClosing(object sender, FormClosingEventArgs args)
    {
        if (!args.Cancel)
        {
            Form collectionForm = sender as Form;
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
        ParentObserver.RaiseCollectionForm_FormClosed(sender, args);

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
