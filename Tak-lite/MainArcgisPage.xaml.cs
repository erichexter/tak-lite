using Esri.ArcGISRuntime.Mapping;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace Tak_lite;

public partial class MainArcgisPage : ContentPage
{
    public MainArcgisPage( MainArcgisViewModel vm)
    {
        InitializeComponent();
        Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPKd0f0ab5bfee9480ea65a7b218677aacc5RM8Bwx-g1fv_GCptKwuKqbpLWKiH3LS90ApDMMy7oIgIXCheMIpdKXi2ZMc6gE1";

        var myMap = new Map(BasemapStyle.ArcGISImageryStandard);

        // Assign the map to the MapView
        mapview.Map = myMap;
        vm.MapView = mapview;
        vm.Load();
        BindingContext = vm;
        

    }
}