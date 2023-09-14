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
        var appSettings = _dataService.GetAppSettings();
        _ares = appSettings.AresUser;
        _game = appSettings.AresGame;
        if (_ares != null)
        {
            ShowLogin = string.IsNullOrEmpty(_ares.token);
            Username = _ares.username;
            _aresAlphaService.SetToken(_ares.token);

            if (_game != null)
            {
                ShowGame = true;
                ShowGameJoin = false;
                GameName = _game.gameName;
            }
            else
            {
                ShowGameJoin = !showLogin;    
            }

            

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
    public async Task JoinGame()
    {

        string playerId=_ares.id;
        string sponsorId=SponsorId;
        string gameId=GameId;
        //var result=await _aresAlphaService.Login(Username, Password);
        var game = await _aresAlphaService.JoinGame(playerId,sponsorId,gameId);
        if (game != null)
        {
            var appsettings = _dataService.GetAppSettings();
            appsettings.AresGame = game;
            GameName = game.gameName;
            ShowGame = true;
            ShowGameJoin = false;
            _dataService.Save(appsettings);
        }
    }

    [ObservableProperty] public string gameId;
    [ObservableProperty] public string sponsorId;
    [ObservableProperty] public string level;
    [ObservableProperty] public string groupName;
    [ObservableProperty] public string gameName;
    [ObservableProperty] public bool showGame;
    private Game _game;
}