using System.Collections.ObjectModel;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using dpp.cot;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;

using Syncfusion.Maui.Maps;
using Tak_lite.Service;
using Tak_lite.ViewModels;
using Color = System.Drawing.Color;
using FontWeight = Esri.ArcGISRuntime.Symbology.FontWeight;
using HorizontalAlignment = Esri.ArcGISRuntime.Symbology.HorizontalAlignment;
using Point = dpp.cot.Point;
using Timer = System.Timers.Timer;
using VerticalAlignment = Esri.ArcGISRuntime.Symbology.VerticalAlignment;

namespace Tak_lite;

public partial class MainArcgisViewModel : ObservableObject, IRecipient<PreferencesUpdatedMessage>,IRecipient<KmlAddedMessage>,IRecipient<KmlVisibleMessage>,IRecipient<KmlHiddenMessage>
{
    private readonly DataService _dataService;
    private readonly LocationService _locationService;
    private readonly IMessenger _messenger;
    private readonly AresAlphaService _alphaService;
    private readonly TakService _takService;
    private Timer _timer;

    [ObservableProperty] private string callsign;
    [ObservableProperty] private MapLatLng center;

    private Location location;

    [ObservableProperty] private string mapTilesUrl;

    [ObservableProperty] private MapZoomPanBehavior mapZoomPanBehavior;

    [ObservableProperty] private ObservableCollection<AtakMapMarker> markers = new();

    public MainArcgisViewModel(LocationService locationService, TakService takService,
        DataService dataService, IMessenger messenger,AresAlphaService alphaService)
    {
        _locationService = locationService;
        _takService = takService;
        _dataService = dataService;
        _messenger = messenger;
        _alphaService = alphaService;
        _takService.Callback = OnTakContact;
        _takService.DisconnectCallback = OnTakContactDisconnect;
        _messenger.Register<PreferencesUpdatedMessage>(this);
        _messenger.Register<KmlAddedMessage>(this);
        _messenger.Register<KmlHiddenMessage>(this);
        _messenger.Register<KmlVisibleMessage>(this);
    }

    private void OnTakContactDisconnect(string uid)
    {
        var marker = (AtakMarkerGraphic)cotMarkersOverlay.Graphics.Cast<IAtakMarker>().FirstOrDefault(a => a.UUID == uid);
        if (marker != null)
        {
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
            var marker1 = CreatePointMarker(new Location( obj.Point.Lat,obj.Point.Lon), obj.Team,obj.Callsign);
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

    [RelayCommand]
    private async void Overlay()
    {
        await Shell.Current.GoToAsync(nameof(ConfigKmlListPage));
    }


    private async void _timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _timer.Interval = 30000;
        location = await _locationService.GetCurrentLocation();
        if (location != null)
        {
            SetLocation(location);
            if (_takService.IsConnected()) _takService.UpdateLocation(location);
            if (_alphaService.LoggedIn) await _alphaService.SendLocation(location.Latitude, location.Longitude);
        }
        var info = await _alphaService.UpdateAllInfo();
        if (info != null)
        {
            foreach (var track in info?.value?.tracks)
            {
                var data = track.ToString().Split(",");
                var id = data[0];
                var lat = double.Parse(data[2]);
                var lng = double.Parse(data[3]);
                var callsign = data[5];
                //"8b281cc5-9f36-4067-bd87-848f1186dbf4,0,30.47136,-97.85051,1,Hotel to - Hex,e3f91780-6dae-4274-bf8a-9a3eee24802e,new squad,,,,,1dc29538-d17b-4dc0-95c1-5d74786b9d47,Red team,,C,"
                OnTakContact(new TakContact()
                    {
                        UUID = id, Callsign = callsign, LastChanged = DateTime.UtcNow,
                        Stale = DateTime.UtcNow.AddMinutes(15), Role = "Team Member", Point = new
                            Point() { Lat = lat, Lon = lng },
                        Team = "Cyan"
                    }
                );
            }
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

        GraphicsOverlayCollection overlays = new GraphicsOverlayCollection
        {
            selfLocationOverlay,cotMarkersOverlay
        };

        MapView.GraphicsOverlays = overlays;

        settings.Kml.ForEach(k=>MapView.Map.OperationalLayers.Add(new KmlLayer(new Uri(k.Filename))
        {
            IsVisible = k.Enabled

        }));

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

        if (settings.AresUser.token != null)
        {
            _alphaService.SetToken(settings.AresUser.token);
            if (settings.AresGame != null)
            {
                _alphaService.SetGameInfo(settings.AresGame.gameId,settings.AresUser.id);
            }
        }

    }

    [ObservableProperty] private string currentLocation;
    private GraphicsOverlay selfLocationOverlay;
    private GraphicsOverlay cotMarkersOverlay;

    public void SetLocation(Location loc)
    {
        location = loc;
        Device.BeginInvokeOnMainThread( () =>
        {

            var pointGraphic = CreatePointMarker(loc,"dodgerblue",Callsign);
            selfLocationOverlay.Graphics.Clear();
            selfLocationOverlay.Graphics.Add(pointGraphic);
            CurrentLocation = "Location: " + location.Latitude.ToString("00.00000") + " " + location.Longitude.ToString("00.00000");
        });
    }

    private static AtakMarkerGraphic CreatePointMarker(Location loc,string color,string label)
    {
        var selfPoint = new MapPoint(loc.Longitude, loc.Latitude, SpatialReferences.Wgs84);
         //new TextSymbol()

         var labelsymbol = new TextSymbol(label,Color.White, 14,HorizontalAlignment.Center,VerticalAlignment.Bottom);
         //labelsymbol.OutlineColor=Color.Black;
         //labelsymbol.OutlineWidth = .5;
         labelsymbol.FontWeight = FontWeight.Normal;
         labelsymbol.OffsetY = 15;
         labelsymbol.BackgroundColor=Color.Gray;
        
         //labelsymbol.LeaderOffsetY = -200;
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
        var comp = new CompositeSymbol(new MarkerSymbol[] { labelsymbol, selfSymbol });

        var pointGraphic = new AtakMarkerGraphic(selfPoint, comp);
        return pointGraphic;
    }

    public void Receive(KmlAddedMessage message)
    {
        var kml = new KmlLayer(new Uri(message.Value));

        MapView.Map.OperationalLayers.Add(kml);

    }

    public void Receive(KmlVisibleMessage message)
    {
        foreach (var layer in MapView.Map.OperationalLayers.Where(a=>a.Name != null && a is KmlLayer && a.Name.Equals(message.Value)))
        {
            layer.IsVisible = true;
            //layer.FullExtent?.GetCenter()
            
        }
    }

    public void Receive(KmlHiddenMessage message)
    {
        foreach (var layer in MapView.Map.OperationalLayers.Where(a=>a.Name != null && a is KmlLayer && a.Name.Equals(message.Value)))
        {
            layer.IsVisible = false;
        }

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
