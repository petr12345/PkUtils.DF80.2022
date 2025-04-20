// Ignore Spelling: Utils, ctrl
//
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PK.PkUtils.UI.Utils;

/// <summary>
/// A static class containing several extension methods related to the design mode
/// </summary>
public static class DesignerSupport
{
    #region Methods

    #region Public Methods

    /// <summary>
    /// A replacement of the property
    /// <code>
    ///   protected bool Component.DesignMode { get; }
    /// </code> </summary>
    ///
    /// <remarks>
    /// Such replacement is necessary for case when you need to figure-out the design mode in
    /// constructor of the control. In that case, the DesignMode still returns false, since it checks
    /// just the Site property, which is still null in the constructor. </remarks>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when the argument <paramref name="ctrl"/> is null. </exception>
    ///
    /// <param name="ctrl"> The WinForms control this method is about.</param>
    ///
    /// <returns> true if design mode, false if not. </returns>
    public static bool IsDesignMode(this Control ctrl)
    {
        bool bRes;
        ArgumentNullException.ThrowIfNull(ctrl);

        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            bRes = true;
        else
            bRes = ctrl.GetDesignModeFromSite();

        return bRes;
    }

    /// <summary> Returns the negation of <see cref="IsDesignMode"/>. </summary>
    ///
    /// <param name="ctrl"> The WinForms control this method is about. </param>
    ///
    /// <returns> true if runtime mode, false if not. </returns>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when the argument <paramref name="ctrl"/> is null. </exception>
    public static bool IsRuntimeMode(this Control ctrl)
    {
        return !ctrl.IsDesignMode();
    }
    #endregion // Public Methods

    #region Private Methods

    /// <summary>
    /// Auxiliary implementation helper; check for the design mode only by inspection
    /// ctrl.Site.DesignMode value. </summary>
    ///
    /// <param name="ctrl"> The WinForms control this method is about. </param>
    ///
    /// <returns> true if design mode, false if not. </returns>
    private static bool GetDesignModeFromSite(this Control ctrl)
    {
        Control parent;
        bool bRes = false;

        if (ctrl != null)
        {
            if (ctrl.Site != null && ctrl.Site.DesignMode)
            {
                bRes = true;
            }
            else if (null != (parent = ctrl.Parent))
            {
                bRes = parent.GetDesignModeFromSite();
            }
        }
        return bRes;
    }
    #endregion // Private Methods
    #endregion // Methods
}
