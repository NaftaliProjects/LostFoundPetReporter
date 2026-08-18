using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.ViewModels;
using LostFoundPetReporter.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace LostFoundPetReporter.Mobile;

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
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // =========================
        // API
        // =========================

        builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(
                "https://localhost:7074/");
        });

        // =========================
        // API Services
        // =========================

        builder.Services.AddTransient<IUserApiService, UserApiService>();

        // =========================
        // ViewModels
        // =========================
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();

        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<CameraViewModel>();
        builder.Services.AddTransient<MapViewModel>();
        builder.Services.AddTransient<MyReportsViewModel>();
        builder.Services.AddTransient<UsersViewModel>();

        // =========================
        // Views
        // =========================
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<CameraPage>();
        builder.Services.AddTransient<MapPage>();
        builder.Services.AddTransient<MyReportsPage>();
        builder.Services.AddTransient<UsersPage>();

        return builder.Build();
    }
}