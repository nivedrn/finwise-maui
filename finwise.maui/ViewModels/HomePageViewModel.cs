using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using finwise.maui.Models;
using finwise.maui.Views;
using finwise.maui.Handlers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace finwise.maui.ViewModels
{
    public partial class HomePageViewModel: BaseViewModel
    {
        public ObservableCollection<ExpenseDB> ExpensesDB { get; set; }

        public BaseViewModel localBVM { get; set; }

        public HomePageViewModel() {
        
            Title = "Home";
            App._bvm.Expenses = new ObservableCollection<Expense>(App._expenses);
            ExpensesDB = new ObservableCollection<ExpenseDB>();
            localBVM = App._bvm;
        }

        [RelayCommand]
        public void RefreshExpenses()
        {
            App._bvm.Expenses = new ObservableCollection<Expense>(App._expenses);
            //return null;
        }

        [RelayCommand]
        public static async Task OpenExpenseDetail(Object obj)
        {
            if(obj is not null)
            {
                await Shell.Current.Navigation.PushModalAsync(new ExpenseDetailPage((Expense)obj), true);
            }
        }

        private async Task FetchExpenses()
        {
            var expenses = await App._localDB.GetItems<ExpenseDB>();

            ExpensesDB.Clear();
            foreach (var expense in expenses)
            {
                ExpensesDB.Add(expense);
            }
        }
    }
}
