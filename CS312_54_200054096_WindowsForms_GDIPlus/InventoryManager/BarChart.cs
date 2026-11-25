using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace InventoryManager;

/// <summary>Gradient bar chart (GDI+). In the Designer it draws sample bars so it can be sized and positioned; at runtime it shows data passed to SetData().</summary>
[ToolboxItem(true)]
[DefaultProperty("Title")]
public class BarChart : Control
{
    static readonly (string Label, double Value)[] DesignSample =
        { ("Category A", 1350), ("Category B", 1160), ("Category C", 990) };

    List<(string Label, double Value)> _data = new();
    string _title = "";

    public BarChart()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.White;
    }

    [Category("Appearance"), DefaultValue("")]
    public string Title { get => _title; set { _title = value ?? ""; Invalidate(); } }

    protected override Size DefaultSize => new Size(420, 260);

    /// <summary>Runtime API (unchanged): replace the plotted data.</summary>
    public void SetData(IEnumerable<(string Label, double Value)> data)
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

        IReadOnlyList<(string Label, double Value)> data =
            _data.Count == 0 && DesignMode ? DesignSample : _data;

        using var titleFont = new Font("Segoe UI Semibold", 10.5f);
        g.DrawString(Title, titleFont, Brushes.Black, 8, 6);
        if (data.Count == 0)
        {
            g.DrawString("No data", Font, Brushes.Gray, 20, 40);
            return;
        }

        var area = new Rectangle(56, 36, Width - 72, Height - 36 - 40);
        if (area.Width < 40 || area.Height < 40) return;

        double max = Math.Max(data.Max(d => d.Value), 1);
        using var gridPen = new Pen(Color.Gainsboro);
        for (int i = 0; i <= 4; i++)
        {
            int y = area.Bottom - area.Height * i / 4;
            g.DrawLine(gridPen, area.Left, y, area.Right, y);
            string t = (max * i / 4).ToString("N0");
            var sz = g.MeasureString(t, Font);
            g.DrawString(t, Font, Brushes.Gray, area.Left - sz.Width - 2, y - sz.Height / 2);
        }

        float slot = (float)area.Width / data.Count;
        float bw = Math.Min(slot * 0.6f, 70f);
        using var sf = new StringFormat
        {
            Alignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };

        for (int i = 0; i < data.Count; i++)
        {
            float h = Math.Max((float)(data[i].Value / max * area.Height), 1f);
            float x0 = area.Left + slot * i;
            var bar = new RectangleF(x0 + (slot - bw) / 2, area.Bottom - h, bw, h);
            using (var br = new LinearGradientBrush(bar, Color.FromArgb(100, 170, 245), Color.FromArgb(30, 90, 180), 90f))
                g.FillRectangle(br, bar);

            g.DrawString(data[i].Value.ToString("N0"), Font, Brushes.Black,
                new RectangleF(x0 - 10, bar.Top - 18, slot + 20, 16), sf);
            g.DrawString(data[i].Label, Font, Brushes.DimGray,
                new RectangleF(x0, area.Bottom + 4, slot, 20), sf);
        }
        using var axis = new Pen(Color.Gray, 1.5f);
        g.DrawLine(axis, area.Left, area.Bottom, area.Right, area.Bottom);
    }
}
