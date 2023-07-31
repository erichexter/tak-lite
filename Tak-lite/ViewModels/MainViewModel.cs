using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Syncfusion.Maui.Maps;
using Tak_lite.Service;
using Timer = System.Timers.Timer;

namespace Tak_lite;

public partial class MainViewModel : ObservableObject,IRecipient<PreferencesUpdatedMessage>
{
    Timer _timer;
    private readonly LocationService _locationService;
    private readonly TakServiceInstance _takServiceInstance;
    private readonly DataService _dataService;
    private readonly IMessenger _messenger;
    [ObservableProperty] private MapLatLng center;

    private ObservableCollection<MapSublayer> Layers = new();

    [ObservableProperty] private string callsign;
    

    //[ObservableProperty]
    private Location location;
    //[ObservableProperty]
    //bool isConnected ;

    [ObservableProperty] private string mapTilesUrl;

    [ObservableProperty] private MapZoomPanBehavior mapZoomPanBehavior;

    [ObservableProperty] private ObservableCollection<ATAKMapMarker> markers = new();

    public MainViewModel(LocationService locationService, TakServiceInstance takServiceInstance,DataService dataService,IMessenger messenger)
    {
        _locationService = locationService;
        _takServiceInstance = takServiceInstance;
        _dataService = dataService;
        _messenger = messenger;
        _takServiceInstance.Callback = OnTakContact;
        _messenger.Register<PreferencesUpdatedMessage>(this);
    }

    public MapTileLayer MapTileLayer { get; set; }

    private void OnTakContact(TakContact obj)
    {
        var marker = markers.FirstOrDefault(a => a.UUID == obj.UUID);
        if (marker != null)
        {
            if (obj.Callsign == null)//remove the marker.
            {

            }
            else
            {
                marker.Latitude = obj.Point.Lat;
                marker.Longitude = obj.Point.Lon;
            }
        }
        else
        {
            markers.Add(new ATAKMapMarker
            {
                UUID=obj.UUID,
                CallSign = obj.Callsign, 
                Latitude = obj.Point.Lat, 
                Longitude = obj.Point.Lon, 
                Color = obj.Team,
                Role = obj.Role,
                IconHeight = 20,
                IconWidth = 20,
                IconStroke = new SolidColorBrush(Color.Parse(obj.Team)),
                IconFill = new SolidColorBrush(Color.Parse(obj.Team))
            });
        }
    }

    private async void UpdateLocation()
    {
        location = await _locationService.GetCurrentLocation();
        if(location != null)
            SetLocation(location);
    }
    [RelayCommand]
    private async Task UserConfig()
    {
        await Shell.Current.GoToAsync(nameof(ConfigListingPage));
    }

    [RelayCommand]
    private void CenterMap()
    {
        MapTileLayer.Center = new MapLatLng(location.Latitude, location.Longitude);
        MapZoomPanBehavior = new MapZoomPanBehavior { ZoomLevel = 20, MaxZoomLevel = 25 };
    }

    [RelayCommand]
    private async void AtakServerFile()
    {
        var file=await FilePicker.Default.PickAsync(new PickOptions()
        {
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                {DevicePlatform.Android,new[]{"application/zip"}},
                {DevicePlatform.WinUI, new []{"*.zip"}},
                {DevicePlatform.iOS, new []{"public.archive"}},
            })
        });
        if (file != null)
        {
            var connectionfile = file.FullPath;

            _takServiceInstance.Connect(connectionfile);
        }
    }

    private async void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        //_timer.Interval = 30000;
        location = await _locationService.GetCurrentLocation();
        if (location != null)
        {
            SetLocation(location);
            if (_takServiceInstance.IsConnected)
            {
                _takServiceInstance.UpdateLocation(location);
            }
            
        }
    }

    public void Load()
    {
        var settings = _dataService.GetAppSettings();
        Callsign = settings.Callsign;
        MapTilesUrl = "http://mt1.google.com/vt/lyrs=y&x={x}&y={y}&z={z}";
        Center = new MapLatLng(0, 0);
        MapZoomPanBehavior = new MapZoomPanBehavior { ZoomLevel = 20, MaxZoomLevel = 25 };
        UpdateLocation();
        _timer = new System.Timers.Timer();
        _timer.Interval = 1000;
        _timer.AutoReset = true;
        _timer.Elapsed += _timer_Elapsed;
        _timer.Start();

    }

    public void SetLocation(Location loc)
    {
        location = loc;
        Device.BeginInvokeOnMainThread(async () =>
        {
            var marker = Markers.SingleOrDefault(a => a.UUID == "self");
            if (marker == null)
            {
                Markers.Add(new ATAKMapMarker
                {
                    UUID = "self",
                    CallSign = "self",
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    IconType = MapIconType.Triangle,
                    IconWidth = 20,
                    IconHeight = 20,
                    IconFill = Brush.LightBlue
                });
            }
            else
            {
                marker.Latitude = loc.Latitude;
                marker.Longitude = loc.Longitude;
            }

            Center = new MapLatLng(location.Latitude, location.Longitude);
        });
    }

    public void Receive(PreferencesUpdatedMessage message)
    {
        var settings = _dataService.GetAppSettings();
        Callsign = settings.Callsign;
    }
}

public class ATAKMapMarker : MapMarker
{
    public string CallSign { get; set; }
    public string Role { get; set; }
    public string Color { get; set; }
    public string UUID { get; set; }
}