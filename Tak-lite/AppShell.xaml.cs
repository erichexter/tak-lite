namespace Tak_lite;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(ConfigPreferencesPage), typeof(ConfigPreferencesPage));
        Routing.RegisterRoute(nameof(ConfigListingPage), typeof(ConfigListingPage));
        Routing.RegisterRoute(nameof(ConfigTakServerDetailPage), typeof(ConfigTakServerDetailPage));
        Routing.RegisterRoute(nameof(ConfigTakServerListPage), typeof(ConfigTakServerListPage));
    }
}
