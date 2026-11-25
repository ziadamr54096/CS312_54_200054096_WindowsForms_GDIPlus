namespace InventoryManager;

/// <summary>
/// Small pop-up that appears at the bottom-right of the screen for a few seconds ("Message from PC-2", "Low stock alert", ...).
/// It never steals the keyboard focus (so it does not interrupt typing). Layout lives in NotificationToast.Designer.cs.
/// </summary>
public partial class NotificationToast : Form
{
    static NotificationToast? _current;

    public NotificationToast()
    {
        InitializeComponent();    // visual layout only
    }

    protected override bool ShowWithoutActivation => true;

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x08000000 | 0x00000080;     // WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW: no focus steal, no taskbar button
            return cp;
        }
    }

    /// <summary>Shows a toast (replacing the previous one). Kind: message / inventory / lowstock / outofstock / info.</summary>
    public static void Present(Form? owner, string title, string body, string kind)
    {
        try { _current?.Close(); } catch (Exception) { }

        var toast = new NotificationToast();
        toast.lblTitle.Text = title;
        toast.lblBody.Text = body;
        toast.pnlAccent.BackColor = kind switch
        {
            "inventory" => Color.SeaGreen,
            "lowstock" => Color.DarkOrange,
            "outofstock" => Color.Firebrick,
            "info" => Color.Gray,
            _ => Color.SteelBlue
        };

        var screen = owner != null && !owner.IsDisposed ? Screen.FromControl(owner) : Screen.PrimaryScreen ?? Screen.AllScreens[0];
        var area = screen.WorkingArea;
        toast.Location = new Point(area.Right - toast.Width - 16, area.Bottom - toast.Height - 16);

        _current = toast;
        toast.FormClosed += (_, _) => { if (ReferenceEquals(_current, toast)) _current = null; };
        toast.tmrClose.Start();
        toast.Show();
    }

    void Toast_Click(object? sender, EventArgs e) => Close();
    void tmrClose_Tick(object? sender, EventArgs e) => Close();
}
