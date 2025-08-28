// Ignore Spelling: Utils, Api, Winapi, Ctrl, clr, Unmap, memset, ull, bufsize, FNAME, LCID, DAC, COLORREF Gdi FW LOGFONT PHYSICALOFFSETX PHYSICALOFFSETY DONTCARE EXTRALIGHT SEMIBOLD EXTRABOLD ULTRABOLD DEMIBOLD lf TEXTMETRIC Pels LOGPIXELSX LOGPIXELSY BITMAPINFOHEADER Rgn CLIPFORMAT Dest Rop Pel Enh Org RGB lpv hemf lplf Argb BKMODE Viewport
//
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace PK.PkUtils.WinApi;


#pragma warning disable 1591  // Missing XML comment for publicly visible type or member...
#pragma warning disable CA1401  // P/Invoke method should not be visible
#pragma warning disable CA1806  // The HRESULT of some API is not used
#pragma warning disable SYSLIB1054  // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time


#region COLORREF class

/// <summary> The COLORREF is used to represent an Win32 RGB color.
/// When specifying an explicit RGB color, the COLORREF value has the following hexadecimal form:
/// 0x00bbggrr.
/// Note this is different from .NET 32-bit ARGB value, which is AARRGGBB.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct COLORREF
{
    /// <summary> The red component of the color. </summary>
    [FieldOffset(0)]
    public byte R;

    /// <summary> The green component of the color. </summary>
    [FieldOffset(1)]
    public byte G;

    /// <summary> The blue component of the color. </summary>
    [FieldOffset(2)]
    public byte B;

    /// <summary> The complete Win32 color value. </summary>
    [FieldOffset(0)]
    public uint Value;

    /// <summary> Constructs the color from individual components. </summary>
    /// <param name="r"> The red component of the color. </param>
    /// <param name="g"> The green component of the color. </param>
    /// <param name="b"> The blue component of the color. </param>
    public COLORREF(byte r, byte g, byte b)
    {
        this.Value = 0;
        this.R = r;
        this.G = g;
        this.B = b;
    }

    /// <summary> Constructs the color from Win32 color value. </summary>
    /// <param name="value"> The complete Win32 color. </param>
    public COLORREF(uint value)
    {
        this.R = 0;
        this.G = 0;
        this.B = 0;
        this.Value = value & 0x00FFFFFF;
    }

    /// <summary> Explicit cast that converts the given Win32 color value (in a format of 0x00bbggrr)
    /// to a COLORREF. </summary>
    /// <param name="val"> The value. </param>
    /// <returns> The resulting COLORREF. </returns>
    public static explicit operator COLORREF(uint val)
    {
        return new COLORREF(val);
    }

    /// <summary> Explicit cast that converts the given Win32 color value (in a format of 0x00bbggrr)
    /// to a COLORREF. </summary>
    /// <param name="val"> The value. </param>
    /// <returns> The resulting COLORREF. </returns>
    public static explicit operator COLORREF(int val)
    {
        return (COLORREF)unchecked(val);
    }

    /// <summary> Explicit cast that converts the given COLORREF to an unsigned integer
    /// Win32 color value (in a format of 0x00bbggrr).
    /// </summary>
    /// <param name="clr"> The color. </param>
    /// <returns> The result of the operation. </returns>
    public static explicit operator uint(COLORREF clr)
    {
        return clr.Value;
    }

    /// <summary> Explicit cast that converts the given COLORREF to an integer
    /// Win32 color value (in a format of 0x00bbggrr).
    /// </summary>
    /// <param name="clr"> The color. </param>
    /// <returns> The result of the operation. </returns>
    public static explicit operator int(COLORREF clr)
    {
        return unchecked((int)clr.Value);
    }
}
#endregion // COLORREF class

#region PrinterBounds class

/// <summary>
/// A helper class for getting the printer's REAL margin bounds in .NET
/// </summary>
/// <remarks>
/// This calls works-around a serious limitation in the .NET printing classes. 
/// The class PrinterBounds retrieves the real printing bounds of a printed page. 
/// The .NET printing classes don't take into account the physical left and top margins of the printer. <br/>
/// In more detail, the PrintPageEventArgs.MarginBounds property doesn't know anything 
/// about the physical left and right margins for your printer (the non-printable region), 
/// and .NET provides no way to find out what these margins are.
/// </remarks>
/// <seealso href="http://www.codeproject.com/Articles/7596/Getting-the-printer-s-REAL-margin-bounds-in-NET">
/// CodeProject article 'Getting the printer's REAL margin bounds in .NET'
/// </seealso>
/// <example>
/// <code language="xml" title="Usage example">
/// <![CDATA[
/// private void printDoc_PrintPage(object sender, 
///    System.Drawing.Printing.PrintPageEventArgs args)
/// {
///   PrinterBounds objBounds = new PrinterBounds(e);
///   Rectangle r = objBounds.Bounds;  // Get the REAL Margin Bounds !
///   
///   e.Graphics.DrawRectangle(Pens.Black , r.Left , r.Top , 200 , 200);
///   e.HasMorePages = false;
/// }
/// ]]>
/// </code>
/// </example>
public class PrinterBounds
{
    /// <summary> Gets the rectangular area that represents the portion of the page inside the margins. 
    /// </summary>
    public readonly Rectangle Bounds;

    /// <summary> 
    /// The hard margin left. 
    /// The value is retrieved through the call of GetDeviceCaps(hDC , PHYSICALOFFSETX);
    /// </summary>
    public readonly int HardMarginLeft;

    /// <summary> 
    /// The hard margin top. 
    /// The value is retrieved through the call of GetDeviceCaps(hDC , PHYSICALOFFSETY);
    /// </summary>
    public readonly int HardMarginTop;

    /// <summary>
    /// Constructor initializing read-only fields.
    /// </summary>
    /// <param name="args">Provides data for the PrintPage event. </param>
    public PrinterBounds(PrintPageEventArgs args)
    {
        IntPtr hDC = args.Graphics.GetHdc();

        HardMarginLeft = Gdi32.GetDeviceCaps(hDC, Gdi32.PHYSICALOFFSETX);
        HardMarginTop = Gdi32.GetDeviceCaps(hDC, Gdi32.PHYSICALOFFSETY);

        args.Graphics.ReleaseHdc(hDC);

        HardMarginLeft = (int)(HardMarginLeft * 100.0 / args.Graphics.DpiX);
        HardMarginTop = (int)(HardMarginTop * 100.0 / args.Graphics.DpiY);

        Bounds = args.MarginBounds;

        Bounds.Offset(-HardMarginLeft, -HardMarginTop);
    }
}
#endregion // PrinterBounds class

#region StockObject enum

