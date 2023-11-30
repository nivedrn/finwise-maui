using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        string title;

        public ICommand SearchCommand => new Command(async () => await ExecuteSearchCommand());
        public ICommand PopModal => new Command(async () => await ExecutePopModalCommand());

        public BaseViewModel() { }

        private static async Task ExecuteSearchCommand()
        {
            await Shell.Current.Navigation.PushModalAsync(new finwise.maui.Views.SearchPage(), true);
        }

        private static async Task ExecutePopModalCommand()
        {
            
            await Shell.Current.Navigation.PopModalAsync();
        }
    }
}
