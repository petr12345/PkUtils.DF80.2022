///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Microsoft Copyright notice
// 
// This software is a Derivative Work based upon a Microsoft Support article
// "How to create a smooth progress bar in Visual C#"
// http://support.microsoft.com/kb/323116
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Ignore Spelling: Utils
//
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.Utils;

namespace PK.PkUtils.UI.Controls;

/// <summary>
/// SmoothProgressBar works as a WinForms progress meter, but with modified painting.
/// Its colors are determined by following rules:
/// - The left part of the progress bar is painted with color property BarColorLeft 
/// ( default value SystemColors.ActiveCaption ).
/// - The right part of the progress bar is painted with color property BackColor
/// ( default value SystemColors.Control ).
/// - The text is painted with the color property BarColorText 
/// ( default value SystemColors.ActiveCaptionText ).
/// 
/// </summary>
public partial class SmoothProgressBar : UserControl
{
    #region Fields

    /// <summary> Minimum value for progress range </summary>
    private int _min;  /* = 0; */

    /// <summary> Maximum value for progress range </summary>
    private int _max = 100;

    /// <summary> Current progress </summary>
    private int _val;  /* = 0; */

    /// <summary> Color of progress meter </summary>
    private Color _barColorLeft = SystemColors.ActiveCaption;

    /// <summary> Color of text </summary>
    private Color _barColorText = SystemColors.ActiveCaptionText;

    /// <summary> displayed text on progress meter </summary>
    private string _text = "Progress text";

    /// <summary>
    /// Auxiliary brush - for drawing the left part of the progress bar.
    /// </summary>
    private SolidBrush _brLeft;

