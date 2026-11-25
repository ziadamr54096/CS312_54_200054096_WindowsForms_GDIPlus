using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace InventoryManager.Auth;

/// <summary>Stockora banner (GDI+): gradient background, drawn box logo, product name and tagline. Droppable from the Toolbox.</summary>
[ToolboxItem(true)]
[DefaultProperty("BrandName")]
public class BrandHeader : Control
{
    string _brand = "Stockora";
    string _tagline = "Inventory Management System";

    public BrandHeader()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    [Category("Appearance"), DefaultValue("Stockora")]
    public string BrandName { get => _brand; set { _brand = value ?? ""; Invalidate(); } }

    [Category("Appearance"), DefaultValue("Inventory Management System")]
    public string Tagline { get => _tagline; set { _tagline = value ?? ""; Invalidate(); } }

    protected override Size DefaultSize => new Size(420, 110);

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        if (Width < 20 || Height < 20) { g.Clear(Color.FromArgb(38, 70, 120)); return; }

        var rect = new Rectangle(0, 0, Width, Height);
        using (var bg = new LinearGradientBrush(rect, Color.FromArgb(30, 58, 104), Color.FromArgb(52, 112, 196), 20f))
            g.FillRectangle(bg, rect);

        // logo: rounded tile with an open box
        int size = Math.Min(Height - 36, 60);
        var tile = new Rectangle(28, (Height - size) / 2, size, size);
        using (var path = KpiCard.Rounded(tile, 12))
        using (var white = new SolidBrush(Color.FromArgb(235, 242, 252)))
            g.FillPath(white, path);

        float cx = tile.X + size / 2f, cy = tile.Y + size / 2f, u = size / 100f;
        using (var top = new SolidBrush(Color.FromArgb(120, 170, 235)))
        using (var left = new SolidBrush(Color.FromArgb(52, 112, 196)))
        using (var right = new SolidBrush(Color.FromArgb(30, 70, 140)))
        {
            g.FillPolygon(top, new[] { P(cx, cy - 30 * u), P(cx + 28 * u, cy - 15 * u), P(cx, cy), P(cx - 28 * u, cy - 15 * u) });
            g.FillPolygon(left, new[] { P(cx - 28 * u, cy - 15 * u), P(cx, cy), P(cx, cy + 32 * u), P(cx - 28 * u, cy + 17 * u) });
            g.FillPolygon(right, new[] { P(cx + 28 * u, cy - 15 * u), P(cx, cy), P(cx, cy + 32 * u), P(cx + 28 * u, cy + 17 * u) });
        }

        using var nameFont = new Font("Segoe UI Semibold", 24f);
        using var tagFont = new Font("Segoe UI", 9.5f);
        float tx = tile.Right + 16;
        var nameSize = g.MeasureString(_brand, nameFont);
        g.DrawString(_brand, nameFont, Brushes.White, tx, Height / 2f - nameSize.Height / 2f - 8);
        using var tagBrush = new SolidBrush(Color.FromArgb(205, 222, 245));
        g.DrawString(_tagline, tagFont, tagBrush, tx + 2, Height / 2f + nameSize.Height / 2f - 12);
    }

    static PointF P(float x, float y) => new(x, y);
}
