using Eco.FrameworkImpl.Ocl;
using Tak_lite.Service;
using Tak_lite.ViewModels;

namespace Tak_lite;

public partial class ConfigKmlListPage : ContentPage
{
    private readonly ConfigKmlListViewModel _vm;

    public ConfigKmlListPage(ConfigKmlListViewModel viewmodel)
    {
        _vm= viewmodel;
        InitializeComponent();
        BindingContext=viewmodel;
        viewmodel.Load();
    }

    private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        await _vm.Edit(((KmlFile)e.Item).Id);
    }

    private async void SwitchCell_OnOnChanged(object sender, ToggledEventArgs e)
    {
        var source = ((SwitchCell)sender).Text;
        await _vm.Toggled(source,e.Value);
    }
}