using finwise.maui.Views;

namespace finwise.maui.Services
{
    public sealed class NavigationService(IServiceProvider services) : INavigationService
    {
        private static INavigation Navigation =>
            Shell.Current?.Navigation ?? throw new InvalidOperationException("Shell is not ready yet.");

        public Task OpenSearchAsync() => PushModalAsync<SearchPage>();

        public Task OpenAddActivityAsync() => PushModalAsync<AddActivityPage>();

        public Task CloseModalAsync() =>
            Navigation.ModalStack.Count > 0 ? Navigation.PopModalAsync(animated: true) : Task.CompletedTask;

        private Task PushModalAsync<TPage>() where TPage : Page =>
            Navigation.PushModalAsync(services.GetRequiredService<TPage>(), animated: true);
    }
}
