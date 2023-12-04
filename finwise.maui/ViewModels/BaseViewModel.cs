using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace finwise.maui.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        string title;

        [ObservableProperty]
        bool rememberMe;

        //public ICommand SearchCommand => new Command(async () => await ExecuteSearchCommand());
        //public ICommand PopModal => new Command(async () => await ExecutePopModalCommand());
        //public ICommand NavigateTo => new Command(async () => await NavigateToPageCommand());

        public BaseViewModel() { }

        [RelayCommand]
        private static async Task Search()
        {
            await Shell.Current.Navigation.PushModalAsync(new finwise.maui.Views.SearchPage(), true);
        }

        [RelayCommand]
        private static async Task NavigateTo()
        {
            await Shell.Current.GoToAsync("");
        }

        [RelayCommand]
        private static async Task PopModal()
        {

            await Shell.Current.Navigation.PopModalAsync();
        }
    }
}
