using Eco.FrameworkImpl.Ocl;
using Tak_lite.ViewModels;

namespace Tak_lite;

public partial class ConfigTakServerListPage : ContentPage
{
    private readonly ConfigTakServerListViewModel _vm;

    public ConfigTakServerListPage(ConfigTakServerListViewModel vm)
	{
		InitializeComponent();
		BindingContext=vm;
        _vm = vm;
    }

    private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        await _vm.Edit(((TakServer)e.Item).Id);
    }
}