using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.Maui.Maps;

namespace Tak_lite;

public partial class  MainViewModel : ObservableObject
{
    private readonly LocationService _locationService;

    public MainViewModel(LocationService locationService)
    {
        _locationService = locationService;
    }
    //[ObservableProperty]
    //bool isConnected ;
    
    [ObservableProperty]
    string mapTilesUrl;
    
    //[ObservableProperty]
     Location location;

     [ObservableProperty]  
     ObservableCollection<MapMarker> markers=new ();

     [ObservableProperty] private MapZoomPanBehavior mapZoomPanBehavior;
     [ObservableProperty] private MapLatLng center;
     public MapTileLayer MapTileLayer { get; set; }


     async void UpdateLocation()
     {
          location = await _locationService.GetCurrentLocation();
         SetLocation(location);
     }

     [RelayCommand]
     void CenterMap()
     {
         MapTileLayer.Center=new MapLatLng(location.Latitude, location.Longitude);
         MapZoomPanBehavior = new MapZoomPanBehavior() { ZoomLevel = 20,MaxZoomLevel = 30};
     }

     public void Load()
     {
         MapTilesUrl = "http://mt1.google.com/vt/lyrs=y&amp;x={x}&amp;y={y}&amp;z={z}"    ;
         Center = new MapLatLng(0, 0);
         MapZoomPanBehavior = new MapZoomPanBehavior() { ZoomLevel = 20,MaxZoomLevel = 30};
         UpdateLocation();
     }

     public void SetLocation(Location loc)
     {
         location = loc;
         //update marker location.
         markers.Clear();
         markers.Add(new MapMarker()
         {
             Latitude = location.Latitude,Longitude = location.Longitude,IconType = MapIconType.Triangle

         });
         Center=new MapLatLng(location.Latitude, location.Longitude);
     }

}