using CommunityToolkit.Mvvm.Messaging;
using finwise.maui.Helpers;
using finwise.maui.Models;
using finwise.maui.ViewModels;
namespace finwise.maui
{
    public partial class App : Application
    {
        public static List<Expense> _expenses { get; private set; }
        public static List<Person> _people { get; private set; }
        public static List<Group> _groups { get; private set; }

        public static BaseViewModel _bvm { get; private set; }

        public App(BaseViewModel bvm)
        {
            _bvm = bvm;

            _expenses = MyStorage.LoadFromDataFile<Expense>();
            _people = MyStorage.LoadFromDataFile<Person>();
            _groups = MyStorage.LoadFromDataFile<Group>();

            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnSleep()
        {
            MyStorage.WriteToDataFile<Expense>(_bvm.Expenses.ToList());
            MyStorage.WriteToDataFile<Person>(_bvm.People.ToList());
            MyStorage.WriteToDataFile<Group>(_bvm.Groups.ToList());
        }

        protected override void OnResume()
        {
            _expenses = MyStorage.LoadFromDataFile<Expense>();
            _people = MyStorage.LoadFromDataFile<Person>();
            _groups = MyStorage.LoadFromDataFile<Group>();
        }
    }
}
