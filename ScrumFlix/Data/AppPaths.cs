// not needed now that MySQL is connected
namespace ScrumFlix.Data;

public static class AppPaths
{
    public static string AppDataFolder
    {
        get
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(basePath, "ScrumFlix");
            Directory.CreateDirectory(folder);
            return folder;
        }
    }

    public static string DatabasePath => Path.Combine(AppDataFolder, "scrumflix.db");
}
