// define following if you want to test the old approach, without TextRenderer
// #define DONT_USE_TEXT_RENDERER

// Ignore Spelling: Utils, Msec, Inline
//
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.WinApi;

#pragma warning disable IDE0018 // Inline variable declaration


namespace PK.PkUtils.UI.TipHandlers;

/// <summary> Supports tooltips for listbox. </summary>
[CLSCompliant(false)]
public class ListBoxTipHandler : TipHandler
{
    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public ListBoxTipHandler()
    { }

    /// <summary> Single-argument constructor. </summary>
    /// <param name="tipTimeDelayMsec">  Provides initial value of <see cref="TipHandler.TipTimeDelayMsec"/>. </param>
    public ListBoxTipHandler(uint tipTimeDelayMsec)
        : base(tipTimeDelayMsec)
    { }
    #endregion // Constructor(s)

    #region Methods

    #region Public_Methods

    /// <summary>
    /// Get the hooked listbox.
    /// </summary>
    /// <returns>The hooked listbox control.</returns>
    public ListBox GetListBox()
    {
        return HookedControl() as ListBox;
    }

    /// <summary>
    /// Is the hooked listbox owner-draw ( DrawMode.OwnerDrawFixed or DrawMode.OwnerDrawVariable) ?
    /// </summary>
    /// <returns>True if the listbox is owner-draw, false otherwise.</returns>
    public bool IsListBoxOwnerDraw()
    {
        ListBox pList;
        bool bResult = false;

        if (null != (pList = GetListBox()))
        {
            DrawMode dm = pList.DrawMode;
            if ((dm == DrawMode.OwnerDrawFixed) || (dm == DrawMode.OwnerDrawVariable))
                bResult = true;
        }
        return bResult;
    }

    /// <inheritdoc/>
    public override uint ItemFromPoint(Point pt, out uint nSubItem)
    {
        ListBox pListBox = GetListBox();
        uint nTempItem = (uint)ItemNumber.cNoItem;

        nSubItem = (uint)SubItemNumber.cNoSubItem;
        if (0 < pListBox.Items.Count)
        {
            nTempItem = (uint)pListBox.IndexFromPoint(pt);
            nSubItem = 0;
        }

        return nTempItem;
    }

    /// <inheritdoc/>
    public override void GetItemInfo(
        uint nItem,
        uint nSubItem,
        bool bFontInControl,
        out Rectangle rc,
        out string s)
    {
        rc = Rectangle.Empty;
        s = string.Empty;

        if (nItem != (uint)ItemNumber.cNoItem)
        {
            Font pFont;
            Size size;
            ListBox pListBox = GetListBox();
            Debug.Assert(pListBox != null);

            s = GetItemText(nItem, nSubItem);
            rc = pListBox.GetItemRectangle((int)nItem);
            if (bFontInControl)
                pFont = pListBox.Font;
            else
                pFont = this.TipWindow.Font;

            using Graphics g = pListBox.CreateGraphics();
#if DONT_USE_TEXT_RENDERER
            // normally you should use the other code variant, usage of TextRenderer is better
            SizeF sizeF = g.MeasureString(s, pFont);
            size = new Size((int)sizeF.Width, (int)Math.Round(sizeF.Height));
            // to get more exact width than MeasureString
            size.Width = MeasureDisplayStringWidth(g, s, pFont);
#else
            size = TextRenderer.MeasureText(g, s, pFont);
            rc = new Rectangle(rc.Location, size);
#endif // DONT_USE_TEXT_RENDERER
        }
    }
    #endregion // Public_Methods

    #region Protected_Methods

    /// <summary>
    /// Method getting the parent for TipWindow. Overrides the base class implementation.
    /// </summary>
    /// <returns> The parent window for child tip control creation. </returns>
    protected override Control GetParentForTipCreation()
    {
        Control pTemp;
        Control pResult = null;

        if (null != (pTemp = base.GetParentForTipCreation()))
        {
            pResult = pTemp.Parent;
        }
        return pResult;
    }

