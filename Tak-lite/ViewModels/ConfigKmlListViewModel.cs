using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Tak_lite.Service;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Tak_lite.ViewModels;

public partial class ConfigKmlListViewModel : ObservableObject
{
    private readonly IMessenger _messenger;
    private readonly DataService _dataService;

    public ConfigKmlListViewModel(IMessenger messenger,DataService dataService)
    {
        _messenger = messenger;
        _dataService = dataService;
    }
    public ObservableCollection<KmlFile> Files { get; set; } = new();
    public void Load()
    {
        var appsettings = _dataService.GetAppSettings();

        Files.Clear();
        foreach (var i in appsettings.Kml) Files.Add(i);
    }

    [RelayCommand]
    public void Create()
    {
        Device.BeginInvokeOnMainThread(async () =>
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/zip" } },
                    { DevicePlatform.WinUI, new[] { "*.kml","*.kmz","*.zip" } },
                    { DevicePlatform.iOS, new[] { "public.archive" } }
                })
            });
            var kmlfile = new KmlFile()
            {
                Enabled = true,
                Filename = file.FullPath,
                Name = Path.GetFileNameWithoutExtension(file.FullPath)
            };
            var settings = _dataService.GetAppSettings();
            settings.Kml.Add(kmlfile);
            _dataService.Save(settings);
            Files.Add(kmlfile);
            _messenger.Send(new KmlAddedMessage(file.FullPath));
            
        });
    }
    public async Task Edit(string id)
    {
        //throw new NotImplementedException();
    }

    public async Task Toggled(string source, bool visible)
    {
        var settings= _dataService.GetAppSettings();
        settings.Kml = Files.ToList();
        foreach (var kmlFile in settings.Kml.Where(a=>a.Name.Equals(source)))
        {
            kmlFile.Enabled=visible;
        }
        _dataService.Save(settings);
        if (visible)
        {
            _messenger.Send(new KmlVisibleMessage(source));
        }
        else
        {
            _messenger.Send(new KmlHiddenMessage(source));
        }
        //fire message to refresh kml layers to visible.
    }
}

public class KmlVisibleMessage : ValueChangedMessage<string>
{
    public KmlVisibleMessage(string value) : base(value)
    {
    }
}
public class KmlHiddenMessage : ValueChangedMessage<string>
{
    public KmlHiddenMessage(string value) : base(value)
    {
    }
}

