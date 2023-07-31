using Newtonsoft.Json;
using Tak_lite.ViewModels;

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
        else
        {
            var appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(settingsPath));
            return appSettings;
        }
    }

    public void Save(AppSettings setting)
    {
        var serialized = JsonConvert.SerializeObject(setting);
        File.WriteAllText(settingsPath, serialized);
    }
}

public class AppSettings
{
    public string Callsign;
    public string Role;
    public string Team;
    public List<TakServer> Servers { get; set; }=new List<TakServer>();
}