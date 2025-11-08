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
using System.Collections.Generic;
using System.Windows.Forms;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.UI.Dialogs.PSTaskDialog;

#region Typedefs

/// <summary> Values that represent SystemIconType. </summary>
public enum SystemIconType
{
    /// <summary> An enum constant representing the information option. </summary>
    Information,
    /// <summary> An enum constant representing the question option. </summary>
    Question,
    /// <summary> An enum constant representing the warning option. </summary>
    Warning,
    /// <summary> An enum constant representing the error option. </summary>
    Error
};

/// <summary> Values that represent TaskDialogButtons. </summary>
public enum TaskDialogButtons
{
    /// <summary> An enum constant representing the yes no option. </summary>
    YesNo,
    /// <summary> An enum constant representing the yes no cancel option. </summary>
    YesNoCancel,
    /// <summary> An enum constant representing the ok cancel option. </summary>
    OKCancel,
    /// <summary> An enum constant representing the ok option. </summary>
    OK,
    /// <summary> An enum constant representing the close option. </summary>
    Close,
    /// <summary> An enum constant representing the cancel option. </summary>
    Cancel,
    /// <summary> An enum constant representing the none option. </summary>
    None
}
#endregion // Typedefs

/// <summary> Dialog for setting the task. </summary>
public static class VistaTaskDialogManager
{
    #region Fields
    #region Public Static Fields

    /// <summary> Event queue for all listeners interested in OnTaskDialogShown events. </summary>
    public static event EventHandler<EventArgs> OnTaskDialogShown;
    /// <summary> Event queue for all listeners interested in OnTaskDialogClosed events. </summary>
    public static event EventHandler<EventArgs> OnTaskDialogClosed;
    #endregion // Public Static Fields

    #region Private Static Fields
    /// <summary>
    /// PetrK 04/05/2012: refactoring - following were changed from public to private, with appropriate property
    /// implemented.
    /// </summary>
    private static bool _VerificationChecked;
    /// <summary> true to enable force emulation mode, false to disable it. </summary>
    private static bool _ForceEmulationMode;
    /// <summary> The radio button result. </summary>
    private static int _RadioButtonResult = -1;
    /// <summary> The command button result. </summary>
    private static int _CommandButtonResult = -1;
    /// <summary> Width of the emulated form. </summary>
    private static int _EmulatedFormWidth = 450;
    /// <summary> The use tool window on XP. </summary>
    private static bool _UseToolWindowOnXP = true;
    /// <summary> The play system sounds. </summary>
    private static bool _PlaySystemSounds = true;
    #endregion // Private Static Fields
    #endregion // Fields

    #region Properties

    #region Dialog_results_related

    /// <summary> all dialog-results related properties should have private setter. </summary>
    ///
    /// <value> true if verification checked, false if not. </value>
    public static bool VerificationChecked
    {
        get { return _VerificationChecked; }
        private set { _VerificationChecked = value; }
    }

    /// <summary> Gets or sets the radio button result. </summary>
    ///
    /// <value> The radio button result. </value>
    public static int RadioButtonResult
    {
        get { return _RadioButtonResult; }
        private set { _RadioButtonResult = value; }
    }

    /// <summary> Gets or sets the command button result. </summary>
    ///
    /// <value> The command button result. </value>
    public static int CommandButtonResult
    {
        get { return _CommandButtonResult; }
        private set { _CommandButtonResult = value; }
    }
    #endregion // Dialog_results_related

    #region Emulation_related
    /// <summary> all emulation-related properties should have public setter. </summary>
    ///
    /// <value> The width of the emulated form. </value>

    public static int EmulatedFormWidth
    {
        get { return _EmulatedFormWidth; }
        set { _EmulatedFormWidth = value; }
    }

    /// <summary> Gets or sets a value indicating whether the emulation mode should be forced. </summary>
    ///
    /// <value> true if force emulation mode, false if not. </value>
    public static bool ForceEmulationMode
    {
        get { return _ForceEmulationMode; }
        set { _ForceEmulationMode = value; }
    }

