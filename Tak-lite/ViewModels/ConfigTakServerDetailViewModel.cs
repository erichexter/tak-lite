using System.IO.Compression;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tak_lite.Service;

namespace Tak_lite.ViewModels;

public partial class ConfigTakServerDetailViewModel : ObservableObject
{
    private readonly DataService _dataService;

    [ObservableProperty] public string id=Guid.NewGuid().ToString();
    [ObservableProperty] public bool enabled;

    [ObservableProperty] public string name;

    [ObservableProperty] public string password;

    [ObservableProperty] public string port;

    [ObservableProperty] public string protocol;

    [ObservableProperty] public string server;

    [ObservableProperty] public string username;

    [ObservableProperty] public string zipfilePath;

    public ConfigTakServerDetailViewModel(DataService dataService)
    {
        _dataService = dataService;
    }

    [RelayCommand]
    public async void OpenFile()
    {
        Device.BeginInvokeOnMainThread(async () =>
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/zip" } },
                    { DevicePlatform.WinUI, new[] { "*.zip" } },
                    { DevicePlatform.iOS, new[] { "public.archive" } }
                })
            });
            if (file != null)
            {
                var connectionfile = file.FullPath;
                var filename = Path.GetFileName(connectionfile);
                var fullFileName = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
                ZipfilePath = filename;
                if (!File.Exists(fullFileName))
                    File.Copy(connectionfile, fullFileName);

                var prefs = GetZipfilePreferences(fullFileName);
                var hosts = GetHost(prefs);
                Server = hosts.host;
                Port = hosts.port.ToString();
                Name = hosts.description;
                Protocol = "SSL";
                Enabled = true;
            }
        });
    }

    public static (string host, int port, string description) GetHost(Preferences manifest)
    {
        var CoTStreamsKey = "cot_streams";
        var ConnectionStringKey = "connectString0";

        var connection = manifest.Preference.First(p => p.Name == CoTStreamsKey)
            .Entry
            .First(e => e.Key == ConnectionStringKey);

        var connectionParams = connection.Text.Split(':');
        var host = connectionParams.First()!;
        var port = int.Parse(connectionParams[1]);

        var description = manifest.Preference.First(p => p.Name == CoTStreamsKey)
            .Entry
            .First(e => e.Key == "description0").Text;

        return (host, port, description);
    }

    public static Preferences GetZipfilePreferences(string packagePath)
    {
        using var package = new ZipArchive(new FileStream(packagePath!, FileMode.Open));
        var prefEntry = package.Entries.First(e => e.Name.EndsWith(".pref"));
        using var prefStream = prefEntry.Open();
        var xmlStream = new StreamReader(prefStream);
        XmlSerializer serializer = new(typeof(Preferences));

        return (Preferences)serializer.Deserialize(xmlStream)!;
    }


    [RelayCommand]
    public async Task SaveServer()
    {
        //btnSave.IsEnabled = fa;

        var appSettings = _dataService.GetAppSettings();
        if (_dataService.GetAppSettings().Servers.Exists(a => a.Id == id))
        {
            appSettings.Servers.Remove(appSettings.Servers.Single(a => a.Id == id));
        }
        appSettings.Servers.Add(new TakServer
        {
            Enabled = enabled,
            Name = name,
            Password = password,
            Port = port,
            Protocol = protocol,
            Server = server,
            Username = username,
            ZipfilePath = zipfilePath,
            Id=id,
        });
        _dataService.Save(appSettings);

        await Shell.Current.GoToAsync("..");
    }

    public void LoadFromId(string id)
    {
        var server=_dataService.GetAppSettings().Servers.SingleOrDefault(a => a.Id == id);
        Enabled = server.Enabled;
        Name = server.Name;
        Password = server.Password;
        Port = server.Port;
        Protocol = server.Protocol;
        Username = server.Username;
        ZipfilePath = server.ZipfilePath;
        Id = server.Id;
        Server = server.Server;
    }
}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
[XmlRoot(ElementName = "entry")]
public class Entry
{
    [XmlAttribute(AttributeName = "key")] public string Key { get; set; }

    [XmlAttribute(AttributeName = "class")]
    public string Class { get; set; }

    [XmlText] public string Text { get; set; }
}

[XmlRoot(ElementName = "preference")]
public class Preference
{
    [XmlElement(ElementName = "entry")] public List<Entry> Entry { get; set; }

    [XmlAttribute(AttributeName = "version")]
    public int Version { get; set; }

    [XmlAttribute(AttributeName = "name")] public string Name { get; set; }

    [XmlText] public string Text { get; set; }
}

[XmlRoot(ElementName = "preferences")]
public class Preferences
{
    [XmlElement(ElementName = "preference")]
    public List<Preference> Preference { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.