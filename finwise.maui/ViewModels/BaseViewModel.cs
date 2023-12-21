using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using finwise.maui.Models;
using CommunityToolkit.Maui.Core;

namespace finwise.maui.ViewModels
{
    [Serializable]
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        string title;

        [ObservableProperty]
        bool rememberMe;

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        bool isNotBusy;

        public ObservableCollection<Expense> Expenses { get; set; }
        public ObservableCollection<Person> People { get; set; }

        public BaseViewModel() {}

        [RelayCommand]
        private static async Task NavigateTo(string target)
        {
            await Shell.Current.GoToAsync($"{target}");
        }

        [RelayCommand]
        private static async Task PopModal()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }

        public void SetBusyState(bool busy)
        {
            this.IsBusy = busy;
            this.IsNotBusy = !busy;
        }
    }
}
