using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using Tak_lite.Service;


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
				fonts.AddFont(filename: "MaterialIcons-Regular.ttf", alias: "MaterialDesignIcons");
			})
            .UseMauiMaps()
            .ConfigureSyncfusionCore()
            ;
        builder.Services.AddSingleton<ConfigurationPage>();
        builder.Services.AddSingleton<ConfigurationViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<LocationService>();
        builder.Services.AddTransient<TakService>();
		
#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
