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
        //Routing.RegisterRoute(nameof(MainArcgisPage),typeof(MainArcgisPage));
        Routing.RegisterRoute(nameof(ConfigKmlListPage),typeof(ConfigKmlListPage));
        Routing.RegisterRoute(nameof(ConfigAresPage),typeof(ConfigAresPage));
    }
}
