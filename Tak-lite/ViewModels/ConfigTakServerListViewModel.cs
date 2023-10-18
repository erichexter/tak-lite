using System.Collections.ObjectModel;
using System.Net;
using System.Net.Mime;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tak_lite.Service;
using ZXing;

namespace Tak_lite.ViewModels;

public partial class ConfigTakServerListViewModel : ObservableObject
{
    private readonly DataService _dataService;

    [ObservableProperty] public Result[] barcode;

    [ObservableProperty] private bool showScanner;

    public ConfigTakServerListViewModel(DataService dataService)
    {
        _dataService = dataService;
        Load();
    }

    public ObservableCollection<TakServer> Servers { get; set; } = new();

    public void Load()
    {
        var appsettings = _dataService.GetAppSettings();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            Servers.Clear();
            foreach (var i in appsettings.Servers) Servers.Add(i);
        });
    }

    [RelayCommand]
    public async Task Scan()
    {
        ShowScanner = true;
    }

    partial void OnBarcodeChanged(Result[] value)
    {
        MainThread.BeginInvokeOnMainThread(async () => { ShowScanner = false; });
        var qrcode = value[0]?.Text;
        if (!string.IsNullOrEmpty(qrcode))
        {
            var pairs = qrcode.Split("|");
            foreach (var pair in pairs)
                if (pair.StartsWith("TAK/"))
                {
                    var url = new Uri(pair.Replace("TAK/", ""));
                    var filename = GetFilenameFromWebServer(url.ToString());
                    var fullFileName = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(url, fullFileName);
                    }

                    var prefs = ConfigTakServerDetailViewModel.GetZipfilePreferences(fullFileName);
                    var hosts = ConfigTakServerDetailViewModel.GetHost(prefs);
                    var appSettings = _dataService.GetAppSettings();
                    appSettings.Servers.Add(new TakServer
                    {
                        Enabled = true,
                        Name = hosts.description,
                        Port = hosts.port.ToString(),
                        Protocol = "SSL",
                        Server = hosts.host,
                        ZipfilePath = filename,
                        Id = Guid.NewGuid().ToString()
                    });
                    _dataService.Save(appSettings);
                    Load();
                }
        }
    }

    public string GetFilenameFromWebServer(string url)
    {
        var result = "";

        var req = WebRequest.Create(url);
        req.Method = "HEAD";
        using (var resp = req.GetResponse())
        {
            // Try to extract the filename from the Content-Disposition header
            if (!string.IsNullOrEmpty(resp.Headers["Content-Disposition"]))
            {
                var disp = new ContentDisposition(resp.Headers["Content-Disposition"]);
                result = disp.FileName;
            }
        }

        return result;
    }

    [RelayCommand]
    public async Task Create()
    {
        await Shell.Current.GoToAsync(nameof(ConfigTakServerDetailPage));
    }

    [RelayCommand]
    public async Task Edit(object id)
    {
        await Shell.Current.GoToAsync(nameof(ConfigTakServerDetailPage),
            new Dictionary<string, object> { { "id", id } });
    }
}

public class TakServer
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public bool Enabled { get; set; } = true;
    public string Server { get; set; }
    public string Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ZipfilePath { get; set; }
    public string Protocol { get; set; } = "SSL";
}