using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using PK.PkUtils.Interfaces;
using PK.PkUtils.WinApi;

namespace PK.SubstEditLib.Subst
{
    [CLSCompliant(true)]
    public class SubstEditTextBoxCtrl<TFIELDID> : TextBox, IModified, IClipboardable
    {
        #region Typedefs
        public delegate void KeyboardEventHandler(object sender, KeyEventArgs e);
        #endregion // Delegates

        #region Fields
        protected Dictionary<Keys, KeyboardEventHandler> _keyTraps;
        protected SubstEditHook<TFIELDID> _hook;
        #endregion // Fields

        #region Constructor(s)

        public SubstEditTextBoxCtrl()
        {
            _hook = new SubstEditHook<TFIELDID>(this);
            AddKeyStroke(Keys.Control | Keys.Y, new KeyboardEventHandler(OnKeyCtrlY));
        }
        #endregion // Constructor(s)

        #region Properties

        public SubstEditHook<TFIELDID> TheHook
        {
            get { return _hook; }
        }

        public SubstLogData<TFIELDID> LogData
        {
            get { return TheHook.LogData; }
        }

        public event ModifiedEventHandler evSubstEditContentsChanged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add { _hook.evModified += value; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove { _hook.evModified -= value; }
        }

        /// <summary>
        /// The event raised if selection has been changed. This one is missing in "classic" text box.
        /// </summary>
        public event EventHandler<SelChagedEventArgs> evSelChaged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add { this.TheHook.evSelChaged += value; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove { this.TheHook.evSelChaged -= value; }
        }
        #endregion // Properties

        #region Methods

        /// <summary>
        /// Assigns the substitution amp
        /// </summary>
        /// <param name="substMap"></param>
        public void AssignSubstMap(IEnumerable<ISubstDescr<TFIELDID>> substMap)
        {
            _hook.AssignSubstMap(substMap);
        }

        /// <summary>
        /// Insert new field on the caret pos.
        /// </summary>
        /// <param name="what"></param>
        /// <returns></returns>
        public bool InsertNewInfo(TFIELDID what)
        {
            return _hook.InsertNewInfo(what);
        }

        /// <summary>
        /// Setting the focus, while preserving the selection info.
        /// Needed since just seting the focus itself sometims selects the whole text.
        /// </summary>
        public void RestoreFocus()
        {
            this.TheHook.RestoreFocus();
        }

        public void Assign(SubstLogData<TFIELDID> rhs)
        {
            TheHook.Assign(rhs);
            TheHook.EmptyUndoBuffer();
        }

        public void AssignPlainText(string strPlain)
        {
            TheHook.AssignPlainText(strPlain);
            TheHook.EmptyUndoBuffer();
        }

        public string GetPlainText()
        {
            return TheHook.GetPlainText();
        }

        public void EmptyUndoBuffer()
        {
            TheHook.EmptyUndoBuffer();
        }

        /// <summary>
        /// Add keystroke you want to handle
        /// </summary>
        /// <param name="key">Keystroke (example: (Keys.Alt | Keys.H))</param>
        /// <param name="method">Method that will handle keystroke</param>
        /// <exception cref="ArgumentNullException">Method is null</exception>
        public void AddKeyStroke(Keys key, KeyboardEventHandler method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            // First use, create dictionary. Slower but saves memory a bit
            _keyTraps ??= [];
            // Add key+handler to dictionary
            _keyTraps.Add(key, new KeyboardEventHandler(method));
        }

        /// <summary>
        /// Remove keystroke
        /// </summary>
        /// <param name="key">Keystroke (example: (Keys.Alt | Keys.H))</param>
        public void RemoveKeyStroke(Keys key)
        {
            if ((_keyTraps != null) && _keyTraps.ContainsKey(key))
            {
                _keyTraps.Remove(key);
            }
        }

        /// <summary>
        /// This method is called during message preprocessing to handle command keys. 
        /// If intercepts any key added previously by 'AddKeyStroke', invokes corresponding handler.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool result = false;
            if ((_keyTraps != null) && _keyTraps.ContainsKey(keyData))
            {
                _keyTraps[keyData](this, new KeyEventArgs(keyData));
                result = true;
            }
            return result;
        }

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

        protected void OnKeyCtrlY(object sender, KeyEventArgs e)
        {
            User32.PostMessage(this.Handle, (int)Win32.RichEm.EM_REDO, IntPtr.Zero, IntPtr.Zero);
        }
        #endregion // Keyboard handlers

        #region IModified Members

        public event ModifiedEventHandler evModified
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add { _hook.evModified += value; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove { _hook.evModified -= value; }
        }

        public bool IsModified
        {
            get { return _hook.IsModified; }
        }

        public void SetModified(bool bValue)
        {
            _hook.SetModified(bValue);
        }
        #endregion // IModified Members

        #region IClipboardable Members

        public bool CanCut
        {
            get { return TheHook.CanCut; }
        }

        public bool CanCopy
        {
            get { return TheHook.CanCopy; }
        }

        public bool CanPaste
        {
            get { return TheHook.CanPaste; }
        }
        // The default implementation of Cut, Copy and Paste in TextBox is sufficient.

        #endregion // IClipboardable Members
    }
}