/// <summary> Values that represent the type of stock objects, used as an input argument 
/// of method <see cref="WinApi.Gdi32.GetStockObject"/>. </summary>
public enum StockObject
{
    /// <summary> White brush. </summary>
    WHITE_BRUSH = 0,
    /// <summary> Light gray brush. </summary>
    LTGRAY_BRUSH = 1,
    /// <summary> Gray brush. </summary>
    GRAY_BRUSH = 2,
    /// <summary> Dark gray brush. </summary>
    DKGRAY_BRUSH = 3,
    /// <summary> Black brush. </summary>
    BLACK_BRUSH = 4,
    /// <summary> Null brush (equivalent to HOLLOW_BRUSH). </summary>
    NULL_BRUSH = 5,
    /// <summary> Hollow brush (equivalent to NULL_BRUSH). </summary>
    HOLLOW_BRUSH = NULL_BRUSH,
    /// <summary> White pen. </summary>
    WHITE_PEN = 6,
    /// <summary> Black pen. </summary>
    BLACK_PEN = 7,
    /// <summary> Null pen. (Such pen draws nothing).</summary>
    NULL_PEN = 8,
    /// <summary> Original equipment manufacturer (OEM) dependent fixed-pitch (monospace) font.</summary>
    OEM_FIXED_FONT = 10,
    /// <summary> Windows fixed-pitch (monospace) system font.</summary>
    ANSI_FIXED_FONT = 11,
    /// <summary> Windows variable-pitch (proportional space) system font.</summary>
    ANSI_VAR_FONT = 12,
    /// <summary> System font. By default, the system uses the system font to draw menus, 
    /// dialog box controls, and text. It is not recommended that you use DEFAULT_GUI_FONT or SYSTEM_FONT 
    /// to obtain the font used by dialogs and windows. 
    /// The default system font is Tahoma.
    /// </summary>
    SYSTEM_FONT = 13,
    /// <summary> Device-dependent font.</summary>
    DEVICE_DEFAULT_FONT = 14,
    /// <summary> Default palette. This palette consists of the static colors in the system palette.</summary>
    DEFAULT_PALETTE = 15,
    /// <summary> Fixed-pitch (monospace) system font. This stock object is provided only for compatibility 
    /// with 16-bit Windows </summary>
    SYSTEM_FIXED_FONT = 16,
}
#endregion // StockObject enum

#region Gdi32 class
/// <summary>
/// Wrapper class for the gdi32.dll.
/// </summary>
public static class Gdi32
{
    #region Constants
    // from WinGDI.h
    public const int DEFAULT_PITCH = 0;
    public const int FIXED_PITCH = 1;
    public const int VARIABLE_PITCH = 2;

    // Font Families
    public const int FF_DONTCARE = (0 << 4);  // Don't care or don't know.
    public const int FF_ROMAN = (1 << 4);  // Variable stroke width, serifed.
                                           // Times Roman, Century Schoolbook, etc.
    public const int FF_SWISS = (2 << 4);  // Variable stroke width, sans-serifed.
                                           // Helvetica, Swiss, etc.
    public const int FF_MODERN = (3 << 4);  // Constant stroke width, serifed or sans-serifed.
                                            // Pica, Elite, Courier, etc.
    public const int FF_SCRIPT = (4 << 4);  // Cursive, etc.
    public const int FF_DECORATIVE = (5 << 4);  // Old English, etc.


    // Font Weights
    public const int FW_DONTCARE = 0;
    public const int FW_THIN = 100;
    public const int FW_EXTRALIGHT = 200;
    public const int FW_LIGHT = 300;
    public const int FW_NORMAL = 400;
    public const int FW_MEDIUM = 500;
    public const int FW_SEMIBOLD = 600;
    public const int FW_BOLD = 700;
    public const int FW_EXTRABOLD = 800;
    public const int FW_HEAVY = 900;

    public const int FW_ULTRALIGHT = FW_EXTRALIGHT;
    public const int FW_REGULAR = FW_NORMAL;
    public const int FW_DEMIBOLD = FW_SEMIBOLD;
    public const int FW_ULTRABOLD = FW_EXTRABOLD;
    public const int FW_BLACK = FW_HEAVY;

    public const int PHYSICALOFFSETX = 112;
    public const int PHYSICALOFFSETY = 113;
    /// <summary> 
    /// A constant representing invalid color ( returned for instance by <see cref="GetBkColor"/>. 
    ///</summary>
    public const uint CLR_INVALID = 0xFFFFFFFF;

    private const int LF_FACESIZE = 32;
    #endregion // Constants

    #region Nested types

    /// <summary>
    /// The LOGFONT structure defines the attributes of a font.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx">
    /// MSDN documentation of LOGFONT structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class LOGFONT
    {
        /// <summary> The height, in logical units, of the font's character cell or character. </summary>
        public int lfHeight;
        /// <summary> The average width, in logical units, of characters in the font. </summary>
        public int lfWidth;
        /// <summary>
        /// The angle, in tenths of degrees, between the escapement vector and the x-axis of the device.
        /// </summary>
        public int lfEscapement;
        /// <summary>
        /// The angle, in tenths of degrees, between each character's base line and the x-axis of the device.
        /// </summary>
        public int lfOrientation;
        /// <summary>
        /// The weight of the font in the range 0 through 1000. For example, 400 is normal and 700 is bold.
        /// </summary>
        public int lfWeight;
        /// <summary> An italic font if set to 1 (true). </summary>
        public byte lfItalic;
        /// <summary> An underlined font if set to 1 (true). </summary>
        public byte lfUnderline;
        /// <summary> An strikeout font if set to 1 (true). </summary>
        public byte lfStrikeOut;
        /// <summary>
        /// The character set (for instance ANSI_CHARSET, BALTIC_CHARSET, CHINESEBIG5_CHARSET etc.).
        /// </summary>
        public byte lfCharSet;
        /// <summary>
        /// The output precision. The output precision defines how closely the output must match the requested
        /// font's height, width, character orientation, escapement, pitch, and font type.
        /// </summary>
        public byte lfOutPrecision;
        /// <summary>
        /// The clipping precision. The clipping precision defines how to clip characters that are partially
        /// outside the clipping region.
        /// </summary>
        public byte lfClipPrecision;
        /// <summary>
        /// The output quality. The output quality defines how carefully the graphics device interface (GDI)
        /// must attempt to match the logical-font attributes to those of an actual physical font.
        /// </summary>
        public byte lfQuality;
        /// <summary>
        /// The pitch and family of the font. The two low-order bits specify the pitch of the font, Bits 4
        /// through 7 of the member specify the font family.
        /// </summary>
        public byte lfPitchAndFamily;
        /// <summary>
        /// A null-terminated string that specifies the typeface name of the font. <br/>
        /// Marshalling of LOGFONT.lfFaceName must stay UnmanagedType.ByValTStr as it is now, regardless any
        /// FxCop complains!
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
        public string lfFaceName; /* = null; - not needed */
    };

    /// <summary>
    /// The TEXTMETRIC structure contains basic information about a physical font. All sizes are specified in
    /// logical units; that is, they depend on the current mapping mode of the display context.
    /// </summary>
    ///
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd145132(v=vs.85).aspx">
    /// MSDN documentation of TEXTMETRIC structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class TEXTMETRIC
    {
        /// <summary> The height (ascent + descent) of characters.</summary>
        public int tmHeight;

        /// <summary> The ascent (units above the base line) of characters.</summary>
        public int tmAscent;

        /// <summary> The descent (units below the base line) of characters.</summary>
        public int tmDescent;

        /// <summary>
        /// The amount of leading (space) inside the bounds set by the tmHeight member. Accent marks and other
        /// diacritical characters may occur in this area. The designer may set this member to zero.
        /// </summary>
        public int tmInternalLeading;

        /// <summary>
        /// The amount of extra leading (space) that the application adds between rows. Since this area is
        /// outside the font, it contains no marks and is not altered by text output calls in either OPAQUE or
        /// TRANSPARENT mode. The designer may set this member to zero.
        /// </summary>
        public int tmExternalLeading;

        /// <summary>
        /// The average width of characters in the font (generally defined as the width of the letter x ). This
        /// value does not include the overhang required for bold or italic characters.
        /// </summary>
        public int tmAveCharWidth;

        /// <summary> The width of the widest character in the font. </summary>
        public int tmMaxCharWidth;

        /// <summary> The weight of the font. </summary>
        public int tmWeight;

        /// <summary> The extra width per string that may be added to some synthesized fonts. </summary>
        public int tmOverhang;

        /// <summary> The horizontal aspect of the device for which the font was designed. </summary>
        public int tmDigitizedAspectX;

        /// <summary>
        /// The vertical aspect of the device for which the font was designed. The ratio of the
        /// tmDigitizedAspectX and tmDigitizedAspectY members is the aspect ratio of the device for which the
        /// font was designed.
        /// </summary>
        public int tmDigitizedAspectY;

        /// <summary> The value of the first character defined in the font. </summary>
        public char tmFirstChar;

        /// <summary> The value of the last character defined in the font. </summary>
        public char tmLastChar;

        /// <summary> The value of the character to be substituted for characters not in the font. </summary>
        public char tmDefaultChar;

        /// <summary>
        /// The value of the character that will be used to define word breaks for text justification.
        /// </summary>
        public char tmBreakChar;

        /// <summary> Specifies an italic font if it is nonzero. </summary>
        public byte tmItalic;

        /// <summary> Specifies an underlined font if it is nonzero. </summary>
        public byte tmUnderlined;

        /// <summary> A strikeout font if it is nonzero. </summary>
        public byte tmStruckOut;

        /// <summary>
        /// Specifies information about the pitch, the technology, and the family of a physical font.
        /// </summary>
        public byte tmPitchAndFamily;

        /// <summary> The character set of the font. </summary>
        public byte tmCharSet;
    }