    /// <summary> Gets or sets a value indicating whether this object use tool window on XP. </summary>
    ///
    /// <value> true if use tool window on xp, false if not. </value>
    public static bool UseToolWindowOnXP
    {
        get { return _UseToolWindowOnXP; }
        set { _UseToolWindowOnXP = value; }
    }

    /// <summary> Gets or sets a value indicating whether the play system sounds. </summary>
    ///
    /// <value> true if play system sounds, false if not. </value>
    public static bool PlaySystemSounds
    {
        get { return _PlaySystemSounds; }
        set { _PlaySystemSounds = value; }
    }
    #endregion // Emulation_related
    #endregion // Properties

    #region Methods

    #region ShowTaskDialogBox

    /// <summary> ShowTaskDialogBox with all arguments availabke. </summary>
    ///
    /// <param name="Owner">            The owner. </param>
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="ExpandedInfo">     Information describing the expanded. </param>
    /// <param name="Footer">           The footer. </param>
    /// <param name="VerificationText"> The verification text. </param>
    /// <param name="RadioButtons">     The radio buttons. </param>
    /// <param name="CommandButtons">   The command buttons. </param>
    /// <param name="Buttons">          The buttons. </param>
    /// <param name="MainIcon">         The main icon. </param>
    /// <param name="FooterIcon">       The footer icon. </param>
    /// <param name="DefaultIndex">     The default index. </param>
    ///
    /// <returns> . </returns>
    public static DialogResult ShowTaskDialogBox(
        IWin32Window Owner,
        string Title,
        string MainInstruction,
        string Content,
        string ExpandedInfo,
        string Footer,
        string VerificationText,
        string RadioButtons,
        string CommandButtons,
        TaskDialogButtons Buttons,
        SystemIconType MainIcon,
        SystemIconType FooterIcon,
        int DefaultIndex)
    {
        DialogResult result;
        OnTaskDialogShown?.Invoke(null, EventArgs.Empty);

        if (VistaTaskDialog.IsAvailableOnThisOS && !ForceEmulationMode)
        {
            // [OPTION 1] Show Vista TaskDialog
            VistaTaskDialog vtd = new()
            {
                WindowTitle = Title,
                MainInstruction = MainInstruction,
                Content = Content,
                ExpandedInformation = ExpandedInfo,
                Footer = Footer
            };

            // Radio Buttons
            if (!string.IsNullOrEmpty(RadioButtons))
            {
                List<VistaTaskDialogButton> lst = [];
                string[] arr = RadioButtons.Split(['|']);
                for (int i = 0; i < arr.Length; i++)
                {
                    try
                    {
                        VistaTaskDialogButton button = new()
                        {
                            ButtonId = 1000 + i,
                            ButtonText = arr[i]
                        };
                        lst.Add(button);
                    }
                    catch (FormatException)
                    {
                    }
                }
                vtd.RadioButtons = lst.ToArray();
                vtd.NoDefaultRadioButton = DefaultIndex == -1;
                if (DefaultIndex >= 0)
                    vtd.DefaultRadioButton = DefaultIndex;
            }

            // Custom Buttons
            if (!string.IsNullOrEmpty(CommandButtons))
            {
                List<VistaTaskDialogButton> lst = [];
                string[] arr = CommandButtons.Split(['|']);
                for (int i = 0; i < arr.Length; i++)
                {
                    try
                    {
                        VistaTaskDialogButton button = new()
                        {
                            ButtonId = 2000 + i,
                            ButtonText = arr[i]
                        };
                        lst.Add(button);
                    }
                    catch (FormatException)
                    {
                    }
                }
                vtd.Buttons = lst.ToArray();
                if (DefaultIndex >= 0)
                    vtd.DefaultButton = DefaultIndex;
            }

            vtd.CommonButtons = Buttons switch
            {
                TaskDialogButtons.YesNo => VistaTaskDialogCommonButtons.Yes | VistaTaskDialogCommonButtons.No,
                TaskDialogButtons.YesNoCancel => VistaTaskDialogCommonButtons.Yes | VistaTaskDialogCommonButtons.No | VistaTaskDialogCommonButtons.Cancel,
                TaskDialogButtons.OKCancel => VistaTaskDialogCommonButtons.Ok | VistaTaskDialogCommonButtons.Cancel,
                TaskDialogButtons.OK => VistaTaskDialogCommonButtons.Ok,
                TaskDialogButtons.Close => VistaTaskDialogCommonButtons.Close,
                TaskDialogButtons.Cancel => VistaTaskDialogCommonButtons.Cancel,
                _ => 0,
            };
            switch (MainIcon)
            {
                case SystemIconType.Information: vtd.MainIcon = VistaTaskDialogIcon.Information; break;
                case SystemIconType.Question: vtd.MainIcon = VistaTaskDialogIcon.Information; break;
                case SystemIconType.Warning: vtd.MainIcon = VistaTaskDialogIcon.Warning; break;
                case SystemIconType.Error: vtd.MainIcon = VistaTaskDialogIcon.Error; break;
            }

            switch (FooterIcon)
            {
                case SystemIconType.Information: vtd.FooterIcon = VistaTaskDialogIcon.Information; break;
                case SystemIconType.Question: vtd.FooterIcon = VistaTaskDialogIcon.Information; break;
                case SystemIconType.Warning: vtd.FooterIcon = VistaTaskDialogIcon.Warning; break;
                case SystemIconType.Error: vtd.FooterIcon = VistaTaskDialogIcon.Error; break;
            }

            vtd.EnableHyperlinks = false;
            vtd.ShowProgressBar = false;
            vtd.AllowDialogCancellation = Buttons == TaskDialogButtons.Cancel ||
                                           Buttons == TaskDialogButtons.Close ||
                                           Buttons == TaskDialogButtons.OKCancel ||
                                           Buttons == TaskDialogButtons.YesNoCancel;
            vtd.CallbackTimer = false;
            vtd.ExpandedByDefault = false;
            vtd.ExpandFooterArea = false;
            vtd.PositionRelativeToWindow = true;
            vtd.RightToLeftLayout = false;
            vtd.NoDefaultRadioButton = false;
            vtd.CanBeMinimized = false;
            vtd.ShowMarqueeProgressBar = false;
            vtd.UseCommandLinks = !string.IsNullOrEmpty(CommandButtons);
            vtd.UseCommandLinksNoIcon = false;
            vtd.VerificationText = VerificationText;
            vtd.VerificationFlagChecked = false;
            vtd.ExpandedControlText = "Hide details";
            vtd.CollapsedControlText = "Show details";
            vtd.Callback = null;

            // Show the Dialog
            result = (DialogResult)vtd.Show(vtd.CanBeMinimized ? null : Owner, out _VerificationChecked, out _RadioButtonResult);

            // if a command button was clicked, then change return result
            // to "DialogResult.OK" and set the CommandButtonResult
            if ((int)result >= 2000)
            {
                CommandButtonResult = (int)result - 2000;
                result = DialogResult.OK;
            }
            if (RadioButtonResult >= 1000)
                RadioButtonResult -= 1000;  // deduct the ButtonID start value for radio _buttons
        }
        else
        {
            // [OPTION 2] Show Emulated Form
            using VistaTaskDialogEmulator td = new();
            td.Title = Title;
            td.MainInstruction = MainInstruction;
            td.Content = Content;
            td.ExpandedInfo = ExpandedInfo;
            td.Footer = Footer;
            td.RadioButtons = RadioButtons;
            td.CommandButtons = CommandButtons;
            td.Buttons = Buttons;
            td.MainIcon = MainIcon;
            td.FooterIcon = FooterIcon;
            td.VerificationText = VerificationText;
            td.Width = EmulatedFormWidth;
            td.DefaultButtonIndex = DefaultIndex;
            td.BuildForm();
            result = td.ShowDialog(Owner);

            RadioButtonResult = td.RadioButtonIndex;
            CommandButtonResult = td.CommandButtonClickedIndex;
            VerificationChecked = td.VerificationCheckBoxChecked;
        }

        OnTaskDialogClosed?.Invoke(null, EventArgs.Empty);
        return result;
    }

