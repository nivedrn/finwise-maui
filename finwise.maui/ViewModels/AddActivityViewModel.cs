using CommunityToolkit.Mvvm.Input;
using finwise.maui.Services;

namespace finwise.maui.ViewModels
{
    public partial class AddActivityViewModel : BaseViewModel
    {
        private readonly INavigationService navigation;

        public AddActivityViewModel(INavigationService navigation)
        {
            this.navigation = navigation;
            Title = "Add Activity";
        }

        [RelayCommand]
        private Task CloseAsync() => navigation.CloseModalAsync();
    }
}
