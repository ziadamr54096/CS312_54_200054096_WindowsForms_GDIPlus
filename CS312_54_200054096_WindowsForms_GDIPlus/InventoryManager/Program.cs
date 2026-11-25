using InventoryManager.Auth;

namespace InventoryManager;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Requirement: exception handling - nothing should ever crash the app silently.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => Ui.Error(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) => Ui.Error(e.ExceptionObject as Exception);

        try
        {
            Db.Init();
            AuthSchema.Ensure();   // adds the Users table; existing tables and data are untouched
        }
        catch (Exception ex)
        {
            Ui.Error(ex);
            return;
        }

        StartupFlow.Run();   // LoginForm -> MainForm (see Auth/StartupFlow.cs)
    }
}
