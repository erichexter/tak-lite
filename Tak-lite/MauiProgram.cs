using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
namespace Tak_lite;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
            .UseMauiMaps()
            .ConfigureSyncfusionCore()
            ;
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<LocationService>();
		
#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
