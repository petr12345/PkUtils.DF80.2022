// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Reflection;

namespace PK.PkUtils.UI.CollectionEditorExtending;

/// <summary> A collection editor utilities. </summary>
public static class CollectionEditorUtils
{
    #region Properties

    /// <summary> Gets the type of the collection form. </summary>
    internal static Type CollectionFormType
    {
        get => CustomCollectionEditor<object>.CollectionFormType;
    }
    #endregion // Properties

    #region Methods

    /// <summary> Query if 'hwnd' is collection editor form. </summary>
    /// <param name="hwnd"> The examined window handle. </param>
    /// <param name="form"> [out] The form. </param>
    /// <returns>   True if collection editor form, false if not. </returns>
    public static bool IsCollectionEditorForm(IntPtr hwnd, out Form form)
    {
        Control ctrl = Control.FromHandle(hwnd);
        bool result = false;

        if ((ctrl is Form resultForm) && IsCollectionEditorForm(resultForm))
        {
            form = resultForm;
            result = true;
        }
        else
        {
            form = null;
        }

        return result;
    }

    /// <summary> Query if <paramref name="form"/> is collection editor form. </summary>
    /// <param name="form"> The examined Form. </param>
    /// <returns>   True if <paramref name="form"/> is collection editor form, false if not. </returns>
    public static bool IsCollectionEditorForm(Form form)
    {
        return (form != null) && CollectionFormType.IsInstanceOfType(form);
    }

    /// <summary> Searches for the first property grid in <paramref name="collectionForm"/>. </summary>
    /// <remarks>
    /// It is assumed the actual type of  <paramref name="collectionForm"/> is CollectionEditor.CollectionForm,
    /// but that type can't be used in code of class not derived from <see cref="CollectionEditor"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="collectionForm"/> is null. </exception>
    /// <param name="collectionForm"> The collection form. Can't be null. </param>
    /// <returns>   The found property grid, or null. </returns>
    public static PropertyGrid FindPropertyGrid(Form collectionForm)
    {
        ArgumentNullException.ThrowIfNull(collectionForm);
        return collectionForm.FindControl<PropertyGrid>(recursive: true);
    }

    /// <summary> Gets collection form "Items" property value. </summary>
    /// <remarks>
    /// It is assumed the actual type of  <paramref name="collectionForm"/> is CollectionEditor.CollectionForm,
    /// but that type can't be used in code of class not derived from <see cref="CollectionEditor"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="collectionForm"/> is null. </exception>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="collectionForm"> The collection form. Can't be null. </param>
    /// <returns>   The collection form items. </returns>
    public static IReadOnlyList<T> GetCollectionFormItems<T>(Form collectionForm)
    {
        ArgumentNullException.ThrowIfNull(collectionForm);

        PropertyInfo itemsProperty = collectionForm.GetType().GetProperty("Items",
            BindingFlags.NonPublic | BindingFlags.Instance);
        IReadOnlyList<T> result = null;

        if (itemsProperty != null)
        {
            object[] items = (object[])itemsProperty.GetValue(collectionForm);
            result = [.. items.OfType<T>()];
        }

        return result;
    }

    /// <summary> Sets collection form items. </summary>
    /// <param name="collectionForm"> The collection form. Can't be null. </param>
    /// <param name="items"> The items array. Can't be null. </param>
    public static void SetCollectionFormItems(Form collectionForm, object[] items)
    {
        ArgumentNullException.ThrowIfNull(collectionForm);
        ArgumentNullException.ThrowIfNull(items);

        collectionForm.SetInstancePropertyValueEx<object[]>(items, "Items");
    }

    /// <summary> Updates the collection form items from list box. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <param name="collectionForm"> The collection form. Can't be null. </param>
    /// <param name="listBox"> The list box control. </param>
    public static void UpdateCollectionFormItemsFromListBox(Form collectionForm, ListBox listBox)
    {
        ArgumentNullException.ThrowIfNull(collectionForm);
        ArgumentNullException.ThrowIfNull(listBox);

        int itemsCount = listBox.Items.Count;
        object[] items = new object[itemsCount];

        for (int ii = 0; ii < itemsCount; ii++)
        {
            // The type of that item at Items[ii] is "System.ComponentModel.Design.CollectionEditor+CollectionEditorCollectionForm+ListItem",
            // but ListItem is inaccessible, hence accessing the 'Value' is not possible directly, like
            // object value = ((ListItem)listBx.Items[ii]).Value;
            object value = listBox.Items[ii].GetInstancePropertyValue("Value");

            items[ii] = value;
        }
        SetCollectionFormItems(collectionForm, items);
    }

    /// <summary> Gets click event handler delegate. </summary>
    /// <param name="button"> The button control. Can't be null. </param>
    public static Delegate GetClickEventHandlerDelegate(Button button)
    {
        ArgumentNullException.ThrowIfNull(button, nameof(button));
        Delegate result = null;

        PropertyInfo eventHandlerListProperty = typeof(Component).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo clickEventField = typeof(Control).GetField("s_clickEvent", BindingFlags.Static | BindingFlags.NonPublic);
        object clickEventKey = clickEventField?.GetValue(null); // Static field, so use null for instance

        if ((eventHandlerListProperty != null) && (clickEventKey != null))
        {
            EventHandlerList eventHandlerList = (EventHandlerList)eventHandlerListProperty.GetValue(button);
            if (eventHandlerList[clickEventKey] is Delegate clickEventHandler)
            {
                result = clickEventHandler;
            }
        }

        return result;
    }

    /// <summary> Gets click event handler delegate for case of NET 4.8. </summary>
    /// <param name="button"> The button control. Can't be null. </param>
    public static Delegate GetClickEventHandlerDelegate_NET48(Button button)
    {
        button.CheckArgNotNull(nameof(button));

        object clickEventKey = null;
        Delegate result = null;

        try
        {
            clickEventKey = typeof(Control).GetStaticFieldValueEx<object>("EventClick");
        }
        catch (InvalidOperationException) { }

        if (clickEventKey != null)
        {
            EventHandlerList buttonEvents = button.GetInstancePropertyValueEx<EventHandlerList>("Events");
            if (buttonEvents != null)
            {
                result = buttonEvents[clickEventKey];
            }
        }

        return result;
    }

    /// <summary> Removes all click-event handlers installed on <paramref name="button"/>, and returns them in list. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="button"/> is null. </exception>
    /// <param name="button"> The button control. Can't be null. </param>
    /// <returns> The list of removed event handlers. </returns>
    internal static List<EventHandler> RemoveAllClickEventHandlers(Button button)
    {
        ArgumentNullException.ThrowIfNull(button, nameof(button));

        List<EventHandler> removedHandlers = [];
        Delegate clickEventHandler = GetClickEventHandlerDelegate(button) ?? GetClickEventHandlerDelegate_NET48(button);

        if (clickEventHandler != null)
        {
            // If there are event handlers registered for the click event, iterate through each handler and remove it
            foreach (EventHandler buttonClickHandler in clickEventHandler.GetInvocationList().OfType<EventHandler>())
            {
                removedHandlers.Add((sender, e) => buttonClickHandler(sender, e));
                button.Click -= buttonClickHandler;
            }
        }

        return removedHandlers;
    }
    #endregion // Methods
}
