using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using finwise.maui.Handlers;
using finwise.maui.ViewModels;
using finwise.maui.Views;
using finwise.maui.Helpers;

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

            builder.Services.AddSingleton<DBHandler>();
            builder.Services.AddSingleton<MyStorage>();
            builder.Services.AddSingleton<BaseViewModel>();
            //builder.Services.AddSingleton<Logger>();

            builder.Services.AddTransient<AppShellViewModel>();
            builder.Services.AddTransient<ExpenseEditorPage>();
            builder.Services.AddTransient<ExpenseEditorViewModel>();
            builder.Services.AddTransient<HomePageViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