    /// <summary> ShowTaskDialogBox overload. </summary>
    ///
    /// <param name="Owner">            The owner. </param>
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="ExpandedInfo">     Information describing the expanded. </param>
    /// <param name="Footer">           The footer. </param>
    /// <param name="VerificationText"> The verification text. </param>
    /// <param name="RadioButtons">     The radio buttons. </param>
    /// <param name="CommandButtons">   The command buttons. </param>
    /// <param name="Buttons">          The buttons. </param>
    /// <param name="MainIcon">         The main icon. </param>
    /// <param name="FooterIcon">       The footer icon. </param>
    ///
    /// <returns> . </returns>
    public static DialogResult ShowTaskDialogBox(
      IWin32Window Owner,
      string Title,
      string MainInstruction,
      string Content,
      string ExpandedInfo,
      string Footer,
      string VerificationText,
      string RadioButtons,
      string CommandButtons,
      TaskDialogButtons Buttons,
      SystemIconType MainIcon,
      SystemIconType FooterIcon)
    {
        return ShowTaskDialogBox(Owner, Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText, RadioButtons, CommandButtons, Buttons, MainIcon, FooterIcon, 0);
    }

    /// <summary> ShowTaskDialogBox overload. </summary>
    ///
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="ExpandedInfo">     Information describing the expanded. </param>
    /// <param name="Footer">           The footer. </param>
    /// <param name="VerificationText"> The verification text. </param>
    /// <param name="RadioButtons">     The radio buttons. </param>
    /// <param name="CommandButtons">   The command buttons. </param>
    /// <param name="Buttons">          The buttons. </param>
    /// <param name="MainIcon">         The main icon. </param>
    /// <param name="FooterIcon">       The footer icon. </param>
    ///
    /// <returns> . </returns>
    public static DialogResult ShowTaskDialogBox(
      string Title,
      string MainInstruction,
      string Content,
      string ExpandedInfo,
      string Footer,
      string VerificationText,
      string RadioButtons,
      string CommandButtons,
      TaskDialogButtons Buttons,
      SystemIconType MainIcon,
      SystemIconType FooterIcon)
    {
        return ShowTaskDialogBox(null, Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText, RadioButtons, CommandButtons, Buttons, MainIcon, FooterIcon, 0);
    }
    #endregion // ShowTaskDialogBox

