using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using finwise.maui.Helpers;
using finwise.maui.Models;
using finwise.maui.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.ViewModels
{
    [Serializable]
    public partial class PeoplePageViewModel: BaseViewModel
    {

        [ObservableProperty]
        Dictionary<string, string> filterParams;

        [ObservableProperty]
        string sheetTitle;

        [ObservableProperty]
        Person currentIndexPerson;

        //[ObservableProperty]
        //string talliedAmountSummary;

        [ObservableProperty]
        Dictionary<string,string> talliedAmountSummary;

        [ObservableProperty]
        bool isTallyAmountOwedToYou;

        public int currentIndex {  get; set; }
        public int peopleCount { get; set; }

        [ObservableProperty]
        string appUserId;

        [ObservableProperty]
        bool isFilteredResults;

        [ObservableProperty]
        ObservableCollection<Person> peopleCollection;

        [ObservableProperty]
        ObservableCollection<ExpenseWithPerson> relatedExpenseDebtsCollection;

        public PeoplePageViewModel()
        {
            Title = "Friends";
            AppUserId = App._settings["userId"];
            FilterParams = new Dictionary<string, string>();
            TalliedAmountSummary = new Dictionary<string, string>();

            TalliedAmountSummary["message"] = "";
            TalliedAmountSummary["amount"] = "";
            TalliedAmountSummary["isOwedToUser"] = "false";

            ResetFilterParams();
            RefreshPeopleList();
        }

        public void Init()
        {
            peopleCount = App._bvm.People.Count;

        }

        public void ResetFilterParams()
        {
            FilterParams["searchTerm"] = "";
            FilterParams["isDeleted"] = "false";
            FilterParams["showAll"] = "true";
            FilterParams["showIfOwesYou"] = "false";
            FilterParams["showIfYouOwe"] = "false";
            FilterParams["sortByCreatedDateAsc"] = "false";
            FilterParams["sortByCreatedDateDesc"] = "true";
            FilterParams["sortByAmountAsc"] = "false";
            FilterParams["sortByAmountDesc"] = "false";
            FilterParams["sortByNameAsc"] = "false";
            FilterParams["sortByNameDesc"] = "false";
            FilterParams["isFiltered"] = "false";
            IsFilteredResults = false;
        }

        public void AddNewPerson(string name)
        {
            if(name != "")
            {
                Person person = new Person();
                person.name = name;
                person.id = Guid.NewGuid().ToString();
                person.createdDate = DateTime.Now;
                person.modifiedDate = DateTime.Now;
                person.isDeleted = false;

                App._bvm.People.Insert(0, person);
                RefreshPeopleList();
            }
            MyStorage.WriteToDataFile<Person>(App._bvm.People.ToList());
        }

        [RelayCommand]
        public async Task OpenPersonDetail(Object obj)
        {
            if (obj is not null)
            {
                currentIndex = App._bvm.People.IndexOf((Person)obj);
                CurrentIndexPerson = App._bvm.People[currentIndex];
                RelatedExpenseDebtsCollection = new ObservableCollection<ExpenseWithPerson>();
                await Shell.Current.Navigation.PushModalAsync(new PersonDetailPage(this), true);
            }
        }

        public void RefreshPeopleList()
        {
            IsFilteredResults = false;
            var results = new ObservableCollection<Person>(App._bvm.People);

            if (this.FilterParams["searchTerm"] != "")
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp.name.Contains(this.FilterParams["searchTerm"], StringComparison.OrdinalIgnoreCase)).ToObservableCollection();
            }

            if (bool.Parse(FilterParams["showIfYouOwe"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => !exp.owesYou).ToObservableCollection();
            }
            else if (bool.Parse(FilterParams["showIfOwesYou"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp.owesYou).ToObservableCollection();
            }

            if (bool.Parse(FilterParams["sortByCreatedDateAsc"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp != null).OrderBy(item => item.createdDate).ToObservableCollection();
            }
            else if (bool.Parse(FilterParams["sortByCreatedDateDesc"]))
            {
                results = results.Where(exp => exp != null).OrderByDescending(item => item.createdDate).ToObservableCollection();
            }
            else if (bool.Parse(FilterParams["sortByAmountAsc"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp != null).OrderBy(item => item.talliedAmount).ToObservableCollection();
            }
            else if (bool.Parse(FilterParams["sortByAmountDesc"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp != null).OrderByDescending(item => item.talliedAmount).ToObservableCollection();
            }
            else if (bool.Parse(FilterParams["sortByNameAsc"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp != null).OrderBy(item => item.name).ToObservableCollection();
            }
            else if (bool.Parse(FilterParams["sortByNameDesc"]))
            {
                IsFilteredResults = true;
                results = results.Where(exp => exp != null).OrderByDescending(item => item.name).ToObservableCollection();

            }

            InitUpdateOverallTallyAmount();

            PeopleCollection = new ObservableCollection<Person>(results);
        }

        public void InitUpdateOverallTallyAmount()
        {
            Task.Run(() =>
            {
                IsBusy = true;
                UpdateOverallTallyAmount();
            });
        }

        public void UpdateOverallTallyAmount()
        {
            decimal totalTalliedAmount = 0;
            bool hasSharedExpenses = false;
            bool allExpensesSettled = true;
            TalliedAmountSummary = new Dictionary<string, string>();
            TalliedAmountSummary["message"] = "";
            TalliedAmountSummary["amount"] = "";
            TalliedAmountSummary["isOwedToUser"] = "false";

            foreach (Person prsn in App._bvm.People)
            {
                var debtsWithUser = App._bvm.Expenses
                                        .SelectMany(expense => expense.expenseDebts)
                                        .Where(debt => debt.fromPersonId == prsn.id || debt.toPersonId == prsn.id).ToList();

                if(debtsWithUser.Count > 0)
                {
                    var debtTotalWithUser = debtsWithUser.Sum(debt => debt.fromPersonId == prsn.id ? debt.debtAmount : -debt.debtAmount);

                    hasSharedExpenses = true;

                    prsn.hasDebts = true;
                    prsn.isSettledUp = true;

                    if (debtTotalWithUser != 0)
                    {
                        allExpensesSettled = false;
                        prsn.isSettledUp = false;
                        prsn.owesYou = debtTotalWithUser >= 0;
                        totalTalliedAmount += debtTotalWithUser;

                        if (debtTotalWithUser < 0) debtTotalWithUser *= -1;
                    }

                    prsn.talliedAmount = debtTotalWithUser;
                }
                else
                {
                    prsn.talliedAmount = 0;
                    prsn.owesYou = true;
                    prsn.hasDebts = false;
                    prsn.isSettledUp = false;
                }
            }

            if (hasSharedExpenses)
            {
                if (totalTalliedAmount > 0)
                {
                    TalliedAmountSummary["message"] = $"Overall, you are owed ";
                    TalliedAmountSummary["amount"] = $"{App._settings["currentCurrencySymbol"]} {totalTalliedAmount.ToString("0.00")}";
                    TalliedAmountSummary["isOwedToUser"] = "true";

                }
                else if (totalTalliedAmount < 0)
                {
                    totalTalliedAmount *= -1;
                    TalliedAmountSummary["message"] = $"Overall, you owe ";
                    TalliedAmountSummary["amount"] = $"{App._settings["currentCurrencySymbol"]} {totalTalliedAmount.ToString("0.00")}";
                    TalliedAmountSummary["isOwedToUser"] = "false";
                }
                else
                {
                    TalliedAmountSummary["message"] = $"Looks Good! All expenses are all settled up.";
                }

            }
            else
            {
                TalliedAmountSummary["message"] = $"No shared expenses created.";
            }

            IsBusy = false;
        }

        public class ExpenseWithPerson
        {
            public Expense expense { get; set; }
            public ExpenseDebt debt { get; set; }
            public bool owesYou { get; set; }
        }

        public ObservableCollection<ExpenseWithPerson> FetchRelatedExpenses()
        {
            var result = new ObservableCollection<ExpenseWithPerson>();

            var userId = App._settings["userId"];

            if(CurrentIndexPerson is not null)
            {
                foreach (var expense in App._bvm.Expenses)
                {
                    var relatedExpenseDebt = expense.expenseDebts.FirstOrDefault(es => (es.fromPersonId == userId || es.fromPersonId == CurrentIndexPerson.id) && (es.toPersonId == userId || es.toPersonId == CurrentIndexPerson.id));

                    if (relatedExpenseDebt != null)
                    {
                        var expenseWithPerson = new ExpenseWithPerson
                        {
                            expense = expense,
                            debt = relatedExpenseDebt,
                            owesYou = relatedExpenseDebt.toPersonId == userId
                        };
                        result.Add(expenseWithPerson);
                    }
                }
            }

            return result;

        }
    }
}
