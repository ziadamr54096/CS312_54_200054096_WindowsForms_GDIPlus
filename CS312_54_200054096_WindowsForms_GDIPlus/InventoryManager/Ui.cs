using System.ComponentModel;
using Microsoft.Data.Sqlite;

namespace InventoryManager;

/// <summary>
/// Runtime helpers shared by the screens. Visual construction no longer lives here:
/// every screen builds its own controls in its .Designer.cs (InitializeComponent).
/// </summary>
public static class Ui
{
    public const string AppTitle = "Inventory Manager";

    /// <summary>
    /// True while a control is being hosted by the Visual Studio Designer. Works inside constructors too
    /// (where Control.DesignMode is still false). Runtime-only work (database, network, timers) must be skipped when this is true.
    /// </summary>
    public static bool InDesigner(Control c) =>
        LicenseManager.UsageMode == LicenseUsageMode.Designtime || c.Site?.DesignMode == true;

    public static void Error(Exception? ex)
    {
        string msg = ex switch
        {
            null => "An unknown error occurred.",
            SqliteException { SqliteErrorCode: 19 } s when s.Message.Contains("UNIQUE") =>
                "That value already exists. SKUs and supplier names must be unique.",
            SqliteException { SqliteErrorCode: 19 } s when s.Message.Contains("FOREIGN KEY") =>
                "This record is linked to other data, so the operation was blocked.",
            SqliteException { SqliteErrorCode: 19 } s when s.Message.Contains("NOT NULL") =>
                "A required field was left empty.",
            SqliteException { SqliteErrorCode: 19 } s when s.Message.Contains("CHECK") =>
                "A number is out of range (price, quantity and reorder level cannot be negative).",
            _ => ex.Message
        };
        MessageBox.Show(msg, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void Warn(string m) => MessageBox.Show(m, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    public static void Info(string m) => MessageBox.Show(m, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
    public static bool Confirm(string m) =>
        MessageBox.Show(m, AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
}