    #region MessageBox

    /// <summary> Message box. </summary>
    ///
    /// <param name="Owner">            The owner. </param>
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="ExpandedInfo">     Information describing the expanded. </param>
    /// <param name="Footer">           The footer. </param>
    /// <param name="VerificationText"> The verification text. </param>
    /// <param name="Buttons">          The buttons. </param>
    /// <param name="MainIcon">         The main icon. </param>
    /// <param name="FooterIcon">       The footer icon. </param>
    ///
    /// <returns> . </returns>
    public static DialogResult MessageBox(
      IWin32Window Owner,
      string Title,
      string MainInstruction,
      string Content,
      string ExpandedInfo,
      string Footer,
      string VerificationText,
      TaskDialogButtons Buttons,
      SystemIconType MainIcon,
      SystemIconType FooterIcon)
    {
        return ShowTaskDialogBox(Owner, Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText, "", "", Buttons, MainIcon, FooterIcon);
    }

    /// <summary>
    /// --------------------------------------------------------------------------------
    ///  Overloaded versions...
    /// --------------------------------------------------------------------------------.
    /// </summary>
    ///
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="ExpandedInfo">     Information describing the expanded. </param>
    /// <param name="Footer">           The footer. </param>
    /// <param name="VerificationText"> The verification text. </param>
    /// <param name="Buttons">          The buttons. </param>
    /// <param name="MainIcon">         The main icon. </param>
    /// <param name="FooterIcon">       The footer icon. </param>
    ///
    /// <returns> . </returns>
    public static DialogResult MessageBox(
      string Title,
      string MainInstruction,
      string Content,
      string ExpandedInfo,
      string Footer,
      string VerificationText,
      TaskDialogButtons Buttons,
      SystemIconType MainIcon,
      SystemIconType FooterIcon)
    {
        return ShowTaskDialogBox(null, Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText, "", "", Buttons, MainIcon, FooterIcon);
    }

