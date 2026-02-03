using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using PK.PkUtils.Interfaces;
using PK.PkUtils.WinApi;

namespace PK.SubstEditLib.Subst;

/// <summary>
/// A TextBox control with substitution-aware editing, undo/redo, and clipboard operations.
/// Supports custom keystroke handling and selection change notifications.
/// </summary>
/// <typeparam name="TFIELDID">The type of the field identifier used for substitutions.</typeparam>
[CLSCompliant(true)]
public class SubstEditTextBoxCtrl<TFIELDID> : TextBox, IModified, IClipboardable
{
    #region Typedefs
    /// <summary>
    /// Delegate for handling keyboard events.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The key event arguments.</param>
    public delegate void KeyboardEventHandler(object sender, KeyEventArgs e);
    #endregion // Delegates

    #region Fields
    /// <summary>
    /// Dictionary mapping keystrokes to their handlers.
    /// </summary>
    protected Dictionary<Keys, KeyboardEventHandler> _keyTraps;
    /// <summary>
    /// The hook implementation for substitution-aware editing.
    /// </summary>
    protected SubstEditHook<TFIELDID> _hook;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="SubstEditTextBoxCtrl{TFIELDID}"/> class.
    /// </summary>
    public SubstEditTextBoxCtrl()
    {
        _hook = new SubstEditHook<TFIELDID>(this);
        AddKeyStroke(Keys.Control | Keys.Y, new KeyboardEventHandler(OnKeyCtrlY));
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets the substitution edit hook.
    /// </summary>
    public SubstEditHook<TFIELDID> TheHook
    {
        get { return _hook; }
    }

    /// <summary>
    /// Gets the logical substitution data.
    /// </summary>
    public SubstLogData<TFIELDID> LogData
    {
        get { return TheHook.LogData; }
    }

    /// <summary>
    /// Occurs when the contents of the substitution edit control have changed.
    /// </summary>
    public event ModifiedEventHandler EventSubstEditContentsChanged
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        add { _hook.EventModified += value; }
        [MethodImpl(MethodImplOptions.Synchronized)]
        remove { _hook.EventModified -= value; }
    }

    /// <summary>
    /// The event raised if selection has been changed. This one is missing in "classic" text box.
    /// </summary>
    public event EventHandler<SelChangedEventArgs> EventSelChanged
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        add { this.TheHook.EventSelChanged += value; }
        [MethodImpl(MethodImplOptions.Synchronized)]
        remove { this.TheHook.EventSelChanged -= value; }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Assigns the substitution map.
    /// </summary>
    /// <param name="substMap">The substitution descriptors to set.</param>
    public void SetSubstitutionMap(IEnumerable<ISubstitutionDescriptor<TFIELDID>> substMap)
    {
        _hook.SetSubstitutionMap(substMap);
    }

    /// <summary>
    /// Inserts a new field at the caret position.
    /// </summary>
    /// <param name="what">The field identifier to insert.</param>
    /// <returns>True if insertion occurred; otherwise false.</returns>
    public bool InsertNewInfo(TFIELDID what)
    {
        return _hook.InsertNewInfo(what);
    }

    /// <summary>
    /// Sets the focus while preserving the selection info.
    /// Needed since just setting the focus itself sometimes selects the whole text.
    /// </summary>
    public void RestoreFocus()
    {
        this.TheHook.RestoreFocus();
    }

    /// <summary>
    /// Assigns the contents from another <see cref="SubstLogData{TFIELDID}"/> instance and empties the undo buffer.
    /// </summary>
    /// <param name="rhs">The source log data.</param>
    public void Assign(SubstLogData<TFIELDID> rhs)
    {
        TheHook.Assign(rhs);
        TheHook.EmptyUndoBuffer();
    }

    /// <summary>
    /// Assigns the contents from plain text and empties the undo buffer.
    /// </summary>
    /// <param name="strPlain">The plain text to assign.</param>
    public void AssignPlainText(string strPlain)
    {
        TheHook.AssignPlainText(strPlain);
        TheHook.EmptyUndoBuffer();
    }

