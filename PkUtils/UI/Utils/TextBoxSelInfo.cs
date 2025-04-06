/***************************************************************************************************************
*
* FILE NAME:   .\UI\Utils\TextBoxSelInfo.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class TextBoxSelInfo
*
**************************************************************************************************************/

// Ignore Spelling: rhs, Sel, Utils
//
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.WinApi;
using tPhysPos = System.Int32;


namespace PK.PkUtils.UI.Utils;

/// <summary> TextBoxSelInfo is a text box selection info. It keeps the selection start position, selection
/// end position and the information whether the caret is at the end of selection. </summary>
///
/// <remarks> If the StartChar is 0 and the EndChar is –1 and IsCaretLast is true, all the text in the edit
/// control is selected. <br/> </remarks>
///
/// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb761661(v=vs.85).aspx"> EM_SETSEL message</seealso>
[CLSCompliant(true)]
public class TextBoxSelInfo : IEquatable<TextBoxSelInfo>
{
    #region Fields
    /// <summary>
    /// selection start position
    /// </summary>
    protected tPhysPos _nStartChar;

    /// <summary>
    /// selection end position
    /// </summary>
    protected tPhysPos _nEndChar;

    /// <summary>
    /// is the caret is at the end of selection
    /// </summary>
    protected bool _bIsCaretLast;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Default argument-less constructor.
    /// </summary>
    public TextBoxSelInfo()
    {
    }

    /// <summary>
    /// Constructor with one argument - the caret position. The beginning and the end of selection have the same value.
    /// </summary>
    /// <param name="nPos">The common value of the beginning and the end position.</param>
    public TextBoxSelInfo(int nPos)
      : this(nPos, nPos)
    {
    }

    /// <summary>
    /// Constructor with the selection start and selection end arguments. 
    /// It is assumed the caret is at the start ( not at the end ) of selection.
    /// </summary>
    /// <param name="nStart">The selection start index.</param>
    /// <param name="nEnd">The selection end index.</param>
    public TextBoxSelInfo(int nStart, int nEnd)
      : this(nStart, nEnd, false)
    {
    }

    /// <summary>
    /// Constructor with the selection start, selection end and the information whether the caret is at the end of selection.
    /// </summary>
    /// <param name="nStart">The selection start index.</param>
    /// <param name="nEnd">The selection end index.</param>
    /// <param name="bLast">True if the caret is at the end of selection; false otherwise.</param>
    public TextBoxSelInfo(int nStart, int nEnd, bool bLast)
    {
        _nStartChar = nStart;
        _nEndChar = nEnd;
        _bIsCaretLast = bLast;

        ValidateMe();
    }

    /// <summary>
    /// A copy constructor.
    /// </summary>
    /// <param name="rhs">The original object from which the data are being copied.</param>
    public TextBoxSelInfo(TextBoxSelInfo rhs)
    {
        ArgumentNullException.ThrowIfNull(rhs);

        _nStartChar = rhs.StartChar;
        _nEndChar = rhs.EndChar;
        _bIsCaretLast = rhs.IsCaretLast;

        ValidateMe();
    }
    #endregion // Constructors

    #region Properties

    /// <summary>
    /// The index of beginning character of selection.
    /// </summary>
    public tPhysPos StartChar
    {
        get { return _nStartChar; }
        protected internal set { _nStartChar = value; }
    }

    /// <summary>
    /// The index of ending character of selection.
    /// </summary>
    public tPhysPos EndChar
    {
        get { return _nEndChar; }
        protected internal set { _nEndChar = value; }
    }

    /// <summary>
    /// True if the caret position is at the end of selection; false otherwise.
    /// </summary>
    public bool IsCaretLast
    {
        get { return _bIsCaretLast; }
        protected internal set { _bIsCaretLast = value; }
    }

    /// <summary> Returns true if there is any selection; i.e. if the values of <see cref="StartChar"/> and
    /// <see cref="EndChar"/> are different. </summary>
    /// <value> true if there is any selection, false if not. </value>
    public bool IsSel
    {
        get { return (StartChar != EndChar); }
    }

    /// <summary>
    /// The position of caret.
    /// </summary>
    public int CaretChar
    {
        get { return (!IsCaretLast ? StartChar : EndChar); }
    }

    /// <summary>
    /// True if this selection info specifies that all the text in the related control is selected; false otherwise.
    /// </summary>
    public bool IsAllSelection
    {
        get { return (StartChar == 0 && EndChar == -1 && IsCaretLast); }
    }

