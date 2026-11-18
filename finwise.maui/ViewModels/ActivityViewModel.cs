using CommunityToolkit.Mvvm.Input;
using finwise.maui.Services;

namespace finwise.maui.ViewModels
{
    public partial class ActivityViewModel : BaseViewModel
    {
        private readonly INavigationService navigation;

        public ActivityViewModel(INavigationService navigation)
        {
            this.navigation = navigation;
            Title = "Activity";
        }

        // Filters live on the search page for now, so both header buttons open it.
        [RelayCommand]
        private Task OpenSearchAsync() => navigation.OpenSearchAsync();
    }
}
