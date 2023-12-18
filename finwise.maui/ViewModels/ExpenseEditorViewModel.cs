using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using finwise.maui.Helpers;
using finwise.maui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        [ObservableProperty]
        string currentCurrencySymbol;

        public ObservableCollection<string> expenseTags { get; set; }

        public ObservableCollection<Person> selectableMembers { get; set; }
        public ObservableCollection<ExpenseShare> tempExpenseShares { get; set; }

        [ObservableProperty]
        public bool showSelectableMembers;

        public int currentIndex {  get; set; }
        
        public ExpenseEditorViewModel(Expense expense)
        {
            if (expense == null)
            {
                ExpenseItem = new Expense();
                isEditMode = false;
                Title = "Add new Expense";
                ExpenseItem.id = Guid.NewGuid().ToString();
                expenseTags = new ObservableCollection<string>();
                tempExpenseShares = new ObservableCollection<ExpenseShare>{
                    new ExpenseShare(App._settings["userId"], true)
                };
            }
            else
            {
                ExpenseItem = expense;
                currentIndex = App._bvm.Expenses.IndexOf(expense);
                isEditMode = true;
                Title = "Modify Expense";
                expenseTags = new ObservableCollection<string>(ExpenseItem.tags);
                tempExpenseShares = new ObservableCollection<ExpenseShare>(ExpenseItem.expenseShares); 
            }

            ShowSelectableMembers = true;
            selectableMembers = new ObservableCollection<Person>(App._bvm.People);
            currentCurrencySymbol = App._settings["currentCurrencySymbol"];
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

        [RelayCommand]
        private async Task SaveExpenseSplit()
        {
            if (ValidateSplit())
            {
                this.ExpenseItem.expenseShares = tempExpenseShares.ToList();
                this.ExpenseItem.isShared = true;
                await Shell.Current.Navigation.PopModalAsync();
            }
        }

        public ObservableCollection<Person> RefreshPeopleList(string searchTerm)
        {
            var existingIds = tempExpenseShares.Select(share => share.personId).ToList();

            if (searchTerm is not null)
            {
                return new ObservableCollection<Person>(App._bvm.People.Where(
                    person => person.name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) &&
                    !existingIds.Contains(person.id))?
                    .ToList());
            }

            return new ObservableCollection<Person>();
        }

        [RelayCommand]
        public void selectableMembersResult_SelectionChanged(Object obj)
        {
            if (obj is not null)
            {
                ShowSelectableMembers = false;
                tempExpenseShares.Add(new ExpenseShare(((Person)obj).id, false));
                RecalculateSplit();
            }
        }

        public void RecalculateSplit()
        {
            foreach (ExpenseShare share in tempExpenseShares)
            {
                switch (ExpenseItem.paidByType)
                {
                    case "Paid By You":
                        if (share.hasPaid)
                        {
                            share.paidAmount = ExpenseItem.amount;
                        }
                        break;

                    default:
                        break;
                }

                switch (ExpenseItem.shareType)
                {
                    case "Equally":
                        if (share.hasShare)
                        {
                            share.shareAmount = ExpenseItem.amount / tempExpenseShares.Count;
                        }
                        break;

                    default:
                        break;
                }
            }

        }

        public bool ValidateSplit()
        {
            return true;
        }
    }
}
