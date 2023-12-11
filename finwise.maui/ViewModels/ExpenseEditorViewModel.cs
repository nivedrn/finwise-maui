using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using finwise.maui.Helpers;
using finwise.maui.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.ViewModels
{
    public partial class ExpenseEditorViewModel: BaseViewModel
    {
        [ObservableProperty]
        Expense expenseItem;

        [ObservableProperty]
        Boolean isEditMode;

        public int currentIndex {  get; set; }
        //public BaseViewModel localBVM { get; set; }

        public ExpenseEditorViewModel(Expense expense)
        {
            //localBVM = App._bvm;
            if (expense == null)
            {
                this.ExpenseItem = new Expense();
                isEditMode = false;
                Title = "Add new Expense";
                this.ExpenseItem.id = Guid.NewGuid().ToString();
                this.ExpenseItem.description = "Test";
                this.ExpenseItem.amount = 20;
            }
            else
            {
                this.ExpenseItem = expense;
                currentIndex = App._bvm.Expenses.IndexOf(expense);
                isEditMode = true;
                Title = "Modify Expense";
            }
        }

        [RelayCommand]
        private async Task SaveExpense()
        {
            if (!IsEditMode)
            {
                this.ExpenseItem.createdDate = DateTime.Now;
                this.ExpenseItem.modifiedDate = DateTime.Now;
                App._bvm.Expenses.Insert(0, this.ExpenseItem);
            }
            else
            {
                this.ExpenseItem.modifiedDate = DateTime.Now;
                App._bvm.Expenses[currentIndex] = this.ExpenseItem;
            }
            MyStorage.WriteToDataFile<Expense>(App._bvm.Expenses.ToList());
            await Shell.Current.Navigation.PopModalAsync();
        }
        
    }
}