    /// <summary>
    /// The BITMAP structure defines the type, width, height, color format, and bit values of a bitmap.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd183371(v=vs.85).aspx">
    /// MSDN documentation of BITMAP structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public struct BITMAP
    {
        /// <summary> The bitmap type. This member must be zero.</summary>
        public Int32 bmType;

        /// <summary> The width, in pixels, of the bitmap. The width must be greater than zero. </summary>
        public Int32 bmWidth;

        /// <summary> The height, in pixels, of the bitmap. The height must be greater than zero. </summary>
        public Int32 bmHeight;

        /// <summary>
        /// The number of bytes in each scan line. This value must be divisible by 2, because the system assumes
        /// that the bit values of a bitmap form an array that is word aligned.
        /// </summary>
        public Int32 bmWidthBytes;

        /// <summary> The count of color planes. </summary>
        public Int16 bmPlanes;

        /// <summary> The number of bits required to indicate the color of a pixel. </summary>
        public Int16 bmBitsPixel;

        /// <summary>
        /// A pointer to the location of the bit values for the bitmap. The bmBits member must be a pointer 
        /// to an array of character (1-byte) values.
        /// </summary>
        public IntPtr bmBits;
    }

    /// <summary>
    /// The BITMAPINFOHEADER structure contains information about the dimensions and color format of a DIB.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd183376(v=vs.85).aspx">
    /// MSDN documentation of BITMAPINFOHEADER structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public struct BITMAPINFOHEADER
    {
        /// <summary> The number of bytes required by the structure. </summary>
        public int biSize;

        /// <summary>
        /// The width of the bitmap, in pixels. 
        /// If biCompression is BI_JPEG or BI_PNG, the biWidth member specifies the width
        /// of the decompressed JPEG or PNG image file, respectively.
        /// </summary>
        public int biWidth;

        /// <summary>
        /// The height of the bitmap, in pixels. If biHeight is positive, the bitmap is a bottom-up DIB and its
        /// origin is the lower-left corner. If biHeight is negative, the bitmap is a top-down DIB and its origin
        /// is the upper-left corner.
        /// 
        /// If biHeight is negative, indicating a top-down DIB, biCompression must be either BI_RGB or
        /// BI_BITFIELDS. Top-down DIBs cannot be compressed.
        /// 
        /// If biCompression is BI_JPEG or BI_PNG, the biHeight member specifies the height of the decompressed
        /// JPEG or PNG image file, respectively.
        /// </summary>
        /// 
        public int biHeight;

        /// <summary> The number of planes for the target device. This value must be set to 1. </summary>
        public Int16 biPlanes;

        /// <summary> The number of bits-per-pixel. </summary>
        public Int16 biBitCount;

        /// <summary>
        /// The type of compression for a compressed bottom-up bitmap (top-down DIBs cannot be compressed).
        /// </summary>
        public int biCompression;

        /// <summary> The size, in bytes, of the image. This may be set to zero for BI_RGB bitmaps. </summary>
        public int biSizeImage;

        /// <summary>
        /// The horizontal resolution, in pixels-per-meter, of the target device for the bitmap. An application
        /// can use this value to select a bitmap from a resource group that best matches the characteristics of
        /// the current device.
        /// </summary>
        public int biXPelsPerMeter;

        /// <summary>
        /// The vertical resolution, in pixels-per-meter, of the target device for the bitmap.
        /// </summary>
        public int biYPelsPerMeter;

        /// <summary>
        /// The number of color indexes in the color table that are actually used by the bitmap.
        /// </summary>
        public int biClrUsed;

        /// <summary>
        /// The number of color indexes that are required for displaying the bitmap. If this value is zero, all
        /// colors are required.
        /// </summary>
        public int bitClrImportant;
    };

    /// <summary> Values that represent the background mode, used as an argument of . </summary>
    public enum BKMODE
    {
        /// <summary> An enum constant representing the transparent case. Background is filled 
        /// with the current background color before the text, hatched brush, or pen is drawn.</summary>
        TRANSPARENT = 1,

        /// <summary> An enum constant representing the opaque case. Background remains untouched.</summary>
        OPAQUE = 2,
    };

    /// <summary> 
    /// Values that represent drawing modes, used as an argument of method <see cref="Gdi32.SetROP2"/>. 
    /// </summary>
    public enum DrawingMode
    {
        /// <summary> All drawing in black, irrespective of the pen color and the background color. </summary>
        R2_BLACK = 1,

        /// <summary> Drawing is the color that is the inverse of the <see cref="R2_MERGEPEN"/>. </summary>
        R2_NOTMERGEPEN = 2,

        /// <summary> Drawing is the color produced by AND-ing the background color with the inverse
        ///  of the pen color. </summary>
        R2_MASKNOTPEN = 3,

        /// <summary> Drawing is the inverse of the pen color. </summary>
        R2_NOTCOPYPEN = 4,

        /// <summary> Drawing is the color produced by AND-ing the pen color with the inverse 
        /// of the background color. </summary>
        R2_MASKPENNOT = 5,

        /// <summary> Drawing is the inverse of the screen color. This ensures that the output
        /// is always visible because it draws in the inverse color to the background.
        /// </summary>
        R2_NOT = 6,

        /// <summary> Resulting pixel is a combination of the colors in the pen and in the screen, 
        /// but not in both. (I.e., does bitwise exclusive-OR of pen and background.)</summary>
        R2_XORPEN = 7,

        /// <summary> Drawing is the color that is inverse of the <see cref="R2_MASKPEN"/> color. </summary>
        R2_NOTMASKPEN = 8,

        /// <summary> Drawing is the color produced by AND-ing the background color with the pen color.
        /// </summary>
        R2_MASKPEN = 9,

        /// <summary> Resulting pixel is the inverse of the <see cref="R2_XORPEN"/> color. </summary>
        R2_NOTXORPEN = 10,

        /// <summary> The drawing operation does nothing. </summary>
        R2_NOP = 11,

        /// <summary> Drawing is the color produced by OR-ing the background color with the inverse 
        ///  of the pen color. </summary>
        R2_MERGENOTPEN = 12,

        /// <summary> Drawing is the pen color. This is the default drawing mode if you don't set a mode.
        /// </summary>
        R2_COPYPEN = 13,

        /// <summary> Drawing is the color produced by OR-ing the pen color with the inverse 
        /// of the background color. </summary>
        R2_MERGEPENNOT = 14,

