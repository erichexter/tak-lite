using Tak_lite.ViewModels;

namespace Tak_lite;

public partial class ConfigListingPage : ContentPage
{
	public ConfigListingPage(ConfigListViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}