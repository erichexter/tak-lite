using System.Collections.ObjectModel;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Syncfusion.Maui.Maps;
using Tak_lite.Service;
using Tak_lite.ViewModels;
using Timer = System.Timers.Timer;

namespace Tak_lite;

public partial class MainArcgisViewModel : ObservableObject, IRecipient<PreferencesUpdatedMessage>,IRecipient<KmlAddedMessage>
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

    public MainArcgisViewModel(LocationService locationService, TakService takService,
        DataService dataService, IMessenger messenger)
    {
        _locationService = locationService;
        _takService = takService;
        
        _dataService = dataService;
        _messenger = messenger;
        _takService.Callback = OnTakContact;
        _takService.DisconnectCallback = OnTakContactDisconnect;
        //akServiceInstance.Callback = OnTakContact;
        _messenger.Register<PreferencesUpdatedMessage>(this);
        _messenger.Register<KmlAddedMessage>(this);
    }

    private void OnTakContactDisconnect(string uid)
    {
        var marker = (AtakMarkerGraphic)cotMarkersOverlay.Graphics.Cast<IAtakMarker>().FirstOrDefault(a => a.UUID == uid);
        if (marker != null)
        {
            //marker.IconStroke = new SolidColorBrush(Color.Parse("gray"));
            //marker.IconFill = new SolidColorBrush(Color.Parse("gray"));
            ((SimpleMarkerSymbol)marker.Symbol).Color = System.Drawing.Color.Gray;

        }

    }

    public MapView MapView { get; set; }

    public void Receive(PreferencesUpdatedMessage message)
    {
        var settings = _dataService.GetAppSettings();
        Callsign = settings.Callsign;
    }

    private void OnTakContact(TakContact obj)
    {
        
        var marker = (AtakMarkerGraphic)cotMarkersOverlay.Graphics.Cast<IAtakMarker>().FirstOrDefault(a => a.UUID == obj.UUID);
        if (marker != null)
        {
            if (obj.Callsign == null) //remove the marker.
            {
            }
            else
            {
                marker.TakContact = obj;
                marker.SetLocation(obj.Point.Lon,obj.Point.Lat);
            }
        }
        else
        {
            var marker1 = CreatePointMarker(new Location( obj.Point.Lat,obj.Point.Lon), obj.Team);
            marker1.UUID = obj.UUID;
            marker1.CallSign = obj.Callsign;
            marker1.SetLocation(obj.Point.Lon,obj.Point.Lat);
            marker1.Color = obj.Team;
            marker1.Role = obj.Role;
            marker1.SourceUid = obj.SourecUid;
            marker1.TakContact = obj;
            
            cotMarkersOverlay.Graphics.Add(marker1);
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
        await Shell.Current.GoToAsync(nameof(ConfigListingPage));
    }

    [RelayCommand]
    private void CenterMap()
    {
        MapView.SetViewpoint(new Viewpoint(new MapPoint( location.Longitude, location.Latitude,SpatialReferences.Wgs84), 1000));
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

             selfLocationOverlay = new GraphicsOverlay();
             cotMarkersOverlay=new GraphicsOverlay();

            // Add the overlay to a graphics overlay collection.
            GraphicsOverlayCollection overlays = new GraphicsOverlayCollection
            {
                selfLocationOverlay,cotMarkersOverlay
            };

            // Set the view model's "GraphicsOverlays" property (will be consumed by the map view).
            MapView.GraphicsOverlays = overlays;
            //var kml = new KmlLayer("");

            //MapView.Map.OperationalLayers.Add(kml);

        UpdateLocation();
        _timer = new Timer();
        _timer.Interval = 1000;
        _timer.AutoReset = true;
        _timer.Elapsed += _timer_Elapsed;
        _timer.Start();
        _takService.LoadServer();    
        _takService.Connect();
        _takService.UpdateContact(new TakContact()
        {
            Callsign = callsign,Role = settings.Role,Team = settings.Team,UUID = Guid.NewGuid().ToString()
        });
        
    }

    [ObservableProperty] private string currentLocation;
    private GraphicsOverlay selfLocationOverlay;
    private GraphicsOverlay cotMarkersOverlay;

    public void SetLocation(Location loc)
    {
        location = loc;
        Device.BeginInvokeOnMainThread(async () =>
        {

            var pointGraphic = CreatePointMarker(loc,"dodgerblue");
            selfLocationOverlay.Graphics.Clear();
            selfLocationOverlay.Graphics.Add(pointGraphic);
            CurrentLocation = $"Location: {location.Latitude} {location.Longitude} ";
        });
    }

    private static AtakMarkerGraphic CreatePointMarker(Location loc,string color)
    {
        var selfPoint = new MapPoint(loc.Longitude, loc.Latitude, SpatialReferences.Wgs84);
         //new TextSymbol()
        var selfSymbol = new SimpleMarkerSymbol
        {
            Style = SimpleMarkerSymbolStyle.Circle,
            Color = System.Drawing.Color.FromName(color),
            Size = 14,
             
        };

        selfSymbol.Outline = new SimpleLineSymbol
        {
            Style = SimpleLineSymbolStyle.Solid,
            Color = System.Drawing.Color.White,
            Width = 2.0
        };
        var pointGraphic = new AtakMarkerGraphic(selfPoint, selfSymbol);
        return pointGraphic;
    }

    public void Receive(KmlAddedMessage message)
    {
        var kml = new KmlLayer(new Uri(message.Value));

        MapView.Map.OperationalLayers.Add(kml);

        
    }
}


public class AtakMarkerGraphic : Graphic,IAtakMarker
{
    public AtakMarkerGraphic(Geometry geometry, Symbol symbol) : base(geometry, symbol)
    {
    }

    public string CallSign { get; set; }
    public string Role { get; set; }
    public string Color { get; set; }
    public string UUID { get; set; }
    public string SourceUid { get; set; }
    public TakContact TakContact { get; set; }
    public double Latitude { get; set; }

    public void SetLocation(double longitude, double latitude)
    {
        base.Geometry = new MapPoint(longitude, latitude, SpatialReferences.Wgs84);
    }
    public double Longitude { get; set; }
}