        /// <summary> Drawing is the color produced by OR-ing the pen color with the background color. 
        /// </summary>
        R2_MERGEPEN = 15,

        /// <summary> All drawing in white, irrespective of the pen color and the background color. </summary>
        R2_WHITE = 16,
    };

    /// <summary>
    /// Enumeration for the raster operations used in BitBlt.
    /// In C++ these are actually #define. But to use these
    /// constants with C#, a new enumeration type is defined.
    /// </summary>
    public enum TernaryRasterOperations
    {
        SRCCOPY = 0x00CC0020, // dest = source
        SRCPAINT = 0x00EE0086, // dest = source OR dest
        SRCAND = 0x008800C6, // dest = source AND dest
        SRCINVERT = 0x00660046, // dest = source XOR dest
        SRCERASE = 0x00440328, // dest = source AND (NOT dest)
        NOTSRCCOPY = 0x00330008, // dest = (NOT source)
        NOTSRCERASE = 0x001100A6, // dest = (NOT src) AND (NOT dest)
        MERGECOPY = 0x00C000CA, // dest = (source AND pattern)
        MERGEPAINT = 0x00BB0226, // dest = (NOT source) OR dest
        PATCOPY = 0x00F00021, // dest = pattern
        PATPAINT = 0x00FB0A09, // dest = DPSnoo
        PATINVERT = 0x005A0049, // dest = pattern XOR dest
        DSTINVERT = 0x00550009, // dest = (NOT dest)
        BLACKNESS = 0x00000042, // dest = BLACK
        WHITENESS = 0x00FF0062, // dest = WHITE
    };

    /// <summary>
    /// Enumeration for Know clipboard formats. For more info about clipboard formats, see
    /// <see href="http://msdn.microsoft.com/ja-jp/library/microsoft.teamfoundation.officeintegration.client.clipformat(v=vs.100).aspx">
    /// MSDN CLIPFORMAT Enumeration
    /// </see>
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ff729168(v=vs.85).aspx">
    /// Standard Clipboard Formats
    /// </seealso>
    public enum CLIPFORMAT
    {

        /// <summary>
        /// Text format. Each line ends with a carriage return/linefeed (CR-LF) combination. A null character
        /// signals the end of the data. Use this format for ANSI text.
        /// </summary>
        CF_TEXT = 1,

        /// <summary> Represents a handle to a bitmap (HBITMAP).</summary>
        CF_BITMAP = 2,

        /// <summary>
        /// Handle to a metafile picture format as defined by the METAFILEPICT structure. When passing a
        /// CF_METAFILEPICT handle by means of DDE, the application responsible for deleting hMem should also
        /// free the metafile referred to by the CF_METAFILEPICT handle.
        /// </summary>
        CF_METAFILEPICT = 3,

        /// <summary> Microsoft Symbolic Link (SYLK) format. </summary>
        CF_SYLK = 4,

        /// <summary> Software Arts' Data Interchange Format. </summary>
        CF_DIF = 5,

        /// <summary> Tagged-image file format. </summary>
        CF_TIFF = 6,

        /// <summary>
        /// Text format containing characters in the OEM character set. Each line ends with a carriage
        /// return/linefeed (CR-LF) combination. A null character signals the end of the data.
        /// </summary>
        CF_OEMTEXT = 7,

        /// <summary> A memory object containing a BITMAPINFO structure followed by the bitmap bits.</summary>
        CF_DIB = 8,

        /// <summary>
        /// Handle to a color palette. Whenever an application places data in the clipboard that depends on or
        /// assumes a color palette, it should place the palette on the clipboard as well.
        /// </summary>
        CF_PALETTE = 9,

        /// <summary> Data for the pen extensions to the Microsoft Windows for Pen Computing. </summary>
        CF_PENDATA = 10,

        /// <summary>
        /// Represents audio data more complex than can be represented in a CF_WAVE standard wave format.
        /// </summary>
        CF_RIFF = 11,

        /// <summary>
        /// Represents audio data in one of the standard wave formats, such as 11 kHz or 22 kHz PCM.
        /// </summary>
        CF_WAVE = 12,

        /// <summary>
        /// Unicode text format. Each line ends with a carriage return/linefeed (CR-LF) combination. A null
        /// character signals the end of the data.
        /// </summary>
        CF_UNICODETEXT = 13,

        /// <summary> A handle to an enhanced metafile (HENHMETAFILE). </summary>
        CF_ENHMETAFILE = 14,

        /// <summary>
        /// A handle to type HDROP that identifies a list of files. An application can retrieve information about
        /// the files by passing the handle to the DragQueryFile function.
        /// </summary>
        CF_HDROP = 15,

        /// <summary> The data is a handle to the locale identifier associated with text in the clipboard. </summary>
        CF_LOCALE = 16,

        /// <summary>
        /// A memory object containing a BITMAPV5HEADER structure followed by the bitmap color space information
        /// and the bitmap bits.
        /// </summary>
        CF_DIBV5 = 17,

        /* no good sense to use that a macro from C++ in C#
        CF_MAX = 18,
        */

        /// <summary>
        /// Owner-display format. The clipboard owner must display and update the clipboard viewer window, and
        /// receive the WM_ASKCBFORMATNAME, WM_HSCROLLCLIPBOARD, WM_PAINTCLIPBOARD, WM_SIZECLIPBOARD, and
        /// WM_VSCROLLCLIPBOARD messages.
        /// </summary>
        CF_OWNERDISPLAY = 0x0080,
    }
    #endregion // Nested types functions

    #region External functions
    #region Public External functions

    /// <summary>
    /// The GetObjectLOGFONT function retrieves information for the specified font, represented by font
    /// handle.
    /// </summary>
    /// <remarks>
    /// The method is just a nickname of <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd144904(v=vs.85).aspx">
    /// GetObject</see> function, declared with specific type arguments to get LOGFONT data. <br/>
    /// To satisfy the FxCop rule
    /// <see href="httphttp://msdn.microsoft.com/cs-cz/library/ms182319(en-us).aspx">SpecifyMarshalingForPInvokeStringArguments</see>
    /// we apply BestFitMapping = false. Marshalling of structure LOGFONT and of LOGFONT.lfFaceName must stay
    /// as it is now, regardless of FxCop complains.
    /// </remarks>
    /// <param name="hFont"> A handle to the font. </param>
    /// <param name="nSize"> The number of bytes of information to be written to the buffer. The caller
    ///  should use Marshal.SizeOf(typeof(LOGFONT)) for this argument. </param>
    /// <param name="lpLogFont"> The LOGFONT that is returned is the LOGFONT used to create the font. </param>
    /// <returns>
    /// If the function succeeds, the return value is the number of bytes stored into the lpLogFont.
    /// <paramref name="lpLogFont"/>. If the function fails, the return value is zero.
    /// </returns>
    [DllImport("gdi32", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true, EntryPoint = "GetObject")]
    public static extern int GetObjectLOGFONT(
      IntPtr hFont,
      int nSize,
      [In, Out][MarshalAs(UnmanagedType.LPStruct)] LOGFONT lpLogFont);

    /// <summary> Retrieves information for the specified bitmap. </summary>
    /// <remarks>
    /// The method is just a nickname of <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd144904(v=vs.85).aspx">
    /// GetObject</see> function, declared with specific type arguments to get BITMAP data.
    /// </remarks>
    /// <param name="hObject"> A handle to a logical bitmap or a device independent bitmap created by
    ///  calling the CreateDIBSection. </param>
    /// <param name="nSize"> The number of bytes of information to be written to the buffer. The caller
    ///  should use Marshal.SizeOf(typeof(BITMAP)) for this argument. </param>
    /// <param name="lpBitmap"> [out] The BITMAP structure to be filled-in. </param>
    /// <returns>
    /// If the function succeeds, the return value is the number of bytes stored into the output BITMAP
    /// structure. If the function fails, the return value is zero.
    /// </returns>
    [DllImport("gdi32", CharSet = CharSet.Auto, EntryPoint = "GetObject")]
    public static extern int GetObjectBITMAP(
      IntPtr hObject,
      int nSize,
      ref BITMAP lpBitmap);

