using AVFoundation;

namespace Tak_lite.Service;

public class DataService
{
    private readonly string filePath;
     private readonly string settingsPath;

     public DataService()
    {
        filePath = FileSystem.AppDataDirectory;
        settingsPath = Path.Combine(filePath, "settings.json");
    }

    

    public AppSettings GetAppSettings()
    {
        if (!File.Exists(settingsPath))
        {
            var appSettings = new AppSettings();
            Save(appSettings);
            return appSettings;
        }

        return null;
    }

    public void Save(AppSettings setting)
    {
    }
}

public class AppSettings
{
    public string Callsign;
    public string Role;
    public string Team;
}