﻿using Camera.MAUI;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging;
using Esri.ArcGISRuntime.Maui;
using Microsoft.Extensions.Logging;
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
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialDesignIcons");
            })
            .UseMauiMaps()
            //.ConfigureSyncfusionCore()
            .UseMauiCommunityToolkit()
            .UseArcGISRuntime()
            .UseMauiCameraView();
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
        builder.Services.AddSingleton<TakService>();
        builder.Services.AddSingleton<MainArcgisPage>();
        builder.Services.AddSingleton<MainArcgisViewModel>();
        builder.Services.AddSingleton<ConfigKmlListPage>();
        builder.Services.AddSingleton<ConfigKmlListViewModel>();
        builder.Services.AddTransient<ConfigAresPage>();
        builder.Services.AddTransient<ConfigAresDetailViewModel>();
        builder.Services.AddSingleton<AresAlphaService>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}