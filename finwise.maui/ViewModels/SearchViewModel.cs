using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using finwise.maui.Services;

namespace finwise.maui.ViewModels
{
    public partial class SearchViewModel : BaseViewModel
    {
        private readonly INavigationService navigation;

        public SearchViewModel(INavigationService navigation)
        {
            this.navigation = navigation;
            Title = "Search";
        }

        /// <summary>The search box text. Nothing is searched yet: there is no data layer.</summary>
        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        [RelayCommand]
        private Task CloseAsync() => navigation.CloseModalAsync();
    }
}
