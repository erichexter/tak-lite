using Tak_lite;

namespace Tak_lite;

public partial class ConfigurationPage : ContentPage
{
	public ConfigurationPage(ConfigurationViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}