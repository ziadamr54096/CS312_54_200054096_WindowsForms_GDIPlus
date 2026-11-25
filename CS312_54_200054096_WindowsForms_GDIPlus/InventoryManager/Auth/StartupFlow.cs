namespace InventoryManager.Auth;

/// <summary>
/// Application start-up sequence:  [first-run admin setup]  ->  LoginForm  ->  existing MainForm  ->  (Log out)  ->  LoginForm ...
/// Closing the LoginForm (or the MainForm without logging out) exits the application.
/// </summary>
public static class StartupFlow
{
    public static void Run()
    {
        // No active administrator yet (first run, or recovery): let the person at the keyboard create one. Nothing is hardcoded.
        if (!AuthService.HasActiveAdmin())
        {
            using var setup = new FirstAdminSetupForm();
            if (setup.ShowDialog() != DialogResult.OK) return;
        }

        while (true)
        {
            AuthUser user;
            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK || login.SignedInUser == null) return;   // cancelled / closed -> exit
                user = login.SignedInUser;
            }

            if (user.MustChangePassword)   // temporary password assigned by an administrator
            {
                using var change = new ChangePasswordForm(user, forced: true);
                if (change.ShowDialog() != DialogResult.OK || change.UpdatedUser == null) continue;   // back to the LoginForm
                user = change.UpdatedUser;
            }

            Session.Begin(user);
            using (var main = new MainForm())            // the existing main window, unchanged
            {
                MainFormAuthIntegration.Attach(main, user);
                Application.Run(main);
            }

            bool loggedOut = Session.LogoutRequested;
            Session.End();
            Session.LogoutRequested = false;
            if (!loggedOut) return;                       // window closed normally -> exit the application
        }
    }
}
