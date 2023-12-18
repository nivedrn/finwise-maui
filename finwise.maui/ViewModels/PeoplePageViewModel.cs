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
        public BaseViewModel localBVM { get; set; }

        [ObservableProperty]
        Dictionary<string, string> filterParams;

        [ObservableProperty]
        string sheetTitle;

        [ObservableProperty]
        Person currentIndexPerson;

        public int currentIndex {  get; set; }

        public PeoplePageViewModel()
        {
            Title = "Friends";
            localBVM = App._bvm;
            filterParams = new Dictionary<string, string> { { "searchTerm", "" } };
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
                currentIndexPerson = App._bvm.People[currentIndex];
                await Shell.Current.Navigation.PushModalAsync(new PersonDetailPage(this), true);
            }
        }

        public ObservableCollection<Person> RefreshPeopleList()
        {
            if (this.FilterParams["searchTerm"] != "")
            {
                return new ObservableCollection<Person>(localBVM.People.Where(exp => exp.name.Contains(this.FilterParams["searchTerm"], StringComparison.OrdinalIgnoreCase))?.ToList());
            }

            return new ObservableCollection<Person>(localBVM.People);
        }
    }
}