    /// <summary>
    /// -------------------------------------------------------------------------------- Overloaded versions...
    /// --------------------------------------------------------------------------------.
    /// </summary>
    ///
    /// <param name="Owner">            The owner. </param>
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="Buttons">          The buttons. </param>
    /// <param name="MainIcon">         The main icon. </param>
    ///
    /// <returns> . </returns>
    public static DialogResult MessageBox(
      IWin32Window Owner,
      string Title,
      string MainInstruction,
      string Content,
      TaskDialogButtons Buttons,
      SystemIconType MainIcon)
    {
        return MessageBox(Owner, Title, MainInstruction, Content, "", "", "", Buttons, MainIcon, SystemIconType.Information);
    }

    /// <summary>
    /// -------------------------------------------------------------------------------- Overloaded versions...
    /// --------------------------------------------------------------------------------.
    /// </summary>
    ///
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="Buttons">          The buttons. </param>
    /// <param name="MainIcon">         The main icon. </param>
    ///
    /// <returns> . </returns>
    public static DialogResult MessageBox(
      string Title,
      string MainInstruction,
      string Content,
      TaskDialogButtons Buttons,
      SystemIconType MainIcon)
    {
        return MessageBox(null, Title, MainInstruction, Content, "", "", "", Buttons, MainIcon, SystemIconType.Information);
    }
    #endregion // MessageBox

    #region ShowRadioBox

    /// <summary> Shows the radio box. </summary>
    ///
    /// <param name="Owner">            The owner. </param>
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="ExpandedInfo">     Information describing the expanded. </param>
    /// <param name="Footer">           The footer. </param>
    /// <param name="VerificationText"> The verification text. </param>
    /// <param name="RadioButtons">     The radio buttons. </param>
    /// <param name="MainIcon">         The main icon. </param>
    /// <param name="FooterIcon">       The footer icon. </param>
    /// <param name="DefaultIndex">     The default index. </param>
    ///
    /// <returns> . </returns>
    public static int ShowRadioBox(
      IWin32Window Owner,
      string Title,
      string MainInstruction,
      string Content,
      string ExpandedInfo,
      string Footer,
      string VerificationText,
      string RadioButtons,
      SystemIconType MainIcon,
      SystemIconType FooterIcon,
      int DefaultIndex)
    {
        DialogResult res = ShowTaskDialogBox(Owner, Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText,
                                             RadioButtons, "", TaskDialogButtons.OKCancel, MainIcon, FooterIcon, DefaultIndex);
        if (res == DialogResult.OK)
            return RadioButtonResult;
        else
            return -1;
    }

