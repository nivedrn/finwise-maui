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
        decimal budgetProgress;

        [ObservableProperty]
        public string budgetMonthDetails;

        [ObservableProperty]
        string budgetProgressStatus;

        [ObservableProperty]
        string budgetProgressStatusCode;

        [ObservableProperty]
        bool showBudgetStatusMessage;

        [ObservableProperty]
        bool withinBudget;

        [ObservableProperty]
        public List<string> budgetProgressDetails;

        public HomePageViewModel() {
        
            Title = "Expenses";
            localBVM = App._bvm;
            IsBusy = false;
            WithinBudget = true;
            filterParams = new Dictionary<string, string> { { "searchTerm", "" } };
            BudgetProgressDetails = new List<string>()
            { "","","",""
            };
            BudgetProgressStatusCode = "DEFAULT";
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
                UpdateBudgetProgressBar();
            });
        }

        public async void UpdateBudgetProgressBar()
        {
            var today = DateTime.Today;
            WithinBudget = true;
            DateTime startDate = new DateTime(today.Year, today.Month, 1);
            DateTime endDate = startDate.AddMonths(1);

            var thisMonthsExpenses = App._bvm.Expenses.Where(expense => expense.expenseDate >= startDate && expense.expenseDate < endDate).ToList();
            decimal totalAmount = thisMonthsExpenses.Sum(expense => expense.amount);

            decimal monthlyBudget = decimal.Parse(App._settings["monthlyBudget"]);

            BudgetMonthDetails = $"{startDate.ToString("MMMM yyyy")}";

            if (monthlyBudget > 0)
            {
                BudgetProgress = totalAmount / monthlyBudget;
                endDate = endDate.AddDays(-1);
                BudgetProgressStatusCode = "DEFAULT";

                ShowBudgetStatusMessage = false;
                if(totalAmount > monthlyBudget)
                {
                    WithinBudget = false;
                    BudgetProgressStatusCode = "DANGER";
                }
                else if(totalAmount == monthlyBudget)
                {
                    BudgetProgressStatusCode = "WARNING";
                }
            }
            else
            {
                BudgetProgress = 1;
                ShowBudgetStatusMessage = true;
                BudgetProgressStatus = "No budget set. Please set a budget in Settings.";
                BudgetProgressStatusCode = "NOTIFY";
            }

            BudgetProgressDetails = new List<string>()
            {   "Spent : ",
                $"{App._settings["currentCurrencySymbol"]} {totalAmount}",
                "   /  Budget : ",
                $"{App._settings["currentCurrencySymbol"]} {monthlyBudget}"
            };

            IsBusy = false;
        }
    }
}
