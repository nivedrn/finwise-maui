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
using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;

namespace finwise.maui.ViewModels
{
    [Serializable]
    public partial class PeoplePageViewModel: BaseViewModel
    {
        public BaseViewModel localBVM { get; set; }

        [ObservableProperty]
        Dictionary<string, string> filterParams;

        [ObservableProperty]
        string sheetTitle;

        [ObservableProperty]
        Person currentIndexPerson;

        [ObservableProperty]
        string talliedAmountSummary;

        public int currentIndex {  get; set; }
        public int peopleCount { get; set; }


        public PeoplePageViewModel()
        {
            Title = "Friends";
            localBVM = App._bvm;
            peopleCount = App._bvm.People.Count;
            filterParams = new Dictionary<string, string> { { "searchTerm", "" }, { "isDeleted" , "false" } };

            InitUpdateOverallTallyAmount();
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
            }
            //MyStorage.WriteToDataFile<Person>(App._bvm.People.ToList());
        }

        [RelayCommand]
        public async Task OpenPersonDetail(Object obj)
        {
            if (obj is not null)
            {
                currentIndex = App._bvm.People.IndexOf((Person)obj);
                CurrentIndexPerson = App._bvm.People[currentIndex];
                await Shell.Current.Navigation.PushModalAsync(new PersonDetailPage(this), true);
            }
        }

        public ObservableCollection<Person> RefreshPeopleList()
        {
            var results = localBVM.People;

            if (this.FilterParams["isDeleted"] != "true")
            {
                results = new ObservableCollection<Person>(results.Where(exp => !exp.isDeleted)?.ToList());
            }

            if (this.FilterParams["searchTerm"] != "")
            {
                results =  new ObservableCollection<Person>(results.Where(exp => exp.name.Contains(this.FilterParams["searchTerm"], StringComparison.OrdinalIgnoreCase))?.ToList());
            }

            return new ObservableCollection<Person>(results);
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
    }
}
