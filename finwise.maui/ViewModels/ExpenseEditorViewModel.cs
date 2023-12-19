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
        Boolean shareUpdated;

        [ObservableProperty]
        string currentCurrencySymbol;

        public ObservableCollection<string> expenseTags { get; set; }

        public ObservableCollection<Person> selectableMembers { get; set; }
        public ObservableCollection<ExpenseShare> tempExpenseShares { get; set; }
        public ObservableCollection<ExpenseDebt> tempExpenseDebts { get; set; }

        [ObservableProperty]
        public bool showSelectableMembers;

        [ObservableProperty]
        public decimal tempPaidByTotal;

        [ObservableProperty]
        public decimal tempExpenseSplitTotal;

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
                ExpenseShare appUserShare = new ExpenseShare();
                appUserShare.personId = App._settings["userId"];
                appUserShare.isAppUser = true;
                appUserShare.hasPaid = true;
                tempExpenseShares = new ObservableCollection<ExpenseShare>{
                    appUserShare
                };
                tempExpenseDebts = new ObservableCollection<ExpenseDebt>();

                ShareUpdated = false;
            }
            else
            {
                ExpenseItem = expense;
                currentIndex = App._bvm.Expenses.IndexOf(expense);
                isEditMode = true;
                Title = "Modify Expense";
                ShareUpdated = expense.isShared;
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
                this.ExpenseItem.expenseDebts = tempExpenseDebts.ToList();
                this.ExpenseItem.isShared = true;
                await Shell.Current.Navigation.PopModalAsync();
                ShareUpdated = true;
            }
        }

        public ObservableCollection<Person> RefreshPeopleList(string searchTerm)
        {
            var existingIds = tempExpenseShares.Select(share => share.personId).ToList();

            if (searchTerm is not null)
            {
                return new ObservableCollection<Person>(App._bvm.People.Where(
                    person => person.name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) && !person.isDeleted &&
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
                ExpenseShare tempShare = new ExpenseShare();
                tempShare.personId = ((Person)obj).id;
                tempExpenseShares.Add(tempShare);

                if (MainThread.IsMainThread)
                    RecalculateSplit();

                else
                    MainThread.BeginInvokeOnMainThread(RecalculateSplit);
            }
        }

        public void RecalculateSplit()
        {
            tempExpenseDebts = new ObservableCollection<ExpenseDebt>();

            foreach (ExpenseShare share in tempExpenseShares)
            {
                if (share.paidAmount > 0) share.hasPaid = true;
                if (share.shareAmount > 0) share.hasShare = true;

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

                if(share.hasShare || share.hasPaid) 
                {
                    var remainingShare = share.shareAmount - share.paidAmount;

                    if (remainingShare > 0)
                    {
                        foreach (ExpenseShare personOwed in tempExpenseShares.Where(es => es.paidAmount > es.shareAmount && es.personId != share.personId).OrderByDescending(es => es.paidAmount - es.shareAmount).ToList())
                        {
                            var personOwedDebtTally = tempExpenseDebts.Where(es => es.toPersonId == personOwed.personId).Sum(es => es.debtAmount);
                            var remainingOwedDebt = (personOwed.paidAmount - personOwed.shareAmount) - personOwedDebtTally;

                            ExpenseDebt debt = new ExpenseDebt();
                            debt.fromPersonId = share.personId;
                            debt.toPersonId = personOwed.personId;

                            if(remainingShare >= remainingOwedDebt)
                            {
                                debt.debtAmount = remainingOwedDebt;
                            }
                            else
                            {
                                debt.debtAmount = remainingShare;
                            }

                            remainingShare = remainingShare - debt.debtAmount;
                            tempExpenseDebts.Add(debt);
                        }
                    }
                }                
            }
        }

        public bool ValidateSplit()
        {
            return true;
        }
    }
}
