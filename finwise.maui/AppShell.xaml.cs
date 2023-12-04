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

            Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));

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