    /// <summary>
    /// --------------------------------------------------------------------------------
    ///  Overloaded versions...
    /// --------------------------------------------------------------------------------.
    /// </summary>
    ///
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="ExpandedInfo">     Information describing the expanded. </param>
    /// <param name="Footer">           The footer. </param>
    /// <param name="VerificationText"> The verification text. </param>
    /// <param name="RadioButtons">     The radio buttons. </param>
    /// <param name="MainIcon">         The main icon. </param>
    /// <param name="FooterIcon">       The footer icon. </param>
    /// <param name="DefaultIndex">     The default index. </param>
    ///
    /// <returns> . </returns>
    public static int ShowRadioBox(
      string Title,
      string MainInstruction,
      string Content,
      string ExpandedInfo,
      string Footer,
      string VerificationText,
      string RadioButtons,
      SystemIconType MainIcon,
      SystemIconType FooterIcon,
      int DefaultIndex)
    {
        DialogResult res = ShowTaskDialogBox(null, Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText,
                                             RadioButtons, "", TaskDialogButtons.OKCancel, MainIcon, FooterIcon, DefaultIndex);
        if (res == DialogResult.OK)
            return RadioButtonResult;
        else
            return -1;
    }

    /// <summary>
    /// -------------------------------------------------------------------------------- Overloaded versions...
    /// --------------------------------------------------------------------------------.
    /// </summary>
    ///
    /// <param name="Owner">            The owner. </param>
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="ExpandedInfo">     Information describing the expanded. </param>
    /// <param name="Footer">           The footer. </param>
    /// <param name="VerificationText"> The verification text. </param>
    /// <param name="RadioButtons">     The radio buttons. </param>
    /// <param name="MainIcon">         The main icon. </param>
    /// <param name="FooterIcon">       The footer icon. </param>
    ///
    /// <returns> . </returns>
    public static int ShowRadioBox(
      IWin32Window Owner,
      string Title,
      string MainInstruction,
      string Content,
      string ExpandedInfo,
      string Footer,
      string VerificationText,
      string RadioButtons,
      SystemIconType MainIcon,
      SystemIconType FooterIcon)
    {
        return ShowRadioBox(Owner, Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText, RadioButtons, MainIcon, FooterIcon, 0);
    }

    /// <summary>
    /// -------------------------------------------------------------------------------- Overloaded versions...
    /// --------------------------------------------------------------------------------.
    /// </summary>
    ///
    /// <param name="Owner">            The owner. </param>
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="RadioButtons">     The radio buttons. </param>
    /// <param name="DefaultIndex">     The default index. </param>
    ///
    /// <returns> . </returns>
    public static int ShowRadioBox(
      IWin32Window Owner,
      string Title,
      string MainInstruction,
      string Content,
      string RadioButtons,
      int DefaultIndex)
    {
        return ShowRadioBox(Owner, Title, MainInstruction, Content, "", "", "", RadioButtons, SystemIconType.Question, SystemIconType.Information, DefaultIndex);
    }

    /// <summary>
    /// -------------------------------------------------------------------------------- Overloaded versions...
    /// --------------------------------------------------------------------------------.
    /// </summary>
    ///
    /// <param name="Owner">            The owner. </param>
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="RadioButtons">     The radio buttons. </param>
    ///
    /// <returns> . </returns>
    public static int ShowRadioBox(
      IWin32Window Owner,
      string Title,
      string MainInstruction,
      string Content,
      string RadioButtons)
    {
        return ShowRadioBox(Owner, Title, MainInstruction, Content, "", "", "", RadioButtons, SystemIconType.Question, SystemIconType.Information, 0);
    }

    /// <summary>
    /// -------------------------------------------------------------------------------- Overloaded versions...
    /// --------------------------------------------------------------------------------.
    /// </summary>
    ///
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="RadioButtons">     The radio buttons. </param>
    ///
    /// <returns> . </returns>
    public static int ShowRadioBox(
      string Title,
      string MainInstruction,
      string Content,
      string RadioButtons)
    {
        return ShowRadioBox(null, Title, MainInstruction, Content, "", "", "", RadioButtons, SystemIconType.Question, SystemIconType.Information, 0);
    }
    #endregion // ShowRadioBox

    #region ShowCommandBox

