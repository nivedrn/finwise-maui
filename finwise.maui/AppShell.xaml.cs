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
                Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
                Routing.RegisterRoute(nameof(ExpenseEditorPage), typeof(ExpenseEditorPage));
            }

            Routing.RegisterRoute(nameof(ExpenseDetailPage), typeof(ExpenseDetailPage));
            Routing.RegisterRoute(nameof(ExpenseSplitPage), typeof(ExpenseSplitPage));
            Routing.RegisterRoute(nameof(PersonDetailPage), typeof(PersonDetailPage));

            appShellViewModel = new AppShellViewModel();
            BindingContext = appShellViewModel;
        }

        protected override async void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            if (args.Target.Location.OriginalString == "//ExpenseEditorPage")
            {
                Debug.WriteLine(args.Target.Location.OriginalString);
                ShellNavigatingDeferral token = args.GetDeferral();
                args.Cancel();
                token.Complete();
                await Shell.Current.Navigation.PushModalAsync(new ExpenseEditorPage(null), true);
            }

        }
    }
}