    /// <summary>
    /// Virtual method creating the (tool)tip window. <br/>
    /// Overrides the base class implementation.
    /// </summary>
    ///
    /// <remarks>
    /// You have to override this method to handle the case your listbox is owner-draw, because CPopupText used in
    /// TipHandler::CreateTipWindow doesn't work in such case.
    /// </remarks>
    ///
    /// <param name="pFont"> The font of new tooltip window. May be null; in that case the font of its parent
    /// window - the hooked control <see cref="TipHandler.HookedControl()"/> - will be used. </param>
    ///
    /// <returns> True on success, false on failure. </returns>
    protected override bool CreateTipWindow(Font pFont)
    {
        bool bRes;
        Debug.Assert(!IsListBoxOwnerDraw());
        bRes = base.CreateTipWindow(pFont);

        return bRes;
    }

    /// <inheritdoc/>
    protected override string GetItemText(uint nItem, uint nSubItem)
    {
        string result = string.Empty;

        if ((nItem != (uint)ItemNumber.cNoItem) && (nSubItem != (uint)SubItemNumber.cNoSubItem))
        {
            ListBox pListBox = GetListBox();

            Debug.Assert(pListBox != null);
            Debug.Assert(nItem < pListBox.Items.Count);
            Debug.Assert(nSubItem == 0);

            result = pListBox.Items[(int)nItem].ToString();
        }

        return result;
    }

    /// <summary>
    /// Overriding the method of the base class
    /// </summary>
    /// <param name="ptInScreen">Screen coordinates of mouse position.</param>
    protected override void OnMouseMove(Point ptInScreen)
    {
        if (User32.IsWindowVisible(this.HookedHWND))
        {
            ListBox pListBox = GetListBox();
            Point ptClient = pListBox.PointToClient(ptInScreen);

            // Get rectangle for item under mouse
            Rectangle rcText = Rectangle.Empty; // item text rectangle in listbox
            uint nItem = (uint)ItemNumber.cNoItem;

            // Must avoid displaying any tooltip, if there is another
            // modal MessageBox or modal dialog overlapping the ListBox.
            // That's why there is a test regarding WndFromPoint
            IntPtr hControl = WndFromPoint(ptInScreen);

            if (hControl != IntPtr.Zero)
            {
                /* for debug purpose
                bool bIstheSame = (hControl == wndHooked.Handle);
                bool bIsParent = (hControl == GetParent(wndHooked.Handle));
                if (theSame || bParent)
                */
                if ((hControl == this.HookedHWND) ||
                    (hControl == User32.GetParent(this.HookedHWND)))
                {   // Get text and text rectangle for item under mouse
                    nItem = OnGetItemInfo(ptClient, true, out _, out rcText, out _);
                }
            }
            // and for another WndFromPoint don't display anything ...

            if (nItem == (uint)ItemNumber.cNoItem)
            {
                TipWindow?.Cancel(); // no item: cancel popup text
            }
            else if (nItem != GetCurItem())
            {
                bool bSel = false;

                if (g_bDrawSelHighlighted)
                {
                    bSel = pListBox.GetSelected((int)nItem);
                }

                // new item, or no item: cancel popup text
                TipWindow.Cancel();
                _nCurItem = nItem; // should set the _nCurItem just now, so the GetCurItem(void) return the proper value

                if (!IsRectCompletelyVisible(rcText))
                {
                    string sText;   // item text
                    Rectangle rc;   // item text rectangle in popup tip
                    int nMargCx = TipWindow.Margins.Width;
                    int deltaCx = nMargCx / 2 - 2;

                    // new item, and not wholly visible: prepare popup tip
                    OnGetItemInfo(ptClient, false, out _, out rc, out sText);

                    // set tip text to that of item
                    TipWindow.Text = sText;
                    // set highlighted status
                    TipWindow.DrawHighlighted = bSel;

                    // move tip window over list text
                    Size sz = new(rc.Width + nMargCx, rc.Height);
                    Point ptLocation = new(rc.Left - deltaCx, rc.Top);

                    TipWindow.MoveToWindow(pListBox, ptLocation, sz);

                    // show popup text delayed
                    TipWindow.ShowDelayed((int)TipTimeDelayMsec);
                }
            }
            _nCurItem = nItem;
        }
    }
    #endregion // Protected_Methods
    #endregion // Methods
}

#pragma warning restore IDE0018 // Inline variable declaration