using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tak_lite.Service;

namespace Tak_lite.ViewModels;

public partial class ConfigTakServerListViewModel : ObservableObject
{
    private readonly DataService _dataService;

    public ConfigTakServerListViewModel(DataService dataService)
    {
        _dataService = dataService;
        Load();
    }

    public ObservableCollection<TakServer> Servers { get; set; } = new();

    public void Load()
    {
        var appsettings = _dataService.GetAppSettings();

        Servers.Clear();
        foreach (var i in appsettings.Servers) Servers.Add(i);
    }

    [RelayCommand]
    public async Task Create()
    {
        await Shell.Current.GoToAsync(nameof(ConfigTakServerDetailPage));
    }
}

public partial class TakServer
{
    public string Name { get; set; }
    public bool Enabled { get; set; } = true;
    public string Server { get; set; }
    public string Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ZipfilePath { get; set; }
    public string Protocol { get; set; } = "SSL";
}