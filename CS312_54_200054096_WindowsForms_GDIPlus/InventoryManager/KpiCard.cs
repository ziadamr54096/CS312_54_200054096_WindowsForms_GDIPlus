using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace InventoryManager;

/// <summary>Rounded, gradient KPI card (GDI+). Drag it from the Toolbox; Caption, Value and Accent are editable in the Properties window.</summary>
[ToolboxItem(true)]
[DefaultProperty("Caption")]
public class KpiCard : Control
{
    string _caption = "";
    string _value = "-";
    Color _accent = Color.SteelBlue;

    public KpiCard()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    [Category("Appearance"), DefaultValue("")]
    public string Caption { get => _caption; set { _caption = value ?? ""; Invalidate(); } }

    [Category("Appearance"), DefaultValue(typeof(Color), "SteelBlue")]
    public Color Accent { get => _accent; set { _accent = value; Invalidate(); } }

    [Category("Appearance"), DefaultValue("-")]
    public string Value { get => _value; set { _value = value ?? ""; Invalidate(); } }

    protected override Size DefaultSize => new Size(200, 90);

    public static GraphicsPath Rounded(Rectangle r, int radius)
    {
        var p = new GraphicsPath();
        int d = radius * 2;
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        g.Clear(Parent?.BackColor ?? SystemColors.Control);
        if (Width < 30 || Height < 30) return;

        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = Rounded(r, 10);
        using var fill = new LinearGradientBrush(r, Color.White, Color.FromArgb(238, 243, 250), 90f);
        g.FillPath(fill, path);
        using var border = new Pen(Color.FromArgb(210, 218, 230));
        g.DrawPath(border, path);

        g.SetClip(path);
        using var accent = new SolidBrush(Accent);
        g.FillRectangle(accent, 0, 0, 7, Height);
        g.ResetClip();

        using var capFont = new Font("Segoe UI", 9.5f);
        using var bigFont = new Font("Segoe UI Semibold", 20f);
        g.DrawString(Caption, capFont, Brushes.DimGray, 18, 12);
        g.DrawString(_value, bigFont, Brushes.Black, 16, 36);
    }
}
