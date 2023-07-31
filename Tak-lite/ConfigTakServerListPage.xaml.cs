using Tak_lite.ViewModels;

namespace Tak_lite;

public partial class ConfigTakServerListPage : ContentPage
{
	public ConfigTakServerListPage(ConfigTakServerListViewModel vm)
	{
		InitializeComponent();
		BindingContext=vm;
	}
}