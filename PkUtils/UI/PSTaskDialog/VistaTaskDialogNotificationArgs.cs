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

// Ignore Spelling: Utils, hyperlink
// 

//------------------------------------------------------------------
// <summary>
// A P/Invoke wrapper for TaskDialog. Usability was given preference to performance and size.
// </summary>
//
// <remarks/>
//------------------------------------------------------------------

namespace PK.PkUtils.UI.PSTaskDialog;

/// <summary>
/// Arguments passed to the TaskDialog callback.
/// </summary>
public class VistaTaskDialogNotificationArgs
{
    /// <summary>
    /// What the TaskDialog callback is a notification of.
    /// </summary>
    private VistaTaskDialogNotification notification;

    /// <summary>
    /// The button ID if the notification is about a button. This a DialogResult
    /// value or the ButtonID member of a TaskDialogButton set in the
    /// TaskDialog.Buttons or TaskDialog.RadioButtons members.
    /// </summary>
    private int buttonId;

    /// <summary>
    /// The HREF string of the hyperlink the notification is about.
    /// </summary>
    private string hyperlink;

    /// <summary>
    /// The number of milliseconds since the dialog was opened or the last time the
    /// callback for a timer notification reset the value by returning true.
    /// </summary>
    private uint timerTickCount;

    /// <summary>
    /// The state of the verification flag when the notification is about the verification flag.
    /// </summary>
    private bool verificationFlagChecked;

    /// <summary>
    /// The state of the dialog expando when the notification is about the expando.
    /// </summary>
    private bool expanded;

    /// <summary>
    /// What the TaskDialog callback is a notification of.
    /// </summary>
    public VistaTaskDialogNotification Notification
    {
        get { return this.notification; }
        set { this.notification = value; }
    }

    /// <summary>
    /// The button ID if the notification is about a button. This a DialogResult
    /// value or the ButtonID member of a TaskDialogButton set in the
    /// TaskDialog.Buttons member.
    /// </summary>
    public int ButtonId
    {
        get { return this.buttonId; }
        set { this.buttonId = value; }
    }

    /// <summary>
    /// The HREF string of the hyperlink the notification is about.
    /// </summary>
    public string Hyperlink
    {
        get { return this.hyperlink; }
        set { this.hyperlink = value; }
    }

    /// <summary>
    /// The number of milliseconds since the dialog was opened or the last time the
    /// callback for a timer notification reset the value by returning true.
    /// </summary>
    public uint TimerTickCount
    {
        get { return this.timerTickCount; }
        set { this.timerTickCount = value; }
    }

    /// <summary>
    /// The state of the verification flag when the notification is about the verification flag.
    /// </summary>
    public bool VerificationFlagChecked
    {
        get { return this.verificationFlagChecked; }
        set { this.verificationFlagChecked = value; }
    }

    /// <summary>
    /// The state of the dialog expando when the notification is about the expando.
    /// </summary>
    public bool Expanded
    {
        get { return this.expanded; }
        set { this.expanded = value; }
    }
}
