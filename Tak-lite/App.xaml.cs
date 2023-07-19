using Syncfusion.Maui.Themes;

namespace Tak_lite;

public partial class App : Application
{
	public App()
	{
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjU4MjQyOUAzMjMyMmUzMDJlMzBsS1pQaU5yTHRvWFN4Q0l2SjBEdmd4ZVN2MVBWTmhKaDAvZElvRjZkbzZBPQ==");
		InitializeComponent();

		MainPage = new AppShell();
	}
}
