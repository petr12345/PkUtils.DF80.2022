/***************************************************************************************************************
*
* FILE NAME:   .\UI\General\BaseDataCtrl.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of generic class BaseDataCtrl
*
**************************************************************************************************************/


// Ignore Spelling: Utils, Ctrl
//
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PK.PkUtils.UI.General;

/// <summary>
/// A base for use control containing data of type <typeparamref name="D"/>
/// </summary>
/// <typeparam name="D">The type of the data object.</typeparam>
/// <remarks>
/// To be able to use Visual Inheritance to derive from this control, 
/// all the member controls should be declared here as protected.
/// That way, they should honor their anchor properties in derived 
/// User Control classes.
/// </remarks>
/// <seealso href="http://stackoverflow.com/questions/443777/does-visual-inheritance-work-with-user-controls-in-vs2008">
/// Does Visual Inheritance work with User controls in VS2008
/// </seealso>
[CLSCompliant(true)]
public partial class BaseDataCtrl<D> : UserControl where D : class
{
    #region Public Interface

    /// <summary> Default argument-less constructor. </summary>
    public BaseDataCtrl()
    {
        InitializeComponent();
    }
    /// <summary>
    /// Indicates whether the child label displaying the runtime of this control is visible or not.
    /// </summary>
    /// <value> true if type label visible, false if not. </value>
    [CategoryAttribute("Behavior")]
    [DefaultValueAttribute(true)]
    [DescriptionAttribute("Indicates whether the child label displaying the runtime of this control is visible or not.")]
    public bool TypeLabelVisible
    {
        get { return _TypeLabelVisible; }
        set
        {
            _TypeLabelVisible = value;
            AdjustDescriptionVisibility();
        }
    }

    /// <summary>
    /// A property accessing the data object.
    /// </summary>
    public D Data
    {
        get;
        protected set;
    }

    /// <summary>
    /// Performs initialization of the control that is presented in the dialog.
    /// Called upon changing the data of parent form.
    /// </summary>
    /// <param name="aData">Initialization data.</param>
    public virtual void InitFromData(D aData)
    {
        this.Data = aData;
        InitDataSources();
    }
    #endregion // Public Interface

    #region Protected Interface

    /// <summary>
    /// Initializes the DataSource property of individual binding sources
    /// </summary>
    protected virtual void InitDataSources()
    {
    }
    #endregion // Protected Interface

    #region Private Members

    #region Private Methods

    private void AdjustDescriptionVisibility()
    {
        if (null != _lblTypeText)
            _lblTypeText.Visible = TypeLabelVisible;
    }

    [Conditional("DEBUG")]
    private void AdjustDescriptionText()
    {
        string strType = this.GetType().FullName;
        char[] sep = ['.'];
        string[] parts = strType.Split(sep, StringSplitOptions.RemoveEmptyEntries);

        // adjust text
        _lblTypeText.Text = parts.Last();
    }

    [Conditional("DEBUG")]
    private void AdjustDescriptionLocation()
    {
        // adjust horizontal position
        if ((_lblTypeText.Visible = this.Visible) && (null != _lblTypeText.Parent))
        {
            Point location = _lblTypeText.Location;
            int nLocationX = _lblTypeText.Location.X;
            int nWidth = _lblTypeText.Size.Width;
            int nParentWidth = _lblTypeText.Parent.ClientRectangle.Width;
            int nDelta = nParentWidth - (nLocationX + nWidth);

            /* if (nDelta < 0) */
            if (nDelta != 0)  // just always
            {
                _lblTypeText.Location = new Point(nLocationX + nDelta, location.Y);
            }
        }
    }
    #endregion // Private Methods

    #region Event Handlers

    private void BaseDataCtrl_Load(object sender, EventArgs e)
    {
        if (null != _lblTypeText)
            _lblTypeText.Visible = TypeLabelVisible;
    }

    private void UC_BaseCtrl_VisibleChanged(object sender, EventArgs e)
    {
#if DEBUG
        AdjustDescriptionText();
        AdjustDescriptionLocation();
#endif //DEBUG
        AdjustDescriptionVisibility();
    }
    #endregion // Event Handlers

    #region Private Fields
    private bool _TypeLabelVisible = true;
    #endregion // Private Fields
    #endregion // Private Members
}
