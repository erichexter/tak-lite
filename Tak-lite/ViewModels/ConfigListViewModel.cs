using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Tak_lite.ViewModels;

public partial class ConfigListViewModel : ObservableObject
{
    private readonly IMessenger _messenger;

    public ConfigListViewModel(IMessenger messenger)
    {
        _messenger = messenger;
    }


    [RelayCommand]
    public async Task Preferences()
    {
        await Shell.Current.GoToAsync(nameof(ConfigPreferencesPage));
    }

    [RelayCommand]
    public async Task Ares()
    {
        await Shell.Current.GoToAsync(nameof(ConfigAresPage));
    }
    [RelayCommand]
    public async Task Servers()
    {
        await Shell.Current.GoToAsync(nameof(ConfigTakServerListPage));
    }

    [RelayCommand]
    public async Task Kml()
    {
        await Shell.Current.GoToAsync(nameof(ConfigKmlListPage));
    }
        
}

public class KmlAddedMessage : ValueChangedMessage<string>
{
    public KmlAddedMessage(string value) : base(value)
    {
    }
}
