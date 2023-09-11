using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tak_lite.Service;

namespace Tak_lite.ViewModels;

public partial class ConfigAresDetailViewModel :ObservableObject
{
    private readonly AresAlphaService _aresAlphaService;
    private readonly DataService _dataService;


    public ConfigAresDetailViewModel(AresAlphaService aresAlphaService,DataService dataService)
    {
        _aresAlphaService = aresAlphaService;
        _dataService = dataService;
    }

    public void Load()
    {
        _ares = _dataService.GetAppSettings().AresUser;
        if (_ares != null)
        {
            ShowLogin = string.IsNullOrEmpty(_ares.token);
            ShowGameJoin = !showLogin;
            Username = _ares.username;
            
        }
        
    }

    public LoginUser _ares { get; set; }

    [ObservableProperty] public bool enabled;

    [ObservableProperty] public string password;

    [ObservableProperty] public string username;

    [ObservableProperty] public bool showLogin;
    [ObservableProperty] public bool showGameJoin;

    [RelayCommand]
    public async Task Save()
    {
        var result=await _aresAlphaService.Login(Username, Password);
        var appsettings = _dataService.GetAppSettings();
        appsettings.AresUser = result.value;
        _dataService.Save(appsettings);
    }

    [RelayCommand]
    public  void Logout()
    {
        //var result=await _aresAlphaService.Login(Username, Password);
        _aresAlphaService.Logout();
        var appsettings = _dataService.GetAppSettings();
        appsettings.AresUser.token = "";
        _dataService.Save(appsettings);
    }

    [RelayCommand]
    public  void JoinGame()
    {
        //var result=await _aresAlphaService.Login(Username, Password);
        _aresAlphaService.JoinGame();
        var appsettings = _dataService.GetAppSettings();
        appsettings.AresUser.token = "";
        _dataService.Save(appsettings);
    }

}