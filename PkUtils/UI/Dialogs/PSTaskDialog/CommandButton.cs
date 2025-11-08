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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PK.PkUtils.UI.Dialogs.PSTaskDialog;

public partial class CommandButton : Button
{
    //--------------------------------------------------------------------------------
    #region PRIVATE MEMBERS
    //--------------------------------------------------------------------------------
    private Image _imgArrow1;
    private Image _imgArrow2;
    private const int LEFT_MARGIN = 10;
    private const int TOP_MARGIN = 10;
    private const int ARROW_WIDTH = 19;

    private enum ButtonState { Normal, MouseOver, Down }

    private ButtonState _state = ButtonState.Normal;

    #endregion

    //--------------------------------------------------------------------------------
    #region PUBLIC PROPERTIES
    //--------------------------------------------------------------------------------
    // Override this to make sure the control is invalidated (repainted) when 'Text' is changed

    /// <summary>
    /// Represents the text of the control. Overrides the implementation of the base class,
    /// to perform additional processing for case <see cref="AutoHeight"/> is set to true.
    /// </summary>
    /// <remarks> Override this to make sure the control is invalidated (repainted) when 'Text' is changed</remarks>
    public override string Text
    {
        get { return base.Text; }
        set
        {
            base.Text = value;
            if (AutoHeight)
            {
                this.Height = GetBestHeight();
            }
            this.Invalidate();
        }
    }

    private Font _smallFont;

    /// <summary>
    /// SmallFont is the font used for secondary lines
    /// </summary>
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Font SmallFont
    {
        get { return _smallFont; }
        set { _smallFont = value; }
    }

    private bool _autoHeight = true;
    /// <summary>
    /// AutoHeight determines whether the button automatically resizes itself to fit the Text
    /// </summary>
    [Browsable(true)]
    [Category("Behavior")]
    [DefaultValue(true)]
    public bool AutoHeight
    {
        get
        {
            return _autoHeight;
        }
        set
        {
            _autoHeight = value;
            if (AutoHeight)
            {
                this.Invalidate();
            }
        }
    }

    #endregion

    //--------------------------------------------------------------------------------
    #region CONSTRUCTOR
    //--------------------------------------------------------------------------------

    /// <summary>
    /// Public argument-less constructor.
    /// </summary>
    public CommandButton()
    {
        InitializeComponent();
        base.Font = new Font("Arial", 11.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        _smallFont = new Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
    }

    #endregion

    //--------------------------------------------------------------------------------
    #region PUBLIC ROUTINES
    //--------------------------------------------------------------------------------

    /// <summary>
    /// Computes the 'best' height of this control.
    /// </summary>
    /// <returns>This method returns a Height part of SizeF structure that represents the size,
    /// (in the units specified by the PageUnit property), .<br/>
    /// For more details, see the
    /// <see href="http://msdn.microsoft.com/en-us/library/6xe5hazb(v=vs.90).aspx"> Graphics.MeasureString </see>
    /// method description.
    /// </returns>
    public int GetBestHeight()
    {
        return (TOP_MARGIN * 2) + (int)GetSmallTextSizeF().Height + (int)GetLargeTextSizeF().Height;
    }
    #endregion

    //--------------------------------------------------------------------------------
    #region PRIVATE ROUTINES
    //--------------------------------------------------------------------------------
    private string GetLargeText()
    {
        string[] lines = this.Text.Split(['\n']);
        return lines[0];
    }

    private string GetSmallText()
    {
        if (this.Text.IndexOf('\n') < 0)
            return "";

        string s = this.Text;
        string[] lines = s.Split(['\n']);
        s = "";
        for (int i = 1; i < lines.Length; i++)
            s += lines[i] + "\n";
        return s.Trim(['\n']);
    }

    private SizeF GetLargeTextSizeF()
    {
        int x = LEFT_MARGIN + ARROW_WIDTH + 5;
        SizeF mzSize = new(this.Width - x - LEFT_MARGIN, 5000.0F);  // presume RIGHT_MARGIN = LEFT_MARGIN
        Graphics g = Graphics.FromHwnd(this.Handle);
        SizeF textSize = g.MeasureString(GetLargeText(), base.Font, mzSize);
        return textSize;
    }

    private SizeF GetSmallTextSizeF()
    {
        string s = GetSmallText();
        if (string.IsNullOrEmpty(s)) return new SizeF(0, 0);
        int x = LEFT_MARGIN + ARROW_WIDTH + 8; // <- indent small text slightly more
        SizeF mzSize = new(this.Width - x - LEFT_MARGIN, 5000.0F);  // presume RIGHT_MARGIN = LEFT_MARGIN
        Graphics g = Graphics.FromHwnd(this.Handle);
        SizeF textSize = g.MeasureString(s, _smallFont, mzSize);
        return textSize;
    }
    #endregion

    //--------------------------------------------------------------------------------
    #region OVERRIDEs
    //--------------------------------------------------------------------------------

    /// <summary>
    ///  Overrides the implementation of the base class, in order to perform custom painting.
    /// </summary>
    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        _imgArrow1 = LoadEmbeddedImage("green_arrow1.png");
        _imgArrow2 = LoadEmbeddedImage("green_arrow2.png");
    }

