using finwise.maui.ViewModels;
namespace finwise.maui
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
            InitializeComponent();

            MainPage = new AppShell();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<AddExpenseViewModel>();
            services.AddTransient<AppShellViewModel>();
            return services.BuildServiceProvider();
        }
    }
}
