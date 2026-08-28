
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Map;

using LostFoundPetReporter.Mobile.Services.Session;
using LostFoundPetReporter.Mobile.ViewModels;
using LostFoundPetReporter.Mobile.Views;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;





namespace LostFoundPetReporter.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
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
        var handler = new HttpClientHandler();

        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;


        builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(
                "https://10.100.102.27:7074/");
        }).ConfigurePrimaryHttpMessageHandler(() => handler);


        // =========================
        // API Services
        // =========================

        builder.Services.AddTransient<IUserApiService, UserApiService>();
        builder.Services.AddTransient<ILostReportApiService, LostReportApiService>();
        builder.Services.AddTransient<IFoundReportApiService, FoundReportApiService>();
        builder.Services.AddSingleton<IUserSession, UserSession>();
        builder.Services.AddTransient<IMapService, MapService>();

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
        builder.Services.AddTransient<CreateLostReportViewModel>();
        builder.Services.AddTransient<CreateFoundReportViewModel>();
        builder.Services.AddTransient<SpecificLostReportViewModel>();
        

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
        builder.Services.AddTransient<CreateLostReportPage>();
        builder.Services.AddTransient<CreateFoundReportPage>();
        builder.Services.AddTransient<SpecificLostReportPage>();


        return builder.Build();
    }
}