    /// <summary> Shows the command box. </summary>
    ///
    /// <param name="Owner">            The owner. </param>
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="ExpandedInfo">     Information describing the expanded. </param>
    /// <param name="Footer">           The footer. </param>
    /// <param name="VerificationText"> The verification text. </param>
    /// <param name="CommandButtons">   The command buttons. </param>
    /// <param name="ShowCancelButton"> true to show, false to hide the cancel button. </param>
    /// <param name="MainIcon">         The main icon. </param>
    /// <param name="FooterIcon">       The footer icon. </param>
    ///
    /// <returns> . </returns>
    public static int ShowCommandBox(
      IWin32Window Owner,
      string Title,
      string MainInstruction,
      string Content,
      string ExpandedInfo,
      string Footer,
      string VerificationText,
      string CommandButtons,
      bool ShowCancelButton,
      SystemIconType MainIcon,
      SystemIconType FooterIcon)
    {
        DialogResult res = ShowTaskDialogBox(Owner, Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText,
                                             "", CommandButtons, ShowCancelButton ? TaskDialogButtons.Cancel : TaskDialogButtons.None,
                                             MainIcon, FooterIcon);
        if (res == DialogResult.OK)
            return CommandButtonResult;
        else
            return -1;
    }

    /// <summary> Shows the command box. </summary>
    ///
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="ExpandedInfo">     Information describing the expanded. </param>
    /// <param name="Footer">           The footer. </param>
    /// <param name="VerificationText"> The verification text. </param>
    /// <param name="CommandButtons">   The command buttons. </param>
    /// <param name="ShowCancelButton"> true to show, false to hide the cancel button. </param>
    /// <param name="MainIcon">         The main icon. </param>
    /// <param name="FooterIcon">       The footer icon. </param>
    ///
    /// <returns> . </returns>
    public static int ShowCommandBox(
      string Title,
      string MainInstruction,
      string Content,
      string ExpandedInfo,
      string Footer,
      string VerificationText,
      string CommandButtons,
      bool ShowCancelButton,
      SystemIconType MainIcon,
      SystemIconType FooterIcon)
    {
        DialogResult res = ShowTaskDialogBox(null, Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText,
                                             "", CommandButtons, ShowCancelButton ? TaskDialogButtons.Cancel : TaskDialogButtons.None,
                                             MainIcon, FooterIcon);
        if (res == DialogResult.OK)
            return CommandButtonResult;
        else
            return -1;
    }

    /// <summary> Shows the command box. </summary>
    ///
    /// <param name="Owner">            The owner. </param>
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="CommandButtons">   The command buttons. </param>
    /// <param name="ShowCancelButton"> true to show, false to hide the cancel button. </param>
    ///
    /// <returns> . </returns>
    public static int ShowCommandBox(IWin32Window Owner, string Title, string MainInstruction, string Content, string CommandButtons, bool ShowCancelButton)
    {
        return ShowCommandBox(Owner, Title, MainInstruction, Content, "", "", "", CommandButtons, ShowCancelButton, SystemIconType.Question, SystemIconType.Information);
    }

    /// <summary> Shows the command box. </summary>
    ///
    /// <param name="Title">            The title. </param>
    /// <param name="MainInstruction">  The main instruction. </param>
    /// <param name="Content">          The content. </param>
    /// <param name="CommandButtons">   The command buttons. </param>
    /// <param name="ShowCancelButton"> true to show, false to hide the cancel button. </param>
    ///
    /// <returns> . </returns>
    public static int ShowCommandBox(string Title, string MainInstruction, string Content, string CommandButtons, bool ShowCancelButton)
    {
        return ShowCommandBox(null, Title, MainInstruction, Content, "", "", "", CommandButtons, ShowCancelButton, SystemIconType.Question, SystemIconType.Information);
    }
    #endregion // ShowCommandBox
    #endregion // Methods
}
#pragma warning restore IDE0305