    /// <summary>
    /// Creates a logical font that has the specified characteristics. The font can subsequently be selected
    /// as the current font for any device context.
    /// </summary>
    /// <remarks>
    /// To satisfy the FxCop rule SpecifyMarshalingForPInvokeStringArguments ( see http://msdn.microsoft.com/cs-
    /// cz/library/ms182319(en-us).aspx )
    /// we apply BestFitMapping = false. Marshalling of structure LOGFONT and of LOGFONT.lfFaceName must stay
    /// as it is now, regardless of FxCop complains.
    /// </remarks>
    /// <param name="lplf"> The reference to a LOGFONT structure that defines the characteristics of the logical
    /// font. </param>
    /// <returns>
    /// If the function succeeds, the return value is a handle to a logical font. If the function fails, the return
    /// value is IntPtr.Zero.
    /// </returns>
    [DllImport("gdi32", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    public static extern IntPtr CreateFontIndirect(
      [In, MarshalAs(UnmanagedType.LPStruct)] LOGFONT lplf);

    /// <summary>
    /// The GetTextMetrics function fills the specified buffer with the metrics for the currently selected
    /// font.
    /// </summary>
    /// <param name="hDC"> A handle to the device context. </param>
    /// <param name="lpMetrics"> A reference to the 
    /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd144941(v=vs.85).aspx">
    /// TEXTMETRIC</see> structure that receives the text metrics. </param>
    /// <returns> If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero.
    /// </returns>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd144941(v=vs.85).aspx">
    /// MSDN documentation about GetTextMetrics </seealso>
    [DllImport("gdi32", CharSet = CharSet.Auto, EntryPoint = "GetTextMetrics")]
    public static extern int GetTextMetrics(
      IntPtr hDC,
      [In, Out][MarshalAs(UnmanagedType.LPStruct)] TEXTMETRIC lpMetrics);

    /// <summary>
    /// The GdiSetBatchLimit function sets the maximum number of function calls that can be accumulated in
    /// the calling thread's current batch. The system flushes the current batch whenever this limit is
    /// exceeded.
    /// </summary>
    /// <param name="dwLimit"> Specifies the batch limit to be set. A value of 0 sets the default limit.
    ///  A value of 1 disables batching. </param>
    /// <returns> If the function succeeds, the return value is the previous batch limit. 
    ///  If the function fails, the return value is zero. </returns>
    [DllImport("gdi32")]
    public static extern uint GdiSetBatchLimit(
      uint dwLimit);

    // constants for GetDeviceCaps
    public const int LOGPIXELSX = 88;    // Logical pixels/inch in X
    public const int LOGPIXELSY = 90;    // Logical pixels/inch in Y

    /// <summary>	The GetDeviceCaps function retrieves device-specific information for the specified device. </summary>
    /// <param name="hDC">   	A handle to the device context. </param>
    /// <param name="nIndex">	The item to be returned. This parameter can be one of the several values;
    /// for more details, see the 
    /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd144877(v=vs.85).aspx">
    /// GetDeviceCaps description</see> on MSDN.
    /// </param>
    /// <returns> The return value specifies the value of the desired item. </returns>
    [DllImport("gdi32")]
    public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

    /// <summary>
    /// The GetTextExtentPoint32 function computes the width and height of the specified string of text.
    /// </summary>
    /// <param name="hDC">A handle to the device context.</param>
    /// <param name="lpsz">A buffer that specifies the text string. 
    /// The string does not need to be null-terminated, because the <paramref name="cbString"/> parameter 
    /// specifies the length of the string.</param>
    /// <param name="cbString">The length of the string pointed to by <paramref name="lpsz"/>.</param>
    /// <param name="lpSize">A reference to a SIZE structure that receives the dimensions of the string, 
    /// in logical units.</param>
    /// <returns></returns>
    [DllImport("gdi32", EntryPoint = "GetTextExtentPoint32")]
    public static extern int GetTextExtentPoint32(
      IntPtr hDC,
      [MarshalAs(UnmanagedType.LPWStr)] string lpsz,
      int cbString,
      ref WinApi.User32.SIZE lpSize);

    /// <summary>
    /// The Rectangle function draws a rectangle. The rectangle is outlined by using the current pen and
    /// filled by using the current brush.
    /// </summary>
    /// <param name="hDC"> The device-context. </param>
    /// <param name="left"> The x-coordinate, in logical coordinates, of the upper-left corner of the
    ///  rectangle. </param>
    /// <param name="top"> The y-coordinate, in logical coordinates, of the upper-left corner of the
    ///  rectangle. </param>
    /// <param name="right"> The x-coordinate, in logical coordinates, of the lower-right corner of the
    ///  rectangle. </param>
    /// <param name="bottom"> The y-coordinate, in logical coordinates, of the lower-right corner of the
    ///  rectangle. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("gdi32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool Rectangle(IntPtr hDC, int left, int top, int right, int bottom);

    /// <summary> The FillRgn function fills a region by using the specified brush.
    /// </summary>
    /// <param name="hDC"> Handle to the device context.</param>
    /// <param name="hRgn"> Handle to the region to be filled. 
    ///  The region's coordinates are presumed to be in logical units.</param>
    /// <param name="hBrush"> Handle to the brush to be used to fill the region. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("gdi32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FillRgn(IntPtr hDC, IntPtr hRgn, IntPtr hBrush);

    /// <summary> The CreateRectRgn function creates a rectangular region. </summary>
    /// <remarks>
    /// When you no longer need the region object created by this function, call the
    /// <see cref="DeleteObject"/> function to delete it.
    /// </remarks>
    /// <param name="X1"> The x-coordinate of the upper-left corner of the region in logical units. </param>
    /// <param name="Y1"> The y-coordinate of the upper-left corner of the region in logical units. </param>
    /// <param name="X2"> The x-coordinate of the lower-right corner of the region in logical units. </param>
    /// <param name="Y2"> The y-coordinate of the lower-right corner of the region in logical units. </param>
    /// <returns>
    /// If the function succeeds, the return value is the handle to the region. If the function fails,
    /// the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("gdi32")]
    public static extern IntPtr CreateRectRgn(int X1, int Y1, int X2, int Y2);

    /// <summary> The SetROP2 function sets the current foreground mix mode. 
    /// GDI uses the foreground mix mode to combine pens and interiors of filled objects with the colors 
    /// already on the screen. The foreground mix mode defines how colors from the brush or pen and 
    /// the colors in the existing image are to be combined. </summary>
    /// <param name="hDC"> A handle to the device context. </param>
    /// <param name="fnDrawMode"> The draw (mix) mode. </param>
    /// <returns> An int. </returns>
    [DllImport("gdi32")]
    public static extern int SetROP2(IntPtr hDC, Gdi32.DrawingMode fnDrawMode);

    /// <summary> The MoveToEx function updates the current position to the specified point and
    ///  returns the previous position.. </summary>
    /// <param name="hDC"> Handle to a device-context. </param>
    /// <param name="x"> The x coordinate, in logical units, of the new position. </param>
    /// <param name="y"> The y coordinate, in logical units, of the new position. </param>
    /// <param name="p"> [out] Reference to a POINT structure that receives the previous current position. 
    /// </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("gdi32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool MoveToEx(IntPtr hDC, int x, int y, ref Point p);

    /// <summary> Draws a line from the current position up to, but not including, the specified point. </summary>
    /// <param name="hDC"> Handle to a device context. </param>
    /// <param name="x"> The x coordinate, in logical units, of the line's ending point. </param>
    /// <param name="y"> The y coordinate, in logical units, of the line's ending point. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("gdi32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool LineTo(IntPtr hDC, int x, int y);

    /// <summary>
    /// The GetStockObject function retrieves a handle to one of the stock pens, brushes, fonts, or palettes.
    /// </summary>
    /// <param name="fnObject">The type of stock object. This parameter can be one of 
    /// <see cref="StockObject "/> enum values.</param>
    /// <returns> If the function succeeds, the return value is a handle to the requested logical object.
    /// If the function fails, the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("gdi32")]
    public static extern IntPtr GetStockObject(StockObject fnObject);

    /// <summary> The SelectObject function selects an object into the specified device context (DC). 
    /// The new object replaces the previous object of the same type. </summary>
    /// <param name="hDC"> A handle to the device context. </param>
    /// <param name="hObject"> A handle to a logical bitmap or a device independent bitmap or a brush or 
    ///  font or pen or region. </param>
    /// <returns> If the selected object is not a region and the function succeeds, 
    ///  the return value is a handle to the object being replaced. 
    ///  If the selected object is not a region and the function fails,
    ///  the return value is IntPtr.Zero.
    ///  For more information regarding the case when the selected object is a region, see 
    /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd162957(v=vs.85).aspx">
    /// SelectObject function</see>  MSDN help.
    ///  </returns>
    [DllImport("gdi32")]
    public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    /// <summary> Returns the current background color for the specified device context. </summary>
    /// <param name="hdc"> Handle to the device context whose background color is to be returned. </param>
    /// <returns> If the function succeeds, the return value is a <see cref="COLORREF"/> value 
    /// for the current background color.
    /// If the function fails, the return value is <see cref="CLR_INVALID"/>.</returns>
    [DllImport("gdi32")]
    public static extern int GetBkColor(IntPtr hdc);

    /// <summary> The SetBkMode function sets the background mix mode of the specified device context. 
    /// The background mix mode is used with text, hatched brushes, and pen styles that are not solid lines. 
    /// </summary>
    /// <param name="hDC"> A handle to the device context. </param>
    /// <param name="nMode"> The background mode. This parameter can be one of two <see cref="BKMODE"/>
    /// value. </param>
    /// <returns>
    ///  If the function succeeds, the return value specifies the previous background mode. 
    ///  If the function fails, the return value is zero.
    /// </returns>
    [DllImport("gdi32")]
    public static extern int SetBkMode(IntPtr hDC, BKMODE nMode);

    /// <summary>
    /// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing
    /// all system resources associated with the object. After the object is deleted, the specified handle
    /// is no longer valid.
    /// </summary>
    /// <param name="hObject"> A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("gdi32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject(IntPtr hObject);

    /// <summary>
    /// Performs a bit-block transfer of the color data corresponding to a rectangle of pixels from the specified
    /// source device context into a destination device context.
    /// </summary>
    /// <param name="hdcDest">  A handle to the destination device context. </param>
    /// <param name="nXDest">  The x-coordinate, in logical units, of the upper-left corner of the destination
    /// rectangle. </param>
    /// <param name="nYDest">  The y-coordinate, in logical units, of the upper-left corner of the destination
    /// rectangle. </param>
    /// <param name="nWidth">   The width, in logical units, of the source and destination rectangles. </param>
    /// <param name="nHeight"> The height, in logical units, of the source and the destination rectangles. </param>
    /// <param name="hdcSrc">   A handle to the source device context. </param>
    /// <param name="nXSrc">   The x-coordinate, in logical units, of the upper-left corner of the source
    /// rectangle. </param>
    /// <param name="nYSrc">   The y-coordinate, in logical units, of the upper-left corner of the source
    /// rectangle. </param>
    /// <param name="dwRop">   A raster-operation code. These codes define how the color data for the source
    /// rectangle is to be combined with the color data for the destination rectangle to achieve the final color. </param>
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("gdi32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool BitBlt(
      IntPtr hdcDest,
      int nXDest, int nYDest, int nWidth, int nHeight,
      IntPtr hdcSrc,
      int nXSrc, int nYSrc,
      TernaryRasterOperations dwRop);

    /// <summary>
    /// Copies a bitmap from a source rectangle into a destination rectangle, stretching or compressing the bitmap
    /// to fit the dimensions of the destination rectangle, if necessary. The system stretches or compresses the
    /// bitmap according to the stretching mode currently set in the destination device context.
    /// </summary>
    /// <param name="hdcDest">      A handle to the destination device context. </param>
    /// <param name="nXOriginDest"> The x-coordinate, in logical units, of the upper-left corner of the destination
    /// rectangle. </param>
    /// <param name="nYOriginDest"> The y-coordinate, in logical units, of the upper-left corner of the destination
    /// rectangle. </param>
    /// <param name="nWidthDest">   The width, in logical units, of the destination rectangle. </param>
    /// <param name="nHeightDest">  The height, in logical units, of the destination rectangle. </param>
    /// <param name="hdcSrc">       A handle to the source device context. </param>
    /// <param name="nXOriginSrc">  The x-coordinate, in logical units, of the upper-left corner of the source
    /// rectangle. </param>
    /// <param name="nYOriginSrc">  The y-coordinate, in logical units, of the upper-left corner of the source
    /// rectangle. </param>
    /// <param name="nWidthSrc">    The width, in logical units, of the source rectangle. </param>
    /// <param name="nHeightSrc">   The height, in logical units, of the source rectangle. </param>
    /// <param name="dwRop">        A raster-operation code. These codes define how the color data for the source
    /// rectangle is to be combined with the color data for the destination rectangle to achieve the final color. </param>
    /// <returns>
    /// true if it succeeds, false if it fails. To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("gdi32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool StretchBlt(
      IntPtr hdcDest,
      int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest,
      IntPtr hdcSrc,
      int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
      TernaryRasterOperations dwRop);

    /// <summary> The CreateCompatibleBitmap function creates a bitmap compatible with the device 
    ///  that is associated with the specified device context. 
    ///  </summary>
    /// <param name="hdc"> A handle to an existing Device Context. </param>
    /// <param name="nWidth"> The bitmap width, in pixels. </param>
    /// <param name="nHeight"> The bitmap height, in pixels. </param>
    /// <returns> If the function succeeds, the return value is a handle to the compatible bitmap (DDB).
    /// If the function fails, the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("gdi32", EntryPoint = "CreateCompatibleBitmap")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    /// <summary> The CreateBitmap function creates a bitmap with the specified width, height, 
    /// and color format (color planes and bits-per-pixel). </summary>
    /// <param name="nWidth"> The bitmap width, in pixels. </param>
    /// <param name="nHeight"> The bitmap height, in pixels. </param>
    /// <param name="cPlanes"> The number of color planes used by the device. </param>
    /// <param name="cBitsPerPel"> The number of bits required to identify the color of a single pixel. </param>
    /// <param name="lpvBits"> A pointer to an array of color data used to set the colors in a rectangle 
    /// of pixels. Each scan line in the rectangle must be word aligned 
    /// (scan lines that are not word aligned must be padded with zeros). 
    /// If this parameter is NULL, the contents of the new bitmap is undefined. </param>
    /// <returns> If the function succeeds, the return value is a handle to a bitmap. 
    ///           If the function fails, the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("gdi32")]
    public static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes,
       uint cBitsPerPel, IntPtr lpvBits);

