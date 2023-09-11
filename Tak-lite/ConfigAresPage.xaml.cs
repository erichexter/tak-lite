using Tak_lite.ViewModels;

namespace Tak_lite;

public partial class ConfigAresPage : ContentPage
{
	public ConfigAresPage(ConfigAresDetailViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        vm.Load();
    }
}