using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using finwise.maui.Helpers;
using finwise.maui.Models;
using finwise.maui.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.ViewModels
{
    public partial class ExpenseDetailViewModel: BaseViewModel
    {
        [ObservableProperty]
        Expense expenseItem;

        public int currentIndex {get;set;}
        public int expensesCount { get; set; }

        public ExpenseDetailViewModel(Expense expense)
        {
            this.currentIndex = App._bvm.Expenses.IndexOf(expense);
            this.expensesCount = App._bvm.Expenses.Count();
            this.Title = "";
            this.ExpenseItem = expense;
        }

        [RelayCommand]
        private async Task EditExpense()
        {
            await Shell.Current.Navigation.PushModalAsync(new ExpenseEditorPage(ExpenseItem), true);
        }
        
    }
}