    /// <summary>
    /// The CreateCompatibleDC function creates a memory device context (DC) compatible with the specified
    /// device. </summary>
    /// <param name="hdc"> A handle to an existing DC. If this handle is IntPtr.Zero, the function creates
    ///  a memory DC compatible with the application's current screen. </param>
    /// <returns> If the function succeeds, the return value is the handle to a memory DC.
    /// If the function fails, the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("gdi32", EntryPoint = "CreateCompatibleDC")]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    /// <summary> The DeleteDC function deletes the specified device context (DC). </summary>
    /// <remarks>
    /// An application must not delete a DC whose handle was obtained by calling the GetDC function.
    /// Instead, it must call the ReleaseDC function to free the DC.
    /// </remarks>
    /// <param name="hDC"> A handle to the device context. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("gdi32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteDC(IntPtr hDC);

    /// <summary>
    /// Creates a logical brush with the specified bitmap pattern. The bitmap can be a DIB section bitmap, which is
    /// created by the CreateDIBSection function, or it can be a device-dependent bitmap.
    /// </summary>
    /// <param name="hBitmap"> A handle to the bitmap to be used to create the logical brush. </param>
    /// <returns> If the function succeeds, the return value identifies a logical brush. 
    ///           If the function fails, the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("gdi32")]
    public static extern IntPtr CreatePatternBrush(IntPtr hBitmap);

