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
using static Android.Net.Wifi.WifiEnterpriseConfig;
using CommunityToolkit.Maui.Core.Extensions;

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
        bool isFilteredResults;

        [ObservableProperty]
        public List<string> budgetProgressDetails;

        public HomePageViewModel() {
        
            Title = "Expenses";
            localBVM = App._bvm;
            IsBusy = false;
            WithinBudget = true;
            IsFilteredResults = false;
            FilterParams = new Dictionary<string, string>();
            ResetFilterParams();
            BudgetProgressDetails = new List<string>()
            { "","","",""
            };
            BudgetProgressStatusCode = "DEFAULT";
            InitUpdateBudgetProgressBar();
        }

        public void ResetFilterParams()
        {
            FilterParams["searchTerm"] = "";
            FilterParams["isDeleted"] = "false";
            FilterParams["showAll"] = "true";
            FilterParams["showIfYouOwe"] = "false";
            FilterParams["showIfOwesYou"] = "false";
            FilterParams["sortByExpenseDateAsc"] = "false";
            FilterParams["sortByExpenseDateDesc"] = "false";
            FilterParams["sortByAmountAsc"] = "false";
            FilterParams["sortByAmountDesc"] = "false";
            FilterParams["sortByNameAsc"] = "false";
            FilterParams["sortByNameDesc"] = "false";
            FilterParams["defaultSort"] = "true";
            IsFilteredResults = false;
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
            IsFilteredResults = false;
            var results = new ObservableCollection<Expense>(App._bvm.Expenses);

            if (this.FilterParams["searchTerm"] != "")
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp.amount.ToString().Contains(this.FilterParams["searchTerm"], StringComparison.OrdinalIgnoreCase) || exp.description.Contains(this.FilterParams["searchTerm"], StringComparison.OrdinalIgnoreCase) || exp.category.Contains(this.FilterParams["searchTerm"], StringComparison.OrdinalIgnoreCase)).ToObservableCollection();
            }

            if (bool.Parse(FilterParams["sortByExpenseDateAsc"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp != null).OrderBy(item => item.expenseDate).ToObservableCollection();
            }
            else if (bool.Parse(FilterParams["sortByExpenseDateDesc"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp != null).OrderByDescending(item => item.expenseDate).ToObservableCollection();
            }
            else if (bool.Parse(FilterParams["sortByAmountAsc"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp != null).OrderBy(item => item.amount).ToObservableCollection();
            }
            else if (bool.Parse(FilterParams["sortByAmountDesc"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp != null).OrderByDescending(item => item.amount).ToObservableCollection();
            }
            else if (bool.Parse(FilterParams["sortByNameAsc"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp != null).OrderBy(item => item.description).ToObservableCollection();
            }
            else if (bool.Parse(FilterParams["sortByNameDesc"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp != null).OrderByDescending(item => item.description).ToObservableCollection();
            }

            return results; 
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
            BudgetProgressStatus = "";
            BudgetProgressStatusCode = "DEFAULT";
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
                BudgetProgressStatus = "Go to Settings to set a Budget.";
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
