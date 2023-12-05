using finwise.maui.Views;
using finwise.maui.ViewModels;
using System.Diagnostics;

namespace finwise.maui
{
    public partial class AppShell : Shell
    {
        private AppShellViewModel appShellViewModel;

        public AppShell()
        {
            InitializeComponent();

            if (DeviceInfo.Idiom != DeviceIdiom.Phone)
            {
                Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
                Routing.RegisterRoute(nameof(PeoplePage), typeof(PeoplePage));
                Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
                Routing.RegisterRoute(nameof(AddExpensePage), typeof(AddExpensePage));
            }

            appShellViewModel = new AppShellViewModel();
            BindingContext = appShellViewModel;
        }

        protected override async void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            if (args.Target.Location.OriginalString == "//AddExpensePage")
            {
                Debug.WriteLine(args.Target.Location.OriginalString);
                ShellNavigatingDeferral token = args.GetDeferral();
                args.Cancel();
                token.Complete();
                await Shell.Current.Navigation.PushModalAsync(new AddExpensePage(), true);
            }

        }
    }
}
