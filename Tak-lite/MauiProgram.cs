using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using Tak_lite.Service;
using Tak_lite.ViewModels;


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

        builder.Services.AddTransient<ConfigTakServerDetailViewModel>();
        builder.Services.AddTransient<ConfigTakServerDetailPage>();
        builder.Services.AddTransient<ConfigTakServerListPage>();
        builder.Services.AddTransient<ConfigTakServerListViewModel>();
        builder.Services.AddSingleton<ConfigListingPage>();
        builder.Services.AddSingleton<ConfigListViewModel>();
        builder.Services.AddSingleton<ConfigPreferencesPage>();
        builder.Services.AddSingleton<ConfigPreferencesViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<LocationService>();
        builder.Services.AddTransient<TakServiceInstance>();
        builder.Services.AddSingleton<DataService>();
        builder.Services.AddSingleton<IMessenger>(provider => WeakReferenceMessenger.Default);

		
#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
