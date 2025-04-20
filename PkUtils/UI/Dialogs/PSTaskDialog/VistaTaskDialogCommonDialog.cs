///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// The Code Project Open License Notice
// 
// This software is a Derivative Work based upon a Code Project article
// Vista TaskDialog Wrapper and Emulator
// http://www.codeproject.com/Articles/21276/Vista-TaskDialog-Wrapper-and-Emulator
//
// The Code Project Open License (CPOL) text is available at
// http://www.codeproject.com/info/cpol10.aspx
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Ignore Spelling: Utils
//
using System;
using System.Windows.Forms;

//------------------------------------------------------------------
// <summary>
// A P/Invoke wrapper for TaskDialog. Usability was given preference to perf and size.
// </summary>
//
// <remarks/>
//------------------------------------------------------------------

namespace PK.PkUtils.UI.Dialogs.PSTaskDialog;

/// <summary>
/// TaskDialog wrapped in a CommonDialog class. This is required to work well in
/// MMC 3.0. In MMC 3.0 you must use the ShowDialog methods on the MMC classes to
/// correctly show a modal dialog. This class will allow you to do this and keep access
/// to the results of the TaskDialog.
/// </summary>
public class VistaTaskDialogCommonDialog : CommonDialog
{
    /// <summary>
    /// The TaskDialog we will display.
    /// </summary>
    private readonly VistaTaskDialog taskDialog;

    /// <summary>
    /// The result of the dialog, either a DialogResult value for common push _buttons set in the TaskDialog.CommonButtons
    /// member or the ButtonID from a TaskDialogButton structure set on the TaskDialog.Buttons member.
    /// </summary>
    private int taskDialogResult;

    /// <summary>
    /// The verification flag result of the dialog. True if the verification checkbox was checked when the dialog
    /// was dismissed.
    /// </summary>
    private bool verificationFlagCheckedResult;

    /// <summary>
    /// TaskDialog wrapped in a CommonDialog class. THis is required to work well in
    /// MMC 2.1. In MMC 2.1 you must use the ShowDialog methods on the MMC classes to
    /// correctly show a modal dialog. This class will allow you to do this and keep access
    /// to the results of the TaskDialog.
    /// </summary>
    /// <param name="taskDialog">The TaskDialog to show.</param>
    public VistaTaskDialogCommonDialog(VistaTaskDialog taskDialog)
    {
        ArgumentNullException.ThrowIfNull(taskDialog);
        this.taskDialog = taskDialog;
    }

    /// <summary>
    /// The TaskDialog to show.
    /// </summary>
    public VistaTaskDialog TaskDialog
    {
        get { return taskDialog; }
    }

    /// <summary>
    /// The result of the dialog, either a DialogResult value for common push _buttons set in the TaskDialog.CommonButtons
    /// member or the ButtonID from a TaskDialogButton structure set on the TaskDialog.Buttons member.
    /// </summary>
    public int TaskDialogResult
    {
        get { return taskDialogResult; }
    }

    /// <summary>
    /// The verification flag result of the dialog. True if the verification checkbox was checked when the dialog
    /// was dismissed.
    /// </summary>
    public bool VerificationFlagCheckedResult
    {
        get { return verificationFlagCheckedResult; }
    }

    /// <summary>
    /// Reset the common dialog.
    /// </summary>
    public override void Reset()
    {
        taskDialog.Reset();
    }

    /// <summary>
    /// The required implementation of CommonDialog that shows the Task Dialog.
    /// </summary>
    /// <param name="hwndOwner">Owner window. This can be null.</param>
    /// <returns>If this method returns true, then ShowDialog will return DialogResult.OK.
    /// If this method returns false, then ShowDialog will return DialogResult.Cancel. The
    /// user of this class must use the TaskDialogResult member to get more information.
    /// </returns>
    protected override bool RunDialog(nint hwndOwner)
    {
        taskDialogResult = taskDialog.Show(hwndOwner, out verificationFlagCheckedResult);
        return taskDialogResult != (int)DialogResult.Cancel;
    }
}