    /// <summary> Specifies which device point maps to the window origin (0,0). </summary>
    ///
    /// <param name="hdc">      Handle to the device context. </param>
    /// <param name="nX">       The x-coordinate, in device units, of the new viewport origin. </param>
    /// <param name="nY">       The y-coordinate, in device units, of the new viewport origin. </param>
    /// <param name="lpPoint"> [out] A reference to a POINT structure that receives the previous viewport origin,
    /// in device coordinates. </param>
    ///
    /// <returns>
    /// If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero.
    /// </returns>
    [DllImport("gdi32")]
    public static extern int SetViewportOrgEx(
      IntPtr hdc, int nX, int nY, ref User32.POINT lpPoint);

    /// <summary>
    /// The CopyEnhMetaFile function copies the contents of an enhanced-format metafile to a specified file.
    /// </summary>
    /// <param name="hemfSrc">A handle to the enhanced metafile to be copied.
    /// </param>
    /// <param name="lpszFile">A pointer to the name of the destination file.
    /// If this parameter is NULL, the source metafile is copied to memory.
    /// </param>
    /// <returns>If the function succeeds, the return value is a handle to the copy of the enhanced metafile.
    /// If the function fails, the return value is NULL.
    /// </returns>
    /// <seealso href="http://www.pinvoke.net/default.aspx/gdi32.copyenhmetafile">
    /// pinvoke.net - CopyEnhMetaFile</seealso>
    [DllImport("gdi32")]
    public static extern IntPtr CopyEnhMetaFile(
      IntPtr hemfSrc,
      [MarshalAs(UnmanagedType.LPWStr)] string lpszFile);

