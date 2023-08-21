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
    public async Task Servers()
    {
        await Shell.Current.GoToAsync(nameof(ConfigTakServerListPage));
    }

    [RelayCommand]
    public async Task Kml()
    {
        await Shell.Current.GoToAsync(nameof(ConfigKmlListPage));
        //Device.BeginInvokeOnMainThread(async () =>
        //{
        //    var file = await FilePicker.Default.PickAsync(new PickOptions
        //    {
        //        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        //        {
        //            { DevicePlatform.Android, new[] { "application/zip" } },
        //            { DevicePlatform.WinUI, new[] { "*.kml","*.kmz","*.zip" } },
        //            { DevicePlatform.iOS, new[] { "public.archive" } }
        //        })
        //    });
        //    _messenger.Send(new KmlAddedMessage(file.FullPath));
        //});
    }
        
}

public class KmlAddedMessage : ValueChangedMessage<string>
{
    public KmlAddedMessage(string value) : base(value)
    {
    }
}
