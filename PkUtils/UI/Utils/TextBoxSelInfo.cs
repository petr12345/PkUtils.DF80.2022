// Ignore Spelling: PK, rhs, Sel, Stackoverflow, Utils
//
using System;
using System.Diagnostics;
using System.Globalization;

namespace PK.PkUtils.UI.Utils;


/// <summary>
/// Represents information about the selection or caret position in a text box, 
/// in an immutable way.
/// </summary>
/// 
/// <remarks>
/// If the StartChar is 0 and the EndChar is –1 and IsCaretLast is true, 
/// all the text in the edit control is selected. <br/>
/// </remarks>
/// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb761661(v=vs.85).aspx">EM_SETSEL message</seealso>
public sealed class TextBoxSelInfo : IEquatable<TextBoxSelInfo>
{
    #region Fields
    // No backing fields — using immutable auto-properties.
    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBoxSelInfo"/> class 
    /// with specified start and end positions and caret direction.
    /// </summary>
    /// <param name="nStart">The start character index.</param>
    /// <param name="nEnd">The end character index.</param>
    /// <param name="isCaretLast">True if the caret is logically after the selection; otherwise, false.</param>
    public TextBoxSelInfo(int nStart, int nEnd, bool isCaretLast)
    {
        StartChar = nStart;
        EndChar = nEnd;
        IsCaretLast = isCaretLast;
        ValidateMe();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBoxSelInfo"/> class 
    /// representing a caret position at a specific character without selection.
    /// </summary>
    /// <param name="nPos">The caret character index.</param>
    public TextBoxSelInfo(int nPos) : this(nPos, nPos, false) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBoxSelInfo"/> class by copying another instance.
    /// </summary>
    /// <param name="rhs">The instance to copy.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="rhs"/> is null.</exception>
    public TextBoxSelInfo(TextBoxSelInfo rhs)
    {
        ArgumentNullException.ThrowIfNull(rhs);
        StartChar = rhs.StartChar;
        EndChar = rhs.EndChar;
        IsCaretLast = rhs.IsCaretLast;
        ValidateMe();
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Gets the zero-based index of the selection start character.
    /// </summary>
    public int StartChar { get; }

    /// <summary>
    /// Gets the zero-based index of the selection end character.
    /// </summary>
    public int EndChar { get; }

    /// <summary>
    /// Gets a value indicating whether the caret is logically positioned after the selection.
    /// </summary>
    public bool IsCaretLast { get; }

    /// <summary>
    /// Gets a value indicating whether any text is selected.
    /// </summary>
    public bool IsSel { get => StartChar != EndChar; }

    /// <summary>
    /// Gets the index of the character where the caret is logically placed.
    /// </summary>
    public int CaretChar { get => IsCaretLast ? EndChar : StartChar; }

    /// <summary>
    /// Gets a value indicating whether the selection spans the entire content.
    /// </summary>
    public bool IsAllSelection { get => StartChar == 0 && EndChar == -1 && IsCaretLast; }

    /// <summary>
    /// Returns a special instance representing a selection of the entire text content.
    /// </summary>
    public static TextBoxSelInfo AllSelection { get => new(0, -1, true); }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Returns a new <see cref="TextBoxSelInfo"/> with the selection or caret offset by a specified amount.
    /// </summary>
    /// <param name="delta">The offset to apply (can be negative).</param>
    /// <returns>A new adjusted <see cref="TextBoxSelInfo"/> instance.</returns>
    public TextBoxSelInfo WithOffset(int delta)
    {
        int newStart = Math.Max(0, StartChar + delta);
        int newEnd = Math.Max(0, EndChar + delta);
        return new TextBoxSelInfo(newStart, newEnd, IsCaretLast);
    }

    /// <summary>
    /// Returns a new <see cref="TextBoxSelInfo"/> instance with the same selection bounds
    /// but with the specified caret position logic.
    /// </summary>
    /// <param name="caretLast">True if the caret should be logically after the selection; otherwise, false.</param>
    /// <returns>A new <see cref="TextBoxSelInfo"/> instance with the updated caret direction.</returns>
    public TextBoxSelInfo WithCaretLast(bool caretLast)
    {
        return new TextBoxSelInfo(StartChar, EndChar, caretLast);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as TextBoxSelInfo);

    /// <summary>
    /// Determines whether the specified <see cref="TextBoxSelInfo"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The instance to compare with.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(TextBoxSelInfo other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartChar == other.StartChar &&
               EndChar == other.EndChar &&
               IsCaretLast == other.IsCaretLast;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(StartChar, EndChar, IsCaretLast);

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string with selection/caret details.</returns>
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture,
            "TextBoxSelInfo(StartChar={0}, EndChar={1}, IsCaretLast={2})",
            StartChar, EndChar, IsCaretLast);
    }

    /// <summary>
    /// Performs consistency checks in debug builds.
    /// </summary>
    [Conditional("DEBUG")]
    private void ValidateMe()
    {
        Debug.Assert(IsAllSelection || StartChar <= EndChar);
    }

    #endregion // Methods
}
