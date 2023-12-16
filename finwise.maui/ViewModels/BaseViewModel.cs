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
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        string title;

        [ObservableProperty]
        bool rememberMe;

        [ObservableProperty]
        bool isBusy;

        public ObservableCollection<Expense> Expenses { get; set; }
        public ObservableCollection<Person> People { get; set; }
        public ObservableCollection<Group> Groups { get; set; }

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
    }
}
