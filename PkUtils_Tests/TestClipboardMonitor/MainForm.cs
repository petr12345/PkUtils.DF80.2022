using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using PK.PkUtils.MessageHooking;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;

namespace PK.TestClipMon
{
    /// <summary>
    /// Clipboard Monitor Example 
    /// <br/>
    /// Demonstrates usage of a clipboard monitor - the class ClipMonitorHook.
    /// Whenever a new contents is copied to the clipboard by any application 
    /// this form will be notified by a call to the WindowProc method 
    /// with the WM_DRAWCLIPBOARD message, allowing this form to
    /// read the contents of the clipboard and perform some processing.
    /// 
    /// The specialized messages Win32.WM.WM_DRAWCLIPBOARD and Win32.WM.WM_CHANGECBCHAIN:
    /// are handled in the class ClipMonitorHook itself ( in the method HookWindowProc ),
    /// so the main thing this form has to care about is to handle the occurrence 
    /// of the event ClipMonitorHook.EventClipboardChanged 
    /// 
    /// </summary>
    public partial class MainForm : System.Windows.Forms.Form
    {
        #region Fields
        #region Clipboard Formats

        protected readonly string[] _arrFormatsAll = new string[]
        {
      DataFormats.Bitmap,
      DataFormats.CommaSeparatedValue,
      DataFormats.Dib,
      DataFormats.Dif,
      DataFormats.EnhancedMetafile,
      DataFormats.FileDrop,
      DataFormats.Html,
      DataFormats.Locale,
      DataFormats.MetafilePict,
      DataFormats.OemText,
      DataFormats.Palette,
      DataFormats.PenData,
      DataFormats.Riff,
      DataFormats.Rtf,
      DataFormats.Serializable,
      DataFormats.StringFormat,
      DataFormats.SymbolicLink,
      DataFormats.Text,
      DataFormats.Tiff,
      DataFormats.UnicodeText,
      DataFormats.WaveAudio
        };
        /*
        protected readonly string[] formatsAllDesc = new String[] 
        {
          "Bitmap",
          "CommaSeparatedValue",
          "Dib",
          "Dif",
          "EnhancedMetafile",
          "FileDrop",
          "Html",
          "Locale",
          "MetafilePict",
          "OemText",
          "Palette",
          "PenData",
          "Riff",
          "Rtf",
          "Serializable",
          "StringFormat",
          "SymbolicLink",
          "Text",
          "Tiff",
          "UnicodeText",
          "WaveAudio"
        };
        */
        #endregion // Clipboard Formats

        protected ClipMonitorHook _hook;
        #endregion // Fields

        #region Constructors

        public MainForm()
        {
            InitializeComponent();
        }
        #endregion // Constructors

        #region Public Methods
        #endregion // Public Methods

