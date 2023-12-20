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

        [ObservableProperty]
        string talliedAmountSummary;

        [ObservableProperty]
        bool isTallyAmountOwedToYou;

        public int currentIndex {  get; set; }
        public int peopleCount { get; set; }

        [ObservableProperty]
        string appUserId;

        [ObservableProperty]
        ObservableCollection<Person> peopleCollection;

        [ObservableProperty]
        ObservableCollection<ExpenseWithPerson> relatedExpenseDebtsCollection;

        public PeoplePageViewModel()
        {
            Title = "Friends";
            filterParams = new Dictionary<string, string> { { "searchTerm", "" }, { "isDeleted" , "false" } };
            AppUserId = App._settings["userId"];
            RefreshPeopleList();
            InitUpdateOverallTallyAmount();
        }

        public void Init()
        {
            peopleCount = App._bvm.People.Count;

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
            var results = App._bvm.People;

            if (this.FilterParams["isDeleted"] != "true")
            {
                results = new ObservableCollection<Person>(results.Where(exp => !exp.isDeleted)?.ToList());
            }

            if (this.FilterParams["searchTerm"] != "")
            {
                results =  new ObservableCollection<Person>(results.Where(exp => exp.name.Contains(this.FilterParams["searchTerm"], StringComparison.OrdinalIgnoreCase))?.ToList());
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
            foreach (Person prsn in App._bvm.People)
            {
                var debtTotalWithUser = App._bvm.Expenses
                                        .SelectMany(expense => expense.expenseDebts)
                                        .Where(debt => debt.fromPersonId == prsn.id || debt.toPersonId == prsn.id)
                                        .Sum(debt => debt.fromPersonId == prsn.id ? debt.debtAmount : -debt.debtAmount);

                prsn.talliedAmount = debtTotalWithUser;
                prsn.owesYou = debtTotalWithUser > 0;
                totalTalliedAmount += debtTotalWithUser;
            }

            if (totalTalliedAmount > 0)
            {
                TalliedAmountSummary = $"Overall you are owed {App._settings["currentCurrencySymbol"]} {totalTalliedAmount}";

            }else if (totalTalliedAmount < 0)
            {
                totalTalliedAmount *= -1;
                TalliedAmountSummary = $"Overall you owe {App._settings["currentCurrencySymbol"]} {totalTalliedAmount}";
            }
            else
            {
                totalTalliedAmount *= -1;
                TalliedAmountSummary = $"Looks Good! You are all settled up.";
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
