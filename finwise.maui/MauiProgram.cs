using CommunityToolkit.Maui;
using finwise.maui.Services;
using finwise.maui.ViewModels;
using finwise.maui.Views;
using Microsoft.Extensions.Logging;

namespace finwise.maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<AppShell>();

            // View models
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<ActivityViewModel>();
            builder.Services.AddTransient<AddActivityViewModel>();
            builder.Services.AddTransient<SearchViewModel>();

            // Tab pages: Shell resolves these from the container when it creates them.
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ActivityPage>();
            builder.Services.AddTransient<PeoplePage>();
            builder.Services.AddTransient<ProfilePage>();

            // Modal pages: created fresh each time they are opened.
            builder.Services.AddTransient<AddActivityPage>();
            builder.Services.AddTransient<SearchPage>();
            builder.Services.AddTransient<ActivityDetailPage>();

            return builder.Build();
        }
    }
}