        #region Protected Methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                Disposer.SafeDispose(ref _hook);
            }
            base.Dispose(disposing);
        }
        #endregion // Protected Methods

        #region Private Methods

        /// <summary>
        /// Build a menu listing the formats supported by the contents of the clipboard
        /// </summary>
        /// <param name="iData">The current clipboard data object</param>
        private void FormatMenuBuild(IDataObject iData)
        {
            string[] astrFormatsNative = iData.GetFormats(false);
            string[] astrFormatsAll = iData.GetFormats(true);

            Hashtable formatList = new(10);

            _menuItemCurrentFormats.DropDownItems.Clear(); // Use DropDownItems instead of MenuItems

            for (int i = 0; i <= astrFormatsAll.GetUpperBound(0); i++)
            {
                formatList.Add(astrFormatsAll[i], "Converted");
            }

            for (int i = 0; i <= astrFormatsNative.GetUpperBound(0); i++)
            {
                if (formatList.Contains(astrFormatsNative[i]))
                {
                    formatList[astrFormatsNative[i]] = "Native/Converted";
                }
                else
                {
                    formatList.Add(astrFormatsNative[i], "Native");
                }
            }

            foreach (DictionaryEntry item in formatList)
            {
                // Create a ToolStripMenuItem instead of MenuItem
                ToolStripMenuItem itmNew = new(item.Key.ToString() + "\t" + item.Value.ToString());
                _menuItemCurrentFormats.DropDownItems.Add(itmNew); // Use DropDownItems instead of MenuItems
            }
        }


        /// <summary>
        /// list the formats that are supported from the default clipboard formats.
        /// </summary>
        /// <param name="iData">The current clipboard contents</param>
        private void SupportedMenuBuild(IDataObject iData)
        {
            _menuItemSupportedFormats.DropDownItems.Clear(); // Use DropDownItems instead of MenuItems

            for (int i = 0; i <= _arrFormatsAll.GetUpperBound(0); i++)
            {
                string strFormat = _arrFormatsAll[i];

                // Create a ToolStripMenuItem instead of MenuItem
                ToolStripMenuItem itmFormat = new(strFormat);

                // Get supported formats
                if (iData.GetDataPresent(strFormat))
                {
                    itmFormat.Checked = true;
                }

                _menuItemSupportedFormats.DropDownItems.Add(itmFormat); // Use DropDownItems instead of MenuItems
            }
        }


        private System.Drawing.Image GetImageFromNativeClipboard()
        {
            System.Drawing.Image img = null;
            User32.OpenClipboard(this.Handle);
            IntPtr hMfPict = User32.GetClipboardData((uint)Gdi32.CLIPFORMAT.CF_ENHMETAFILE);

            if (IntPtr.Zero == hMfPict)
            {
                hMfPict = User32.GetClipboardData((uint)Gdi32.CLIPFORMAT.CF_METAFILEPICT);
            }

            if (IntPtr.Zero != hMfPict)
            {
                Metafile metafile = new(hMfPict, true);
                System.Drawing.Bitmap bitmap = new(metafile);

                Gdi32.DeleteEnhMetaFile(hMfPict);
                hMfPict = IntPtr.Zero;
                img = bitmap;
            }
            User32.CloseClipboard();

            return img;
        }

        /// <summary>
        /// Show the clipboard contents in the window 
        /// and show the notification balloon if a link is found
        /// </summary>
        private void GetClipboardData()
        {
            // Data on the clipboard uses the IDataObject interface
            DataObject data = null;
            IDataObject iData = null;
            string[] formats = null;
            System.Drawing.Image img = null;
            bool bOk = false;

            /* test what's on native clipboard
            foreach (Gdi32.CLIPFORMAT cf in PK.PkUtils.Reflection.ReflectionUtils.GetEnumValues<Gdi32.CLIPFORMAT>())
            {
              User32.OpenClipboard(this.Handle);
              IntPtr hData = User32.GetClipboardData((uint)cf);
              User32.CloseClipboard();

              if (hData != IntPtr.Zero)
              {
                hData = IntPtr.Zero;
              }
            }
            */

            try
            {
                iData = Clipboard.GetDataObject();
                data = iData as DataObject;
                if (data != null)
                {
                    formats = data.GetFormats();
                }
            }
            catch (System.Runtime.InteropServices.ExternalException externEx)
            {
                // Copying a field definition in Access 2002 causes this sometimes?
                Debug.WriteLine("InteropServices.ExternalException: {0}", externEx.Message);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            // Get RTF if it is present
            if (iData.GetDataPresent(DataFormats.Rtf))
            {
                _ctlClipboardText.Rtf = (string)iData.GetData(DataFormats.Rtf);
                _ctlClipboardText.Show();
                _pictureBox.Hide();
                bOk = true;
            }
            else if (iData.GetDataPresent(DataFormats.UnicodeText))
            {
                _ctlClipboardText.Text = (string)iData.GetData(DataFormats.UnicodeText);
                Debug.WriteLine((string)iData.GetData(DataFormats.UnicodeText));
                _ctlClipboardText.Show();
                _pictureBox.Hide();
                bOk = true;
            }
            else if (iData.GetDataPresent(DataFormats.Text))
            {
                _ctlClipboardText.Text = (string)iData.GetData(DataFormats.Text);
                Debug.WriteLine((string)iData.GetData(DataFormats.Text));
                _ctlClipboardText.Show();
                _pictureBox.Hide();
                bOk = true;
            }
            else if (iData.GetDataPresent(DataFormats.Bitmap))
            {
                System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)iData.GetData(DataFormats.Bitmap);
                _pictureBox.Image = bitmap;
                _ctlClipboardText.Hide();
                _pictureBox.Show();
                bOk = true;
            }
            else if (iData.GetDataPresent(DataFormats.Dib))
            {
                System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)iData.GetData(DataFormats.Bitmap);
                _pictureBox.Image = bitmap;
                _ctlClipboardText.Hide();
                _pictureBox.Show();
                bOk = true;
            }
            else if (iData.GetDataPresent(DataFormats.MetafilePict))
            {
                // See Getting CF_ENHMETAFILE from clipboard
                // http://bytes.com/topic/c-sharp/answers/496688-getting-cf_enhmetafile-clipboard
                // 
                // See also ClipView.cs © 2001 by Charles Petzold
                // ftp://ftp.charlespetzold.com/ProgWinCS/Clip%20Drag%20and%20Drop/ClipView/ClipView.cs

                if (null != data)
                {
                    img = data.GetImage();
                }
                if (null == img)
                {
                    img = GetImageFromNativeClipboard();
                }

                if (null != img)
                {
                    System.Drawing.Bitmap bitmap = new(img);
                    _pictureBox.Image = bitmap;
                    bOk = true;
                }

                if (bOk)
                {
                    _ctlClipboardText.Hide();
                    _pictureBox.Show();
                }
            }
            else if (iData.GetDataPresent(DataFormats.EnhancedMetafile))
            {
                if (null != data)
                {
                    img = data.GetImage();
                }
                if (null == img)
                {
                    img = GetImageFromNativeClipboard();
                }

                if (null != img)
                {
                    System.Drawing.Bitmap bitmap = new(img);
                    _pictureBox.Image = bitmap;
                    bOk = true;
                }

                if (bOk)
                {
                    _ctlClipboardText.Hide();
                    _pictureBox.Show();
                }
            }

            if (!bOk)
            {
                string[] arrFormats = iData.GetFormats();
                StringBuilder sb = new();

                if (arrFormats.Length == 0)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "Currently there are no data in clipboard");
                }
                else
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture,
                      "Currently there are {0} clipboard data formats available:{1}",
                      arrFormats.Length, Environment.NewLine);
                    foreach (string str in arrFormats)
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture,
                          "  {0} {1}", str, Environment.NewLine);
                    }
                    sb.AppendFormat(CultureInfo.InvariantCulture,
                      "<this utility does not support displaying any of these format(s)>{0}", Environment.NewLine);
                }


                // Only show previously handled formats
                _ctlClipboardText.Text = sb.ToString();
                _ctlClipboardText.Show();
                _pictureBox.Hide();
            }

            FormatMenuBuild(iData);
            SupportedMenuBuild(iData);
        }
        #endregion // Private Methods

        #region Event Handlers

        private void itmExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMain_Load(object sender, System.EventArgs e)
        {
            if (null == _hook)
            {
                _hook = new ClipMonitorHook(this);
                _hook.EventClipboardChanged += new EventHandler(OnEventClipboardChanged);
            }
            GetClipboardData();
        }

        private void OnEventClipboardChanged(object sender, EventArgs e)
        {
            GetClipboardData();
        }
        #endregion // Event Handlers
    }
}
