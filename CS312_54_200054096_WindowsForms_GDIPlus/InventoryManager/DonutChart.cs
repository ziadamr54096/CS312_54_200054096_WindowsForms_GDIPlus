using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace InventoryManager;

/// <summary>Donut chart with legend (GDI+). Shows sample slices in the Designer; at runtime it shows data passed to SetData().</summary>
[ToolboxItem(true)]
[DefaultProperty("Title")]
public class DonutChart : Control
{
    static readonly (string Label, double Value, Color Color)[] DesignSample =
    {
        ("In stock", 7, Color.SeaGreen), ("Low stock", 3, Color.Orange), ("Out of stock", 2, Color.Firebrick)
    };

    List<(string Label, double Value, Color Color)> _data = new();
    string _title = "";

    public DonutChart()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.White;
    }

    [Category("Appearance"), DefaultValue("")]
    public string Title { get => _title; set { _title = value ?? ""; Invalidate(); } }

    protected override Size DefaultSize => new Size(300, 260);

    /// <summary>Runtime API (unchanged): replace the plotted data.</summary>
    public void SetData(IEnumerable<(string Label, double Value, Color Color)> data)
    {
        _data = data.ToList();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        g.Clear(BackColor);

        IReadOnlyList<(string Label, double Value, Color Color)> data =
            _data.Count == 0 && DesignMode ? DesignSample : _data;

        using var titleFont = new Font("Segoe UI Semibold", 10.5f);
        g.DrawString(Title, titleFont, Brushes.Black, 8, 6);

        double total = data.Sum(d => d.Value);
        int legendH = data.Count * 20;
        int size = Math.Min(Width - 20, Height - 40 - legendH);
        if (size < 40) return;

        if (total <= 0)
        {
            g.DrawString("No data", Font, Brushes.Gray, 20, 40);
            return;
        }

        var ring = new Rectangle((Width - size) / 2, 34, size, size);
        float start = -90f;
        foreach (var d in data)
        {
            if (d.Value <= 0) continue;
            float sweep = (float)(d.Value / total * 360.0);
            using var br = new SolidBrush(d.Color);
            g.FillPie(br, ring, start, sweep);
            start += sweep;
        }

        int hole = (int)(size * 0.56);
        var inner = new Rectangle(ring.X + (size - hole) / 2, ring.Y + (size - hole) / 2, hole, hole);
        using (var bg = new SolidBrush(BackColor))
            g.FillEllipse(bg, inner);

        using var bigFont = new Font("Segoe UI Semibold", 16f);
        using var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(total.ToString("N0"), bigFont, Brushes.Black, inner, center);

        int y = ring.Bottom + 8;
        int x = Math.Max(10, (Width - 190) / 2);
        foreach (var d in data)
        {
            using var br = new SolidBrush(d.Color);
            g.FillRectangle(br, x, y + 3, 12, 12);
            g.DrawString($"{d.Label}: {d.Value:N0}", Font, Brushes.Black, x + 18, y);
            y += 20;
        }
    }
}
