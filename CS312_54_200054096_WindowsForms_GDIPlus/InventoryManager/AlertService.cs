using System.Data;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;

namespace InventoryManager;

public record SmtpSettings(string Host, int Port, bool Ssl, string User, string Password, string To, bool Auto);

/// <summary>
/// Requirement: networking (TCP + SMTP) and async. Everything here is awaited so the UI never freezes.
/// </summary>
public static class AlertService
{
    public static SmtpSettings Load() => new(
        Db.GetSetting("SmtpHost", "smtp.gmail.com"),
        int.TryParse(Db.GetSetting("SmtpPort", "587"), out var port) ? port : 587,
        Db.GetSetting("SmtpSsl", "1") == "1",
        Db.GetSetting("SmtpUser"),
        Db.GetSetting("SmtpPassword"),
        Db.GetSetting("AlertTo"),
        Db.GetSetting("AutoAlert", "0") == "1");

    public static void Save(SmtpSettings s)
    {
        Db.SetSetting("SmtpHost", s.Host);
        Db.SetSetting("SmtpPort", s.Port.ToString());
        Db.SetSetting("SmtpSsl", s.Ssl ? "1" : "0");
        Db.SetSetting("SmtpUser", s.User);
        Db.SetSetting("SmtpPassword", s.Password);
        Db.SetSetting("AlertTo", s.To);
        Db.SetSetting("AutoAlert", s.Auto ? "1" : "0");
    }

    static async Task SendAsync(SmtpSettings s, string to, string subject, string body)
    {
        using var client = new SmtpClient(s.Host, s.Port)
        {
            EnableSsl = s.Ssl,
            Credentials = new NetworkCredential(s.User, s.Password)
        };
        using var msg = new MailMessage(s.User, to, subject, body);
        using var cts = new CancellationTokenSource(20000);
        await client.SendMailAsync(msg, cts.Token);
    }

    /// <summary>Step 1: raw TCP connect + read the server greeting. Step 2: log in and send a test mail.</summary>
    public static async Task<string> TestAsync(SmtpSettings s)
    {
        using (var tcp = new TcpClient())
        {
            using var cts = new CancellationTokenSource(8000);
            await tcp.ConnectAsync(s.Host, s.Port, cts.Token);
            using var reader = new StreamReader(tcp.GetStream());
            string? banner = await reader.ReadLineAsync(cts.Token);
            if (s.Port != 465 && (banner == null || !banner.StartsWith("220")))
                throw new InvalidOperationException("Unexpected greeting from server: " + banner);
        }
        await SendAsync(s, s.User, "Inventory Manager - test message", "Your SMTP settings work.");
        return "Connection successful. A test e-mail was sent to " + s.User + ".";
    }

    public static async Task<string> NotifyLowStockAsync()
    {
        var s = Load();
        if (string.IsNullOrWhiteSpace(s.Host) || string.IsNullOrWhiteSpace(s.User) || string.IsNullOrWhiteSpace(s.To))
            return "E-mail is not configured. Open the Settings tab first.";

        DataTable dt = await Task.Run(() => Db.Query(
            @"SELECT p.Sku, p.Name, p.Quantity, p.ReorderLevel, COALESCE(s.Name,'-') AS Supplier
              FROM Products p LEFT JOIN Suppliers s ON s.Id = p.SupplierId
              WHERE p.Quantity <= p.ReorderLevel ORDER BY p.Quantity"));

        if (dt.Rows.Count == 0) return "No low-stock items - nothing to send.";

        var sb = new StringBuilder("The following products need restocking:\n\n");
        foreach (DataRow r in dt.Rows)
            sb.AppendLine($"- {r["Sku"]}  {r["Name"]}: {r["Quantity"]} left (reorder level {r["ReorderLevel"]}), supplier: {r["Supplier"]}");

        await SendAsync(s, s.To, $"[Inventory Manager] {dt.Rows.Count} item(s) low on stock", sb.ToString());
        return $"Alert e-mail sent to {s.To} ({dt.Rows.Count} items).";
    }
}
