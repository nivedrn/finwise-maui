using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using finwise.maui.Models;
using finwise.maui.Views;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace finwise.maui.ViewModels
{
    public partial class HomePageViewModel: BaseViewModel
    {
        public BaseViewModel localBVM { get; set; }

        [ObservableProperty]
        Dictionary<string, string> filterParams;

        [ObservableProperty]
        public float budgetProgress;

        [ObservableProperty]
        public string budgetMonthDetails;

        [ObservableProperty]
        public List<string> budgetProgressDetails;

        public HomePageViewModel() {
        
            Title = "Expenses";
            localBVM = App._bvm;
            IsBusy = false;
            filterParams = new Dictionary<string, string> { { "searchTerm", "" } };
            BudgetProgressDetails = new List<string>()
            { "","","",""
            };
            InitUpdateBudgetProgressBar();
        }

        [RelayCommand]
        public static async Task OpenExpenseDetail(Object obj)
        {
            if(obj is not null)
            {
                await Shell.Current.Navigation.PushModalAsync(new ExpenseDetailPage((Expense)obj), true);
            }
        }
 
        public ObservableCollection<Expense> RefreshExpenseList()
        {
            if (this.FilterParams["searchTerm"] != "")
            {
                return new ObservableCollection<Expense>(localBVM.Expenses.Where(exp => exp.description.Contains(this.FilterParams["searchTerm"], StringComparison.OrdinalIgnoreCase))?.ToList());
            }

            return new ObservableCollection<Expense>(App._bvm.Expenses); 
        }

        public void InitUpdateBudgetProgressBar()
        {
            Task.Run(async () =>
            {
                IsBusy = true;
                await UpdateBudgetProgressBar();
            });
        }

        public async Task UpdateBudgetProgressBar()
        {
            var today = DateTime.Today;
            DateTime startDate = new DateTime(today.Year, today.Month, 1);
            DateTime endDate = startDate.AddMonths(1);

            var thisMonthsExpenses = App._bvm.Expenses.Where(expense => expense.expenseDate >= startDate && expense.expenseDate < endDate).ToList();
            float totalAmount = thisMonthsExpenses.Sum(expense => expense.amount);
            float budget = float.Parse(App._settings["monthlyBudget"]) != 0 ? float.Parse(App._settings["monthlyBudget"]) : totalAmount;
            BudgetProgress = totalAmount / budget;

            endDate = endDate.AddDays(-1);

            BudgetMonthDetails = $"{startDate.ToString("MMM dd, yyyy")} to {endDate.AddDays(-1).ToString("MMM dd, yyyy")}";

            BudgetProgressDetails = new List<string>()
            {   "Total money spent : ",
                $"{App._settings["currentCurrencySymbol"]} {totalAmount}",
                "Monthly Budget : ",
                $"{App._settings["currentCurrencySymbol"]} {budget}"
            };

            IsBusy = false;
        }

    }
}
