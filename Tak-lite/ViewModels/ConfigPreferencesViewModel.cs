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
        Hiearchy.Add("Alpha 1");
        Hiearchy.Add("Alpha 2");
        Hiearchy.Add("Alpha 3");
        Hiearchy.Add("Alpha 4");
        Hiearchy.Add("Alpha 5");
        Hiearchy.Add("Alpha 6");
        Hiearchy.Add("Alpha 7");
        Hiearchy.Add("Charlie 1");
        Hiearchy.Add("Charlie 2");
        Hiearchy.Add("Charlie 3");
        Hiearchy.Add("Charlie 4");
        Hiearchy.Add("Charlie 5");
        Hiearchy.Add("Charlie 6");
        Hiearchy.Add("Hotel 1");
        Hiearchy.Add("Hotel 2");
        Hiearchy.Add("Hotel 3");
        Hiearchy.Add("Hotel 4");
        Hiearchy.Add("Hotel 5");
        Hiearchy.Add("Hotel 6");
        Hiearchy.Add("Alpha");
        Hiearchy.Add("Charlie");
        Hiearchy.Add("Hotel");
        Hiearchy.Add("Command");


    }



    [ObservableProperty] private string role;
    [ObservableProperty] private string callsign;
    
    [ObservableProperty] private string team;

    [ObservableProperty] private string gameTeam;
    [ObservableProperty] private List<string> hiearchy = new();

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
