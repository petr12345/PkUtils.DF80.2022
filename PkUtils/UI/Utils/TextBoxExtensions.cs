// Ignore Spelling: CCA, rhs, Sel, Stackoverflow, Utils
//
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.UI.Utils;

/// <summary>
/// A helper class containing several extension methods of TextBoxBase.
/// </summary>
[CLSCompliant(true)]
public static class TextBoxExtensions
{
    /// <summary>
    /// Getting the selection info of the given TextBoxBase 
    /// </summary>
    /// <param name="textBx">The TextBox this is all about.</param>
    /// <returns> A new <see cref="TextBoxSelInfo"/> object.</returns>
    public static TextBoxSelInfo GetSelInfo(this TextBoxBase textBx)
    {
        textBx.CheckNotDisposed(nameof(textBx));

        // Lets stay away from unsafe code here.
        // int nStartChar, nEndChar
        // unsafe
        // {
        //    User32.SendMessage(textBx.Handle, (int)Win32.EM.EM_GETSEL, new IntPtr(&nStartChar), new IntPtr(&nEndChar));
        // }
        // 
        int nStartChar = textBx.SelectionStart;
        int nEndChar = nStartChar + textBx.SelectionLength;
        bool initCaretLast = false;
        TextBoxSelInfo result = new(nStartChar, nEndChar, initCaretLast);

        if (nStartChar != nEndChar)
        {   // determine whether caret is last
            bool actualCaretLast;
            Point ptCaret = new();
            Point ptStart = GetPositionFromCharIndexFix(textBx, nStartChar);
            Point ptEnd = GetPositionFromCharIndexFix(textBx, nEndChar);

            User32.GetCaretPos(ref ptCaret);

            if (ptEnd.X < 0)   // "overflow"
            {
                actualCaretLast = (ptStart.X != ptCaret.X);
            }
            else if (ptCaret.X == ptEnd.X)
            {
                actualCaretLast = true;
            }
            else if (ptCaret.X == ptStart.X)
            {
                actualCaretLast = false;
            }
            else
            {
                actualCaretLast = ((ptEnd.X - ptCaret.X) < (ptCaret.X - ptStart.X));
            }
            if (actualCaretLast != initCaretLast)
            {
                result = result.WithCaretLast(actualCaretLast);
            }
        }

        return result;
    }

