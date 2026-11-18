using System.Diagnostics;
using finwise.maui.Services;

namespace finwise.maui
{
    public partial class AppShell : Shell
    {
        private const string AddActivityRoute = "//AddActivityPage";

        private readonly INavigationService navigation;

        public AppShell(INavigationService navigation)
        {
            InitializeComponent();
            this.navigation = navigation;
        }

        protected override async void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            // The "+" tab is a button, not a destination: stay on the current tab
            // and open Add Activity as a modal instead.
            if (!args.Target.Location.OriginalString.StartsWith(AddActivityRoute, StringComparison.Ordinal))
                return;

            args.Cancel();
            try
            {
                await navigation.OpenAddActivityAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Opening Add Activity failed: {ex}");
            }
        }
    }
}