    /// <summary>
    /// Gets the plain text representation of the control's contents.
    /// </summary>
    /// <returns>The plain text with fields replaced by their display text.</returns>
    public string GetPlainText()
    {
        return TheHook.GetPlainText();
    }

    /// <summary>
    /// Empties the undo buffer.
    /// </summary>
    public void EmptyUndoBuffer()
    {
        TheHook.EmptyUndoBuffer();
    }

    /// <summary>
    /// Adds a keystroke to be handled by a custom handler.
    /// </summary>
    /// <param name="key">Keystroke (example: (Keys.Alt | Keys.H)).</param>
    /// <param name="method">Method that will handle keystroke.</param>
    /// <exception cref="ArgumentNullException">Thrown if method is null.</exception>
    public void AddKeyStroke(Keys key, KeyboardEventHandler method)
    {
        ArgumentNullException.ThrowIfNull(method);

        // First use, create dictionary. Slower but saves memory a bit
        _keyTraps ??= [];
        // Add key+handler to dictionary
        _keyTraps.Add(key, new KeyboardEventHandler(method));
    }

    /// <summary>
    /// Removes a keystroke handler.
    /// </summary>
    /// <param name="key">Keystroke (example: (Keys.Alt | Keys.H)).</param>
    public void RemoveKeyStroke(Keys key)
    {
        if ((_keyTraps != null) && _keyTraps.ContainsKey(key))
        {
            _keyTraps.Remove(key);
        }
    }

    /// <summary>
    /// Called during message preprocessing to handle command keys.
    /// If intercepts any key added previously by 'AddKeyStroke', invokes corresponding handler.
    /// </summary>
    /// <param name="msg">The Windows message.</param>
    /// <param name="keyData">The key data.</param>
    /// <returns>True if the key was handled; otherwise false.</returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        bool result = false;
        if ((_keyTraps != null) && _keyTraps.TryGetValue(keyData, out KeyboardEventHandler value))
        {
            value(this, new KeyEventArgs(keyData));
            result = true;
        }
        return result;
    }

    /// <summary>
    /// Releases the resources used by the control.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (null != _hook)
            {
                _hook.Dispose();
                _hook = null;
            }
        }
        base.Dispose(disposing);
    }
    #endregion // Methods

    #region Keyboard handlers

    /// <summary>
    /// Handler for Ctrl+Y (redo) keystroke.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The key event arguments.</param>
    protected void OnKeyCtrlY(object sender, KeyEventArgs e)
    {
        User32.PostMessage(this.Handle, (int)Win32.RichEm.EM_REDO, IntPtr.Zero, IntPtr.Zero);
    }
    #endregion // Keyboard handlers

    #region IModified Members

    /// <summary>
    /// Occurs when the contents of the control have been modified.
    /// </summary>
    public event ModifiedEventHandler EventModified
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        add { _hook.EventModified += value; }
        [MethodImpl(MethodImplOptions.Synchronized)]
        remove { _hook.EventModified -= value; }
    }

    /// <summary>
    /// Gets a value indicating whether the contents have been modified.
    /// </summary>
    public bool IsModified
    {
        get { return _hook.IsModified; }
    }

    /// <summary>
    /// Sets the modified flag and raises the Modified event if set to true.
    /// </summary>
    /// <param name="bValue">The new modified value.</param>
    public void SetModified(bool bValue)
    {
        _hook.SetModified(bValue);
    }
    #endregion // IModified Members

    #region IClipboardable Members

    /// <summary>
    /// Gets a value indicating whether the current selection can be cut to the clipboard.
    /// </summary>
    public bool CanCut
    {
        get { return TheHook.CanCut; }
    }

    /// <summary>
    /// Gets a value indicating whether the current selection can be copied to the clipboard.
    /// </summary>
    public bool CanCopy
    {
        get { return TheHook.CanCopy; }
    }

    /// <summary>
    /// Gets a value indicating whether the clipboard contents can be pasted.
    /// </summary>
    public bool CanPaste
    {
        get { return TheHook.CanPaste; }
    }
    // The default implementation of Cut, Copy and Paste in TextBox is sufficient.

    #endregion // IClipboardable Members
}
