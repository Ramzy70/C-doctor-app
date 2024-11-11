using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class RoundedRichTextBox : RichTextBox
{
    public RoundedRichTextBox()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        UpdateStyles();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using (GraphicsPath path = new GraphicsPath())
        {
            path.AddArc(0, 0, 8, 8, 180, 90);
            path.AddArc(Width - 8, 0, 8, 8, 270, 90);
            path.AddArc(Width - 8, Height - 8, 8, 8, 0, 90);
            path.AddArc(0, Height - 8, 8, 8, 90, 90);
            path.CloseFigure();

            this.Region = new Region(path);
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        this.Invalidate();
    }
}
