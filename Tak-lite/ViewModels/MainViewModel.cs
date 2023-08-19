using System.Collections.ObjectModel;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Syncfusion.Maui.Maps;
using Tak_lite.Service;
using Timer = System.Timers.Timer;

namespace Tak_lite;

public partial class MainViewModel : ObservableObject, IRecipient<PreferencesUpdatedMessage>
{
    private readonly DataService _dataService;
    private readonly LocationService _locationService;
    private readonly IMessenger _messenger;
    private readonly TakService _takService;
    private Timer _timer;

    [ObservableProperty] private string callsign;
    [ObservableProperty] private MapLatLng center;

    private ObservableCollection<MapSublayer> Layers = new();


    //[ObservableProperty]
    private Location location;
    //[ObservableProperty]
    //bool isConnected ;

    [ObservableProperty] private string mapTilesUrl;

    [ObservableProperty] private MapZoomPanBehavior mapZoomPanBehavior;

    [ObservableProperty] private ObservableCollection<AtakMapMarker> markers = new();

    public MainViewModel(LocationService locationService, TakService takService,
        DataService dataService, IMessenger messenger)
    {
        _locationService = locationService;
        _takService = takService;
        
        _dataService = dataService;
        _messenger = messenger;
        _takService.Callback = OnTakContact;
        _takService.DisconnectCallback = OnTakContactDisconnect;
        //akServiceInstance.Callback = OnTakContact;
        _messenger.Register(this);
    }

    private void OnTakContactDisconnect(string uid)
    {
        var marker = markers.FirstOrDefault(a => a.UUID == uid);
        if (marker != null)
        {
            marker.IconStroke = new SolidColorBrush(Color.Parse("gray"));
            marker.IconFill = new SolidColorBrush(Color.Parse("gray"));

        }

    }

    public MapTileLayer MapTileLayer { get; set; }

    public void Receive(PreferencesUpdatedMessage message)
    {
        var settings = _dataService.GetAppSettings();
        Callsign = settings.Callsign;
    }

    private void OnTakContact(TakContact obj)
    {
        
        var marker = markers.FirstOrDefault(a => a.UUID == obj.UUID);
        if (marker != null)
        {
            if (obj.Callsign == null) //remove the marker.
            {
            }
            else
            {
                marker.Latitude = obj.Point.Lat;
                marker.Longitude = obj.Point.Lon;
                marker.TakContact= obj;
            }
        }
        else
        {
            markers.Add(new AtakMapMarker
            {
                UUID = obj.UUID,
                CallSign = obj.Callsign,
                Latitude = obj.Point.Lat,
                Longitude = obj.Point.Lon,
                Color = obj.Team,
                Role = obj.Role,
                IconHeight = 20,
                IconWidth = 20,
                IconStroke = new SolidColorBrush(Color.Parse(obj.Team)),
                IconFill = new SolidColorBrush(Color.Parse(obj.Team)),
                SourceUid=obj.SourecUid,
                TakContact=obj,
            });
        }
    }

    private async void UpdateLocation()
    {
        location = await _locationService.GetCurrentLocation();
        if (location != null)
            SetLocation(location);
    }

    [RelayCommand]
    private async Task UserConfig()
    {
        await Shell.Current.GoToAsync(nameof(MainArcgisPage));
    }

    [RelayCommand]
    private void CenterMap()
    {
        MapTileLayer.Center = new MapLatLng(location.Latitude, location.Longitude);
        MapZoomPanBehavior = new MapZoomPanBehavior { ZoomLevel = 20, MaxZoomLevel = 25 };
    }


    private async void _timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _timer.Interval = 30000;
        location = await _locationService.GetCurrentLocation();
        if (location != null)
        {
            SetLocation(location);
            if (_takService.IsConnected()) _takService.UpdateLocation(location);
        }

        RemoveExpiredMarkers();
    }

    private void RemoveExpiredMarkers()
    {
        var remove = markers.Where(a => a.TakContact !=null && a.TakContact.Stale < DateTime.UtcNow).ToList();
        foreach (var marker in remove)
        {
            markers.Remove(marker);
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
        _timer = new Timer();
        _timer.Interval = 1000;
        _timer.AutoReset = true;
        _timer.Elapsed += _timer_Elapsed;
        _timer.Start();
        _takService.LoadServer();    
        //_takService.Connect();
        _takService.UpdateContact(new TakContact()
        {
            Callsign = callsign,Role = settings.Role,Team = settings.Team,UUID = Guid.NewGuid().ToString()
        });
        
    }

    [ObservableProperty] private string currentLocation;
    public void SetLocation(Location loc)
    {
        location = loc;
        Device.BeginInvokeOnMainThread(async () =>
        {
            var marker = Markers.SingleOrDefault(a => a.UUID == "self");
            if (marker == null)
            {
                Markers.Add(new AtakMapMarker 
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
            CurrentLocation = $"Location: {location.Latitude} {location.Longitude} ";
        });
    }
}

public class AtakMapMarker : MapMarker,IAtakMarker
{
    public string CallSign { get; set; }
    public string Role { get; set; }
    public string Color { get; set; }
    public string UUID { get; set; }
    public string SourceUid { get; set; }
    public TakContact TakContact { get; set; }
}

public interface IAtakMarker
{
    public string CallSign { get; set; }
    public string Role { get; set; }
    public string Color { get; set; }
    public string UUID { get; set; }
    public string SourceUid { get; set; }
    public TakContact TakContact { get; set; }
    double Latitude { get; set; }
    double Longitude { get; set; }
}