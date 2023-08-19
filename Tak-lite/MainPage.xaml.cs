namespace Tak_lite;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        vm.MapTileLayer = MapTileLayer;
        vm.Load();
        BindingContext = vm;
        
    }
}