    /// <summary>
    /// The DeleteEnhMetaFile function deletes an enhanced-format metafile or an enhanced-format metafile handle.
    /// </summary>
    /// <param name="hemf">A handle to an enhanced metafile.</param>
    /// <returns>
    /// If the function succeeds, the return value is true; otherwise false.
    /// </returns>
    [DllImport("gdi32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteEnhMetaFile(IntPtr hemf);

    #endregion //  Public External functions

    #region Internal External functions

    /// <summary> The CreatePen function creates a logical pen that has the specified style, width, and color. 
    /// The pen can subsequently be selected into a device context and used to draw lines and curves. </summary>
    ///
    /// <param name="fnPenStyle">   The pen style. </param>
    /// <param name="nWidth">       The width of the pen, in logical units. </param>
    /// <param name="crColor">      A color reference for the pen color. </param>
    ///
    /// <returns> The new pen handle. </returns>
    [DllImport("gdi32")]
    internal static extern IntPtr CreatePen(
      int fnPenStyle,
      int nWidth,
      uint crColor);

    /// <summary> Creates a logical brush that has the specified solid color. </summary>
    /// <param name="crColor"> The color of the brush. </param>
    /// <returns> If the function succeeds, the return value identifies a logical brush.
    /// If the function fails, the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("gdi32")]
    internal static extern IntPtr CreateSolidBrush(
      uint crColor);

    /// <summary>
    /// The SetBkColor function sets the current background color to the specified color value, or to the
    /// nearest physical color if the device cannot represent the specified color value.
    /// </summary>
    /// <remarks>
    /// This function fills the gaps between styled lines drawn using a pen created 
    /// by the CreatePen function; it does not fill the gaps between styled lines drawn using a pen 
    /// created by the ExtCreatePen function. 
    /// The SetBkColor function also sets the background colors for TextOut and ExtTextOut.
    ///
    /// If the background mode is OPAQUE, the background color is used to fill gaps between styled lines, 
    /// gaps between hatched lines in brushes, and character cells. The background color is also used when 
    /// converting bitmaps from color to monochrome and vice versa.
    /// </remarks>
    /// <param name="hDC"> Handle to the device context. </param>
    /// <param name="crColor"> The new background color. </param>
    /// <returns> If the function succeeds, the return value specifies the previous background color 
    /// as a COLORREF value. 
    /// If the function fails, the return value is <see cref="CLR_INVALID"/>.</returns>
    [DllImport("gdi32")]
    internal static extern int SetBkColor(
        IntPtr hDC,
        uint crColor);

    /// <summary>
    /// The SetTextColor function sets the text color for the specified device context to the specified
    /// color.
    /// </summary>
    /// <param name="hDC"> Handle to the device context whose text color is to be set. </param>
    /// <param name="crColor"> The color of the text. </param>
    /// <returns> If the function succeeds, the return value is a color reference for the previous 
    /// text color as a <see cref="COLORREF"/> value. 
    /// If the function fails, the return value is <see cref="CLR_INVALID"/>.
    /// </returns>
    [DllImport("gdi32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.I4)]
    internal static extern COLORREF SetTextColor(
        IntPtr hDC,
        uint crColor);
    #endregion // Internal External functions
    #endregion //  External functions

    #region Constructor(s)

    /// <summary>
    /// Static constructor, performs PrelinkAll check
    /// </summary>
    static Gdi32()
    {
        try
        {
            Marshal.PrelinkAll(typeof(Gdi32));
        }
#if DEBUG
        catch (Exception ex)
        {
            string strMsg = string.Format(CultureInfo.InvariantCulture,
                "PrelinkAll failed for '{0}', with exception: '{1}', stack trace '{2}'",
                typeof(Gdi32).ToString(), ex.Message, ex.StackTrace);
            Debug.Fail(strMsg);
            throw;
        }
#else
        catch (Exception)
        {
            throw;
        }
#endif // DEBUG
    }
    #endregion // Constructor(s)
}
#endregion // Gdi32 class

#region GDI class
/// <summary>
/// Provides utilities directly accessing the gdi32.dll 
/// </summary>
public static class GDI
{
    #region Fields

    private static Point _nullPoint = new(0, 0);
    #endregion // Fields

    #region Methods

    /// <summary> Convert color from the Argb .NET format to a gdi32 RGB. </summary>
    /// <param name="argb"> The .NET 32-bit ARGB value, which is in format AARRGGBB. </param>
    /// <returns> A COLORREF value, which has a form 0x00bbggrr. </returns>
    public static COLORREF ArgbToRGB(int argb)
    {
        return (COLORREF)(unchecked((uint)((argb >> 16 & 0x0000FF) | (argb & 0x00FF00) | (argb << 16 & 0xFF0000))));
    }

    /// <summary> Conversion form Win32 color ( which has a format 0x00bbggrr) to .NET color. </summary>
    /// <param name="color"> The Win32 color. </param>
    /// <returns> A .NET <see cref="System.Drawing.Color"/> value. </returns>
    public static Color FromWin32Color(int color)
    {
        int r = color & 0x000000FF;
        int g = (color & 0x0000FF00) >> 8;
        int b = (color & 0x00FF0000) >> 16;
        return Color.FromArgb(r, g, b);
    }

    /// <summary> Conversion from .NET color to Win32 color ( which has a format 0x00bbggrr). </summary>
    /// <param name="c"> The <see cref="System.Drawing.Color"/> value to process. </param>
    /// <returns> The Win32 color. </returns>
    public static COLORREF ToWin32Color(System.Drawing.Color c)
    {
        return new COLORREF(c.R, c.G, c.B);
    }

    /// <summary> Creates a <see cref="COLORREF"/> from color components. </summary>
    /// <param name="red"> The red component. </param>
    /// <param name="green"> The green component. </param>
    /// <param name="blue"> The blue component. </param>
    /// <returns> A new COLORREF. </returns>
    public static COLORREF RGB(byte red, byte green, byte blue)
    {
        return new COLORREF(red, green, blue);
    }

    /// <summary> Draw exclusive-or rectangle. </summary>
    /// <param name="graphics"> The destination graphics object encapsulating a GDI+ drawing surface. </param>
    /// <param name="pen"> The pen. </param>
    /// <param name="rectangle"> The rectangle to be painted. </param>
    /// <seealso cref="DrawXORLine"/>
    public static void DrawXORRectangle(Graphics graphics, Pen pen, Rectangle rectangle)
    {
        IntPtr hDC = graphics.GetHdc();
        IntPtr hPen = GDI.CreatePen(0, (int)pen.Width, ArgbToRGB(pen.Color.ToArgb()));
        Gdi32.SelectObject(hDC, hPen);
        Gdi32.SetROP2(hDC, Gdi32.DrawingMode.R2_NOTXORPEN);
        Gdi32.Rectangle(hDC, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        Gdi32.DeleteObject(hPen);
        graphics.ReleaseHdc(hDC);
    }

    /// <summary> Draw exclusive-or line. </summary>
    /// <param name="graphics"> The destination graphics object encapsulating a GDI+ drawing surface. </param>
    /// <param name="pen"> The pen. </param>
    /// <param name="x1"> The first point x-coordinate value. </param>
    /// <param name="y1"> The first point y-coordinate value. </param>
    /// <param name="x2"> The second point x-coordinate value. </param>
    /// <param name="y2"> The second point y-coordinate value. </param>
    /// <seealso cref="DrawXORRectangle"/>
    public static void DrawXORLine(Graphics graphics, Pen pen, int x1, int y1, int x2, int y2)
    {
        IntPtr hDC = graphics.GetHdc();
        IntPtr hPen = GDI.CreatePen(0, (int)pen.Width, ArgbToRGB(pen.Color.ToArgb()));
        Gdi32.SelectObject(hDC, hPen);
        Gdi32.SetROP2(hDC, Gdi32.DrawingMode.R2_NOTXORPEN);
        Gdi32.MoveToEx(hDC, x1, y1, ref _nullPoint);
        Gdi32.LineTo(hDC, x2, y2);
        Gdi32.DeleteObject(hPen);
        graphics.ReleaseHdc(hDC);
    }

    /// <summary> Fill exclusive-or region. </summary>
    /// <param name="graphics"> The destination graphics object encapsulating a GDI+ drawing surface. </param>
    /// <param name="brush"> The brush to be used for painting. </param>
    /// <param name="rectangle"> The rectangle to be painted. </param>
    public static void FillXORRgn(Graphics graphics, SolidBrush brush, Rectangle rectangle)
    {
        IntPtr hDC = graphics.GetHdc();
        IntPtr hBrush = GDI.CreateSolidBrush(ArgbToRGB(brush.Color.ToArgb()));
        IntPtr hRgn = Gdi32.CreateRectRgn(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        Gdi32.SelectObject(hDC, hBrush);
        Gdi32.SetROP2(hDC, Gdi32.DrawingMode.R2_NOTXORPEN);
        Gdi32.FillRgn(hDC, hRgn, hBrush);
        Gdi32.DeleteObject(hBrush);
        graphics.ReleaseHdc(hDC);
    }

    /// <summary> The CreatePen function creates a logical pen that has the specified style, width, and color. 
    /// The pen can subsequently be selected into a device context and used to draw lines and curves. </summary>
    ///
    /// <param name="fnPenStyle">   The pen style. </param>
    /// <param name="nWidth">       The width of the pen, in logical units. </param>
    /// <param name="crColor">      A color reference for the pen color. </param>
    ///
    /// <returns> The new pen handle. </returns>
    public static IntPtr CreatePen(
      int fnPenStyle,
      int nWidth,
      COLORREF crColor)
    {
        return Gdi32.CreatePen(fnPenStyle, nWidth, (uint)crColor);
    }

    /// <summary> Creates a logical brush that has the specified solid color. </summary>
    /// <param name="crColor"> The color of the brush. </param>
    /// <returns> If the function succeeds, the return value identifies a logical brush.
    /// If the function fails, the return value is IntPtr.Zero.
    /// </returns>
    public static IntPtr CreateSolidBrush(COLORREF crColor)
    {
        return Gdi32.CreateSolidBrush((uint)crColor);
    }

    /// <summary>
    /// The SetBkColor function sets the current background color to the specified color value, or to the
    /// nearest physical color if the device cannot represent the specified color value.
    /// </summary>
    /// <remarks>
    /// This function fills the gaps between styled lines drawn using a pen created 
    /// by the CreatePen function; it does not fill the gaps between styled lines drawn using a pen 
    /// created by the ExtCreatePen function. 
    /// The SetBkColor function also sets the background colors for TextOut and ExtTextOut.
    ///
    /// If the background mode is OPAQUE, the background color is used to fill gaps between styled lines, 
    /// gaps between hatched lines in brushes, and character cells. The background color is also used when 
    /// converting bitmaps from color to monochrome and vice versa.
    /// </remarks>
    /// <param name="hDC"> Handle to the device context. </param>
    /// <param name="crColor"> The new background color. </param>
    /// <returns> If the function succeeds, the return value specifies the previous background color 
    /// as a COLORREF value. 
    /// If the function fails, the return value is <see cref="Gdi32.CLR_INVALID"/>.</returns>
    public static int SetBkColor(
        IntPtr hDC,
        COLORREF crColor)
    {
        return Gdi32.SetBkColor(hDC, (uint)crColor);
    }

    /// <summary>
    /// The SetTextColor function sets the text color for the specified device context to the specified
    /// color.
    /// </summary>
    /// <param name="hDC"> Handle to the device context whose text color is to be set. </param>
    /// <param name="crColor"> The color of the text. </param>
    /// <returns> If the function succeeds, the return value is a color reference for the previous 
    /// text color as a <see cref="COLORREF"/> value. 
    /// If the function fails, the return value is <see cref="Gdi32.CLR_INVALID"/>.
    /// </returns>
    public static COLORREF SetTextColor(
        IntPtr hDC,
        [MarshalAs(UnmanagedType.I4)] COLORREF crColor)
    {
        return Gdi32.SetTextColor(hDC, (uint)crColor);
    }
    #endregion // Methods
}
#endregion // GDI class

#pragma warning restore SYSLIB1054
#pragma warning restore CA1806
#pragma warning restore CA1401
#pragma warning restore 1591

