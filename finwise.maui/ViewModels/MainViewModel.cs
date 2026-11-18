using CommunityToolkit.Mvvm.Input;
using finwise.maui.Services;

namespace finwise.maui.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly INavigationService navigation;

        public MainViewModel(INavigationService navigation)
        {
            this.navigation = navigation;
            Title = "Home";
        }

        [RelayCommand]
        private Task OpenSearchAsync() => navigation.OpenSearchAsync();
    }
}
