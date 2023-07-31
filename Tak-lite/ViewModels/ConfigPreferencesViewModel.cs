using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Tak_lite.Service;

namespace Tak_lite;

public partial class ConfigPreferencesViewModel : ObservableObject
{
    private readonly DataService _dataService;
    private readonly IMessenger _messenger;

    public ConfigPreferencesViewModel(DataService dataService,IMessenger messenger)
    {
        _dataService = dataService;
        _messenger = messenger;
        Load();
    }

    public void Load()
    {
        appSettings = _dataService.GetAppSettings();
        Role = appSettings.Role;
        Callsign = appSettings.Callsign;
        Team= appSettings.Team;
    }



    [ObservableProperty] private string role;
    [ObservableProperty] private string callsign;
    [ObservableProperty] private string team;
    private AppSettings appSettings;


    [RelayCommand]
    public async void Save()
    {
        appSettings.Callsign = Callsign;
        appSettings.Team = Team;
        appSettings.Role = Role;
        _dataService.Save(appSettings);
        _messenger.Send(new PreferencesUpdatedMessage(DateTime.UtcNow));
        //refresh the main screen
        await Shell.Current.GoToAsync("..");
    }

}

public class PreferencesUpdatedMessage : ValueChangedMessage<DateTime>
{
    public PreferencesUpdatedMessage(DateTime value) : base(value)
    {
    }
}