    /// <summary>
    /// Replacement ( fixup ) of original TextBoxBase.GetPositionFromCharIndex method,
    /// which does work quite reliably.
    /// </summary>
    /// <param name="textBx">The TextBox this is all about.</param>
    /// <param name="index">The index of the character for which to retrieve the location. </param>
    /// <returns>Retrieves the location within the control at the specified character index.</returns>
    /// <remarks>
    /// The original method <see cref="TextBoxBase.GetPositionFromCharIndex "/>produces bug in two scenarios: <br/>
    /// a/ the case when index == textBx.TextLength, 
    ///   which means the position at the end of string (beyond the last character).
    ///   The returned position does not match what it should be.<br/>
    /// b/ wrong result when the correct character position ( its vertical coordinate) 
    ///   is negative because the position is on the line scrolled-out of view.
    ///   In the case, the returned vertical coordinate from GetPositionFromCharIndex 
    ///   is (wrong) very big positive number. <br/>
    /// </remarks>
    /// 
    /// <seealso cref="GetCharIndexFromPositionFix"/>
    /// 
    /// <seealso href="http://social.msdn.microsoft.com/Forums/en/vbgeneral/thread/5f8a35ef-dd21-41f7-a7e9-8f631b6f5045">
    /// Nasty bug with GetPositionFromCharIndex ? </seealso>
    /// 
    /// <seealso href="http://stackoverflow.com/questions/913735/how-to-move-insert-caret-on-textbox-when-accepting-a-drop">
    /// Stackoverflow: How to move insert caret on textbox when accepting a Drop</seealso>
    ///
    public static Point GetPositionFromCharIndexFix(this TextBoxBase textBx, int index)
    {
        textBx.CheckNotDisposed(nameof(textBx));

        bool bFixA = false;
        Point ptRes = textBx.GetPositionFromCharIndex(index);
        // fixup of the problem /b, to make it negative if the value is wrong very big positive
        ptRes = new Point((short)ptRes.X, (short)ptRes.Y);

        if (ptRes.X < 0)
        {
            bFixA = true; // the fix of problem a/ is needed
        }
        else if ((index > 0) && (index == textBx.TextLength))
        {
            bFixA = true; // the fix of problem a/ is needed
        }

        if (bFixA)
        {
            if (index > 0)
            {
                ptRes = new Point(0, 0);
                int nLen = textBx.TextLength;

                Debug.Assert(nLen <= index);
                if (nLen > 0)
                {
                    string strText = textBx.Text;
                    string strTmp = strText[nLen - 1].ToString();

                    ptRes = GetPositionFromCharIndexFix(textBx, nLen - 1);
                    using Graphics g = textBx.CreateGraphics();

                    /* the TextRenderer provides more exact results
                    SizeF sizeF = g.MeasureString(strTmp, textBx.Font);
                    ptRes.X += (int)sizeF.Width;
                    */
                    ptRes.X += TextRenderer.MeasureText(g, strTmp, textBx.Font).Width;
                }
            }
            else
            {
                ptRes = new Point(0, ptRes.Y);
            }
        }
        else if (ptRes.Y >= 0)
        {
            while (GetCharIndexFromPositionFix(textBx, ptRes) > index)
            {
                ptRes.X--;
            }
        }

        return ptRes;
    }

    /// <summary>
    /// Replacement ( fixup ) of original <see cref="TextBoxBase.GetCharIndexFromPosition"/>.
    /// We must do that because of wrong behavior of GetCharIndexFromPosition, 
    /// which is missing (ignoring) the position at the end of string (beyond the last character).
    /// </summary>
    /// <param name="textBx">The TextBox this is all about.</param>
    /// <param name="pt">point in client area coordinates</param>
    /// <returns>Retrieves the index of the character nearest to the specified location.</returns>
    ///
    /// <seealso cref="GetPositionFromCharIndexFix"/>
    /// 
    /// <seealso href="http://stackoverflow.com/questions/913735/how-to-move-insert-caret-on-textbox-when-accepting-a-drop">
    /// Stackoverflow: How to move insert caret on textbox when accepting a Drop</seealso>
    public static int GetCharIndexFromPositionFix(this TextBoxBase textBx, Point pt)
    {
        textBx.CheckNotDisposed(nameof(textBx));

        int nLastCharIndex = textBx.TextLength - 1;
        int nRes = textBx.GetCharIndexFromPosition(pt);

        if ((nLastCharIndex >= 0) && (nRes == nLastCharIndex))
        {
            Point temp = textBx.GetPositionFromCharIndex(nRes);
            if (pt.X > temp.X)
            {
                nRes++;
            }
        }
        return nRes;
    }

    /// <summary>
    /// Setting the selection
    /// </summary>
    /// <param name="textBx">The TextBox this is all about.</param>
    /// <param name="info">An applied selection info object.</param>
    public static void SetSelInfo(this TextBoxBase textBx, TextBoxSelInfo info)
    {
        ArgumentNullException.ThrowIfNull(textBx);
        ArgumentNullException.ThrowIfNull(info);

        if (info.IsAllSelection || !info.IsCaretLast)
        {
            User32.SendMessage(textBx.Handle, (int)Win32.EM.EM_SETSEL, new IntPtr(info.StartChar), new IntPtr(info.EndChar));
        }
        else
        {
            User32.SendMessage(textBx.Handle, (int)Win32.EM.EM_SETSEL, new IntPtr(info.EndChar), new IntPtr(info.StartChar));
        }
    }
}
