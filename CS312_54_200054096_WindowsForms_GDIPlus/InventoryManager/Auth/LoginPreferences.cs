namespace InventoryManager.Auth;

/// <summary>"Remember me": remembers only the USERNAME (never the password or a token) in a small per-Windows-user file.</summary>
public static class LoginPreferences
{
    static string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "InventoryManager", "last-user.txt");

    public static string? LoadUsername()
    {
        try { return File.Exists(FilePath) ? File.ReadAllText(FilePath).Trim() : null; }
        catch (IOException) { return null; }
        catch (UnauthorizedAccessException) { return null; }
    }

    public static void Save(string? username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username)) { if (File.Exists(FilePath)) File.Delete(FilePath); return; }
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, username.Trim());
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}
