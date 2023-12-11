using CommunityToolkit.Mvvm.Messaging;
using finwise.maui.Handlers;
using finwise.maui.Helpers;
using finwise.maui.Models;
using finwise.maui.ViewModels;
namespace finwise.maui
{
    public partial class App : Application
    {
        public static DBHandler _localDB { get; private set; }

        public static List<Expense> _expenses { get; private set; }
        public static List<Person> _people { get; private set; }
        public static List<Group> _groups { get; private set; }

        public static BaseViewModel _bvm { get; private set; }

        public App(BaseViewModel bvm)
        {
            //_localDB = localDB;

            _bvm = bvm;

            Task.Run(async () =>
            {
                _expenses = await MyStorage.LoadFromDataFile<Expense>();
                _people = await MyStorage.LoadFromDataFile<Person>();
                _groups = await MyStorage.LoadFromDataFile<Group>();
            });

            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnSleep()
        {
            Task.Run(async () =>
            {
                await MyStorage.WriteToDataFile<Expense>(_bvm.Expenses.ToList());
                await MyStorage.WriteToDataFile<Person>(_bvm.People.ToList());
                await MyStorage.WriteToDataFile<Group>(_bvm.Groups.ToList());
                //await MyStorage.WriteToDataFile<Expense>(_expenses);
                //await MyStorage.WriteToDataFile<Person>(_people);
                //await MyStorage.WriteToDataFile<Group>(_groups);
            });
        }

        protected override void OnResume()
        {
            Task.Run(async () =>
            {
                _expenses = await MyStorage.LoadFromDataFile<Expense>();
                _people = await MyStorage.LoadFromDataFile<Person>();
                _groups = await MyStorage.LoadFromDataFile<Group>();
            });
        }
    }
}
