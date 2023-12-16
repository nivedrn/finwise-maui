using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using finwise.maui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.ViewModels
{
    public partial class PeoplePageViewModel: BaseViewModel
    {
        public BaseViewModel localBVM { get; set; }

        [ObservableProperty]
        Dictionary<string, string> filterParams;

        [ObservableProperty]
        string sheetTitle;

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
        }
        
        public void AddNewGroup(string name)
        {
            if(name != "")
            {
                Group group = new Group();
                group.groupName = name;
                group.id = Guid.NewGuid().ToString();
                group.createdDate = DateTime.Now;
                group.modifiedDate = DateTime.Now;
                group.isDeleted = false;

                App._bvm.Groups.Insert(0, group);
            }
        }

        [RelayCommand]
        public static async Task OpenExpenseDetail(Object obj)
        {
            if (obj is not null)
            {
                //await Shell.Current.Navigation.PushModalAsync(new ExpenseDetailPage((Expense)obj), true);
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
