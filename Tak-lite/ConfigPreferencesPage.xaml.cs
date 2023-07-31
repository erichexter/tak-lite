using Tak_lite;

namespace Tak_lite;

public partial class ConfigPreferencesPage : ContentPage
{
	public ConfigPreferencesPage(ConfigPreferencesViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}