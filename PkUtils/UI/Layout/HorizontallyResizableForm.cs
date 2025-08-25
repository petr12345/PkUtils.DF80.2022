// Ignore Spelling: Utils, resizable
//
using System;
using System.Windows.Forms;
using PK.PkUtils.WinApi;
using static PK.PkUtils.WinApi.User32;
using static PK.PkUtils.WinApi.Win32;

namespace PK.PkUtils.UI.Layout;

/// <summary>
/// A Windows Form that is resizable only horizontally. 
/// Remaps window hit test results to restrict resizing to left and right edges only.
/// Remembers last window placement between sessions.
/// </summary>
public class HorizontallyResizableForm : Form
{
    #region Fields

    /// <summary>
    /// Stores the last window placement information.
    /// </summary>
    private Nullable<WINDOWPLACEMENT> _lastWindowPlacement;

    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public HorizontallyResizableForm()
    {
        // Set some default code to for the form resizable horizontally. 
        // This can be redefined in a derived classes.
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.MinimumSize = new System.Drawing.Size(300, this.MinimumSize.Height); // Set a minimum width

        this.FormClosing += HorizontalyResizableForm_FormClosing;
    }

    /// <summary> Initializes a new instance of the <see cref="HorizontallyResizableForm"/> class. </summary>
    /// <param name="lastWindowPlacement"> The last window placement. Can be null. </param>
    public HorizontallyResizableForm(Nullable<WINDOWPLACEMENT> lastWindowPlacement)
        : this()
    {
        _lastWindowPlacement = lastWindowPlacement;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets the last window placement. </summary>
    public WINDOWPLACEMENT? LastWindowPlacement { get => _lastWindowPlacement; }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Called when the form is loaded. Restores the last window placement if available.
    /// </summary>
    /// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
    protected override void OnLoad(EventArgs args)
    {
        base.OnLoad(args);

        // Restore the last window placement if available.
        // Otherwise, use the default StartPosition behavior.
        if (LastWindowPlacement.HasValue)
        {
            WINDOWPLACEMENT placement = LastWindowPlacement.Value;
            // If the window was minimized, restore it to normal state.  
            // Not sure if this is necessary, i.e. if LastWindowPlacement could ever have such value, but bettwer safe than sorry.
            if (placement.ShowCmd == SW.MINIMIZE)
            {
                placement.ShowCmd = SW.RESTORE;
            }
            SetWindowPlacement(this.Handle, ref placement);
        }
    }

    /// <summary>
    /// Processes Windows messages, customizing hit test results for window resizing edges and corners.
    /// </summary>
    /// <param name="m">The Windows <see cref="Message"/> to process.</param>
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (m.Msg == (int)Win32.WM.WM_NCHITTEST)
        {
            // Remap hit test results:
            // - Corners map to left/right edges
            // - Top/bottom edges are disabled for resizing
            // - Other codes remain unchanged
            m.Result = (IntPtr)((MousePositionCode)(int)m.Result switch
            {
                MousePositionCode.HTTOPLEFT or MousePositionCode.HTBOTTOMLEFT => MousePositionCode.HTLEFT,
                MousePositionCode.HTTOPRIGHT or MousePositionCode.HTBOTTOMRIGHT => MousePositionCode.HTRIGHT,
                MousePositionCode.HTTOP or MousePositionCode.HTBOTTOM => MousePositionCode.HTNOWHERE,
                _ => (MousePositionCode)(int)m.Result
            });
        }
    }

    #endregion // Methods

    #region Event_Handlers

    /// <summary> Handles the FormClosing event to store the current window placement. </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="FormClosingEventArgs"/> that contains the event data.</param>
    private void HorizontalyResizableForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        var placement = new WINDOWPLACEMENT();
        // Try to get the current window placement; store it if successful.
        if (GetWindowPlacement(this.Handle, ref placement))
            _lastWindowPlacement = placement;
        else
            _lastWindowPlacement = null;
    }

    #endregion // Event_Handlers
}