    /// <summary>
    /// Loads an embedded image from the control's assembly by searching manifest resource names
    /// and returns a fully independent Bitmap (stream safely disposed).
    /// </summary>
    /// <param name="fileName">The file name of the embedded resource (e.g. "green_arrow1.png").</param>
    protected Bitmap LoadEmbeddedImage(string fileName)
    {
        Bitmap result = null;
        Assembly asm = this.GetType().Assembly;

        // Find a resource name that ends with the provided file name (case-insensitive).
        string resourceName = null;
        foreach (string name in asm.GetManifestResourceNames())
        {
            if (name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
            {
                resourceName = name;
                break;
            }
        }

        if (resourceName != null)
        {
            using Stream stream = asm.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                // Create a temporary Image from the stream then clone into a Bitmap so the stream can be closed.
                using Image tmp = Image.FromStream(stream);
                result = new Bitmap(tmp);
            }
        }

        return result;
    }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Overrides the implementation of the base class, in order to perform custom painting.
    /// </summary>
    ///
    /// <param name="args"> A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data. </param>
    protected override void OnPaint(PaintEventArgs args)
    {
        args.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        args.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        LinearGradientBrush brush;
        LinearGradientMode mode = LinearGradientMode.Vertical;

        Rectangle newRect = new(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
        Color text_color = SystemColors.WindowText;

        Image img = _imgArrow1;

        if (Enabled)
        {
            switch (_state)
            {
                case ButtonState.Normal:
                    args.Graphics.FillRectangle(Brushes.White, newRect);
                    if (base.Focused)
                        args.Graphics.DrawRectangle(new Pen(Color.SkyBlue, 1), newRect);
                    else
                        args.Graphics.DrawRectangle(new Pen(Color.White, 1), newRect);
                    text_color = Color.DarkBlue;
                    break;

                case ButtonState.MouseOver:
                    brush = new LinearGradientBrush(newRect, Color.White, Color.WhiteSmoke, mode);
                    args.Graphics.FillRectangle(brush, newRect);
                    args.Graphics.DrawRectangle(new Pen(Color.Silver, 1), newRect);
                    img = _imgArrow2;
                    text_color = Color.Blue;
                    break;

                case ButtonState.Down:
                    brush = new LinearGradientBrush(newRect, Color.WhiteSmoke, Color.White, mode);
                    args.Graphics.FillRectangle(brush, newRect);
                    args.Graphics.DrawRectangle(new Pen(Color.DarkGray, 1), newRect);
                    text_color = Color.DarkBlue;
                    break;
            }
        }
        else
        {
            brush = new LinearGradientBrush(newRect, Color.WhiteSmoke, Color.Gainsboro, mode);
            args.Graphics.FillRectangle(brush, newRect);
            args.Graphics.DrawRectangle(new Pen(Color.DarkGray, 1), newRect);
            text_color = Color.DarkBlue;
        }

        string largeText = this.GetLargeText();
        string smallText = this.GetSmallText();

        SizeF szL = GetLargeTextSizeF();
        //e.Graphics.DrawString(largeText, base.Font, new SolidBrush(text_color), new RectangleF(new PointF(LEFT_MARGIN + _imgArrow1.Width + 5, TOP_MARGIN), szL));
        TextRenderer.DrawText(args.Graphics, largeText, base.Font, new Rectangle(LEFT_MARGIN + _imgArrow1.Width + 5, TOP_MARGIN, (int)szL.Width, (int)szL.Height), text_color, TextFormatFlags.Default);

        if (!string.IsNullOrEmpty(smallText))
        {
            SizeF szS = GetSmallTextSizeF();
            args.Graphics.DrawString(smallText, _smallFont, new SolidBrush(text_color), new RectangleF(new PointF(LEFT_MARGIN + _imgArrow1.Width + 8, TOP_MARGIN + (int)szL.Height), szS));
        }

        args.Graphics.DrawImage(img, new Point(LEFT_MARGIN, TOP_MARGIN + (int)(szL.Height / 2) - img.Height / 2));
    }

    //--------------------------------------------------------------------------------
    /// <summary> Overrides the implementation of the base class, in order to perform additional processing. </summary>
    ///
    /// <param name="args">  An EventArgs that contains the event data. </param>
    protected override void OnMouseLeave(System.EventArgs args)
    {
        _state = ButtonState.Normal;
        this.Invalidate();
        base.OnMouseLeave(args);
    }

    //--------------------------------------------------------------------------------
    /// <summary> Overrides the implementation of the base class, in order to perform additional processing. </summary>
    ///
    /// <param name="args">  An EventArgs that contains the event data. </param>
    protected override void OnMouseEnter(EventArgs args)
    {
        _state = ButtonState.MouseOver;
        this.Invalidate();
        base.OnMouseEnter(args);
    }

    //--------------------------------------------------------------------------------
    /// <summary> Overrides the implementation of the base class, in order to perform additional processing. </summary>
    ///
    /// <param name="args">  An EventArgs that contains the event data. </param>
    protected override void OnMouseUp(MouseEventArgs args)
    {
        _state = ButtonState.MouseOver;
        this.Invalidate();
        base.OnMouseUp(args);
    }

    //--------------------------------------------------------------------------------
    /// <summary> Overrides the implementation of the base class, in order to perform additional processing. </summary>
    ///
    /// <param name="args">  An EventArgs that contains the event data. </param>
    protected override void OnMouseDown(MouseEventArgs args)
    {
        _state = ButtonState.Down;
        this.Invalidate();
        base.OnMouseDown(args);
    }

    //--------------------------------------------------------------------------------
    /// <summary> Overrides the implementation of the base class, in order to perform additional processing. </summary>
    ///
    /// <param name="args">  An <see cref="T:System.EventArgs" /> that contains the event data. </param>
    protected override void OnSizeChanged(EventArgs args)
    {
        if (AutoHeight)
        {
            int h = GetBestHeight();
            if (this.Height != h)
            {
                this.Height = h;
                return;
            }
        }
        base.OnSizeChanged(args);
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (components != null)
            {
                components.Dispose();
                components = null;
            }
            if (_imgArrow1 != null)
            {
                _imgArrow1.Dispose();
                _imgArrow1 = null;
            }
            if (_imgArrow2 != null)
            {
                _imgArrow2.Dispose();
                _imgArrow2 = null;
            }
            if (_smallFont != null)
            {
                _smallFont.Dispose();
                _smallFont = null;
            }
        }
        base.Dispose(disposing);
    }
    #endregion // OVERRIDEs

    //--------------------------------------------------------------------------------
}
