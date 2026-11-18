using CommunityToolkit.Maui;
using FinWise.Core.Repositories;
using FinWise.Data;
using FinWise.Data.Repositories;
using finwise.maui.Services;
using finwise.maui.ViewModels;
using finwise.maui.Views;
using Microsoft.Extensions.Logging;

namespace finwise.maui
{
    public static class MauiProgram
    {
        private const string DatabaseFileName = "finwise.db3";

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

            // Storage: one SQLite database for the whole app. The schema is created or upgraded
            // (and default categories seeded) the first time a repository uses it.
            builder.Services.AddSingleton(TimeProvider.System);
            builder.Services.AddSingleton(sp => new FinWiseDatabase(
                Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName),
                sp.GetRequiredService<TimeProvider>()));
            builder.Services.AddSingleton<IPersonRepository, PersonRepository>();
            builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
            builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
            builder.Services.AddSingleton<ISettlementRepository, SettlementRepository>();

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