    /// <summary>
    /// Auxiliary brush - for drawing the text
    /// </summary>
    private SolidBrush _brText;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor
    /// </summary>
    public SmoothProgressBar()
    {
        InitializeComponent();
        InitializeColors();
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// This property obtains or sets the lower value for the range of valid values for progress. The default
    /// value of this property is zero (0); you cannot set this property to a negative value. </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int Minimum
    {
        get
        {
            return _min;
        }

        set
        {
            // Prevent a negative value.
            if (value < 0)
            {
                _min = 0;
            }

            // Make sure that the minimum value is never set higher than the maximum value.
            if (value > _max)
            {
                _min = value;
                _min = value;
            }

            // Ensure value is still in range
            if (_val < _min)
            {
                _val = _min;
            }

            // Invalidate the control to get a repaint.
            this.Invalidate();
        }
    }

    /// <summary>
    /// This property obtains or sets the upper value for the range of valid values for progress. The default
    /// value of this property is 100. </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int Maximum
    {
        get
        {
            return _max;
        }

        set
        {
            // Make sure that the maximum value is never set lower than the minimum value.
            if (value < _min)
            {
                _min = value;
            }

            _max = value;

            // Make sure that value is still in range.
            if (_val > _max)
            {
                _val = _max;
            }

            // Invalidate the control to get a repaint.
            this.Invalidate();
        }
    }

    /// <summary>
    /// This property obtains or sets the current level of progress. The value must be in the range that the
    /// Minimum and the Maximum properties define. </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int Value
    {
        get
        {
            return _val;
        }

        set
        {
            int oldValue = _val;

            // Make sure that the value does not stray outside the valid range.
            if (value < _min)
            {
                _val = _min;
            }
            else if (value > _max)
            {
                _val = _max;
            }
            else
            {
                _val = value;
            }

            // Invalidate only the changed area - calculate the updateRect first
            float percent;

            Rectangle newValueRect = this.ClientRectangle;
            Rectangle oldValueRect = this.ClientRectangle;

            // Use a new value to calculate the rectangle for progress.
            percent = (_val - _min) / (float)(_max - _min);
            newValueRect.Width = (int)(newValueRect.Width * percent);

            // Use an old value to calculate the rectangle for progress.
            percent = (oldValue - _min) / (float)(_max - _min);
            oldValueRect.Width = (int)(oldValueRect.Width * percent);

            Rectangle updateRect = new();

            // Find only the part of the screen that must be updated.
            if (newValueRect.Width > oldValueRect.Width)
            {
                updateRect.X = oldValueRect.Size.Width;
                updateRect.Width = newValueRect.Width - oldValueRect.Width;
            }
            else
            {
                updateRect.X = newValueRect.Size.Width;
                updateRect.Width = oldValueRect.Width - newValueRect.Width;
            }

            updateRect.Height = this.Height;

            // Invalidate the intersection region only.
            this.Invalidate(updateRect);
        }
    }

    /// <summary>
    /// The color of the left part of the progress bar
    /// </summary>
    [BrowsableAttribute(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BarColorLeft
    {
        get
        {
            return _barColorLeft;
        }
        set
        {
            if (BarColorLeft != value)
            {
                _barColorLeft = value;
                InitializeColors();
                // Invalidate the control to get a repaint.
                this.Invalidate();
            }
        }
    }

    /// <summary>
    /// The color of the text in the progress bar
    /// </summary>
    [BrowsableAttribute(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BarColorText
    {
        get
        {
            return _barColorText;
        }
        set
        {
            if (BarColorText != value)
            {
                _barColorText = value;
                InitializeColors();
                // Invalidate the control to get a repaint.
                this.Invalidate();
            }
        }
    }

    /// <summary>
    /// The the text in the progress bar
    /// </summary>
    [BrowsableAttribute(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string BarText
    {
        get
        {
            return _text;
        }
        set
        {
            if (BarText != value)
            {
                _text = value;
                this.Invalidate();
            }
        }
    }
    #endregion // Properties

    #region implementation of IDisposable

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Disposer.SafeDispose(ref components);
            Disposer.SafeDispose(ref _brLeft);
            Disposer.SafeDispose(ref _brText);
        }

        base.Dispose(disposing);
    }

    #endregion // implementation of IDisposable

    #region Methods

    /// <summary>
    /// Implementation helper - initialization
    /// </summary>
    protected void InitializeColors()
    {
        _brLeft = new SolidBrush(BarColorLeft);
        _brText = new SolidBrush(BarColorText);
    }

    /// <summary>
    /// Overrides the method of the base class, to invalidate the control on resize
    /// </summary>
    /// <param name="args">The resize event arguments </param>
    protected override void OnResize(EventArgs args)
    {
        // Invalidate the control to get a repaint.
        this.Invalidate();
    }

    /// <summary> Overrides the method of the base class to do custom painting. </summary>
    ///
    /// <param name="args"> A PaintEventArgs specifies the Graphics to use to paint the control and the
    /// ClipRectangle in which to paint. </param>
    protected override void OnPaint(PaintEventArgs args)
    {
        Graphics g = args.Graphics;
        float percent = (_val - _min) / (float)(_max - _min);
        Rectangle cl_rect = this.ClientRectangle;
        Rectangle rectL = cl_rect;

        // Calculate area for drawing the progress.
        rectL.Width = (int)(rectL.Width * percent);

        // Draw the progress meter - left part
        g.FillRectangle(_brLeft, rectL);

        // Draw a three-dimensional border around the control.
        Draw3DBorder(g);

        // Draw text - left and right part
        Font font;
        Font thisFont = this.Font;
        string text = this.BarText;
        StringFormat sf = new();
        Rectangle centered = cl_rect;
        int deltaY;

        sf.Alignment = StringAlignment.Center;
        if (0 != ((int)thisFont.Style & (int)FontStyle.Bold))
            font = thisFont;
        else
            font = new Font(thisFont.Name, thisFont.Size, FontStyle.Bold);
        deltaY = (int)((centered.Height - args.Graphics.MeasureString(text, font).Height) / 2);

        centered.Offset(0, deltaY);
        g.DrawString(text, font, _brText, centered, sf);
    }

    /// <summary>
    /// Draw a three-dimensional border around the control.
    /// </summary>
    /// <param name="g"></param>
    private void Draw3DBorder(Graphics g)
    {
        int PenWidth = (int)Pens.White.Width;

        g.DrawLine(Pens.DarkGray,
            new Point(this.ClientRectangle.Left, this.ClientRectangle.Top),
            new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Top));
        g.DrawLine(Pens.DarkGray,
            new Point(this.ClientRectangle.Left, this.ClientRectangle.Top),
            new Point(this.ClientRectangle.Left, this.ClientRectangle.Height - PenWidth));
        g.DrawLine(Pens.White,
            new Point(this.ClientRectangle.Left, this.ClientRectangle.Height - PenWidth),
            new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Height - PenWidth));
        g.DrawLine(Pens.White,
            new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Top),
            new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Height - PenWidth));
    }
    #endregion // Methods
}
