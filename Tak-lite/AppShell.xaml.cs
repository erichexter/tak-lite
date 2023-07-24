namespace Tak_lite;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(ConfigurationPage),typeof(ConfigurationPage));
	}
}