    /// <summary>
    /// Returns a new instance of TextBoxSelInfo, which specifies that all the text in the related control is selected.
    /// </summary>
    public static TextBoxSelInfo AllSelection
    {
        get { return new TextBoxSelInfo(0, -1, true); }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Sets the value of <see cref="IsCaretLast"/> property.
    /// </summary>
    /// <param name="bLast">True if the caret is at the end of selection; false otherwise.</param>
    public void SetCaretLast(bool bLast)
    {
        _bIsCaretLast = bLast;
        AssertValid();
    }

    /// <summary>
    /// Modify this TextBoxSelInfo to make it specify that all the text in the related control is selected.
    /// </summary>
    public void MakeAllSelection()
    {
        _nStartChar = 0; _nEndChar = -1; _bIsCaretLast = true;
        AssertValid();
    }

    /// <summary> Overrides the virtual method of the base class. The GetHashCode method is suitable for use in
    /// hashing algorithms and data structures such as a hash table. </summary>
    ///
    /// <returns> A hash code for this object. </returns>
    public override int GetHashCode()
    {
        return (_nStartChar.GetHashCode() ^ _nEndChar.GetHashCode() ^ _bIsCaretLast.GetHashCode());
    }

    /// <summary>
    /// Overrides the virtual method of the base class. Determines whether the specified Object is equal to the current Object.
    /// </summary>
    /// <param name="obj">The object being compared with the current object</param>
    /// <returns>True if this and compared objects are equal, false otherwise.</returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as TextBoxSelInfo);
    }

    /// <summary>
    /// Moves the selection start and selection end by the specified offset <paramref name="nDelta"/>.
    /// </summary>
    /// <param name="nDelta">The offset which is added to selection start position and selection end position.</param>
    public void Offset(int nDelta)
    {
        _nStartChar += nDelta;
        _nEndChar += nDelta;
    }

    /// <summary>
    /// Validates the current object.
    /// </summary>
    [Conditional("DEBUG")]
    public virtual void AssertValid()
    {
        ValidateMe();
    }

    /// <summary>
    /// Non-virtual method validating an instance of this type. 
    /// The reason of existence of this method is to avoid calling virtual method from constructor.
    /// </summary>
    [Conditional("DEBUG")]
    protected void ValidateMe()
    {
        Debug.Assert(IsAllSelection || (StartChar <= EndChar));
    }

#if DEBUG
    /// <summary>
    /// Returns a string describing all values of fields of this instance.
    /// </summary>
    public virtual string Say
    {
        get
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "TextBoxSelInfo: (_nStartChar={0}, _nEndChar={1}, _bIsCaretLast={2})",
                _nStartChar, _nEndChar, _bIsCaretLast);
        }
    }
#endif
    #endregion // Methods

    #region IEquatable<TextBoxSelInfo> Members

    /// <inheritdoc/>
    public virtual bool Equals(TextBoxSelInfo other)
    {
        bool result = false;

        if (other == null)
        {
            /* result = false; already is */
        }
        else if (object.ReferenceEquals(this, other))
        {
            result = true;
        }
        else if (object.ReferenceEquals(other.GetType(), typeof(TextBoxSelInfo)))
        {
            result = ((this.StartChar == other.StartChar)
              && (this.EndChar == other.EndChar)
              && (this.IsCaretLast == other.IsCaretLast));
        }
        else
        {   // the other object is somehow derived, let him decide
            result = other.Equals((object)this);
        }

        return result;
    }
    #endregion // IEquatable<TextBoxSelInfo> Members
};

/// <summary>
/// A helper class containing several extension methods of TextBoxBase.
/// </summary>
[CLSCompliant(true)]
public static class TextBoxHelper
{
    /// <summary>
    /// Getting the selection info of the given TextBoxBase 
    /// </summary>
    /// <param name="textBx">The TextBox this is all about.</param>
    /// <returns> A new <see cref="TextBoxSelInfo"/> object.</returns>
    public static TextBoxSelInfo GetSelInfo(this TextBoxBase textBx)
    {
        int nStartChar, nEndChar;
        TextBoxSelInfo result;

        textBx.CheckNotDisposed(nameof(textBx));

        unsafe
        {
            User32.SendMessage(textBx.Handle, (int)Win32.EM.EM_GETSEL, new IntPtr(&nStartChar), new IntPtr(&nEndChar));
            /* this.CallOrigProc(Win32.EM_GETSEL, new IntPtr(&nStartChar), new IntPtr(&nEndChar)); */
        }
        result = new TextBoxSelInfo(nStartChar, nEndChar);

        if (nStartChar != nEndChar)
        {   // determine whether caret is last
            Point pcaret = new();
            Point pstart = GetPositionFromCharIndexFix(textBx, nStartChar);
            Point pend = GetPositionFromCharIndexFix(textBx, nEndChar);

            User32.GetCaretPos(ref pcaret);

            if (pend.X < 0)   // "overflow"
            {
                result.IsCaretLast = (pstart.X != pcaret.X);
            }
            else if (pcaret.X == pend.X)
            {
                result.IsCaretLast = true;
            }
            else if (pcaret.X == pstart.X)
            {
                /* result.IsCaretLast = false; already is */
            }
            else
            {
                result.IsCaretLast = ((pend.X - pcaret.X) < (pcaret.X - pstart.X));
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
