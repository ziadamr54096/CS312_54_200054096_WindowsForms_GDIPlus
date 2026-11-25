using System.Text.RegularExpressions;

namespace InventoryManager;

public partial class SettingsPanel : UserControl
{
    public SettingsPanel()
    {
        InitializeComponent();   // visual layout only - see SettingsPanel.Designer.cs
    }

    // ---------- runtime initialisation (skipped inside the Visual Studio Designer) ----------

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Ui.InDesigner(this)) return;

        try
        {
            var s = AlertService.Load();
            txtHost.Text = s.Host;
            numPort.Value = Math.Clamp(s.Port, 1, 65535);
            chkSsl.Checked = s.Ssl;
            txtUser.Text = s.User;
            txtPass.Text = s.Password;
            txtTo.Text = s.To;
            chkAuto.Checked = s.Auto;
        }
        catch (Exception ex) { Ui.Error(ex); }
    }

    // ---------- event handlers (wired in the Designer) ----------

    void btnSave_Click(object? sender, EventArgs e)
    {
        try
        {
            if (!TryRead(out var s)) return;
            AlertService.Save(s);
            lblStatus.ForeColor = Color.SeaGreen;
            lblStatus.Text = "Settings saved.";
        }
        catch (Exception ex) { Ui.Error(ex); }
    }

    async void btnTest_Click(object? sender, EventArgs e)
    {
        if (!TryRead(out var s)) return;
        btnTest.Enabled = false;
        lblStatus.ForeColor = Color.DimGray;
        lblStatus.Text = "Testing connection...";
        try
        {
            lblStatus.Text = await AlertService.TestAsync(s);
            lblStatus.ForeColor = Color.SeaGreen;
        }
        catch (Exception ex)
        {
            lblStatus.ForeColor = Color.Firebrick;
            lblStatus.Text = "Test failed: " + ex.Message;
        }
        finally { btnTest.Enabled = true; }
    }

    // ---------- business logic (unchanged) ----------

    bool TryRead(out SmtpSettings s)
    {
        s = new SmtpSettings("", 587, true, "", "", "", false);
        var email = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (txtHost.Text.Trim().Length == 0) { Ui.Warn("Enter the SMTP server."); txtHost.Focus(); return false; }
        if (!email.IsMatch(txtUser.Text.Trim())) { Ui.Warn("Enter a valid 'From / user' e-mail address."); txtUser.Focus(); return false; }
        if (txtPass.Text.Length == 0) { Ui.Warn("Enter the password (an App Password for Gmail)."); txtPass.Focus(); return false; }
        string to = txtTo.Text.Trim().Length == 0 ? txtUser.Text.Trim() : txtTo.Text.Trim();
        if (!email.IsMatch(to)) { Ui.Warn("The alert recipient is not a valid e-mail address."); txtTo.Focus(); return false; }

        s = new SmtpSettings(txtHost.Text.Trim(), (int)numPort.Value, chkSsl.Checked,
                             txtUser.Text.Trim(), txtPass.Text, to, chkAuto.Checked);
        return true;
    }
}
