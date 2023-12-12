using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using finwise.maui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.ViewModels
{
    public partial class PeoplePageViewModel: BaseViewModel
    {
        public BaseViewModel localBVM { get; set; }

        [ObservableProperty]
        Dictionary<string, string> filterParams;

        public PeoplePageViewModel()
        {
            Title = "People";
            localBVM = App._bvm;
            filterParams = new Dictionary<string, string> { { "searchTerm", "" } };
        }

        [RelayCommand]
        public static async Task OpenExpenseDetail(Object obj)
        {
            if (obj is not null)
            {
                //await Shell.Current.Navigation.PushModalAsync(new ExpenseDetailPage((Expense)obj), true);
            }
        }

        public ObservableCollection<Person> RefreshPeopleList()
        {
            if (this.FilterParams["searchTerm"] != "")
            {
                return new ObservableCollection<Person>(localBVM.People.Where(exp => exp.name.Contains(this.FilterParams["searchTerm"], StringComparison.OrdinalIgnoreCase))?.ToList());
            }

            return new ObservableCollection<Person>(App._bvm.People);
        }
    }
}
