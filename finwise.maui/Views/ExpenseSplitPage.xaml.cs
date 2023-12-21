using finwise.maui.Models;
using finwise.maui.ViewModels;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml.Linq;

namespace finwise.maui.Views;

public partial class ExpenseSplitPage : TabbedPage
{
    ExpenseEditorViewModel expenseEditorVM;
    public bool firstLoad;
    public int selectedNavIndex;

    public ExpenseSplitPage(ExpenseEditorViewModel viewModel, int tabIndex = 0)
	{
		InitializeComponent();
        this.BindingContext = expenseEditorVM = viewModel;

        firstLoad = true;
        selectedNavIndex = tabIndex;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var changedFocus = expenseMembersSearch.Focus();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if(selectedNavIndex != 1)
        {
            CurrentPage = Children[1];
        }
        else
        {
            CurrentPage = Children[0];
        }

        if (MainThread.IsMainThread)
            expenseEditorVM.RecalculateSplit();

        else
            MainThread.BeginInvokeOnMainThread(expenseEditorVM.RecalculateSplit);
                
        //tempExpenseShareMemberSplits.ItemsSource = expenseEditorVM.tempExpenseShares;
    }

    private void expenseMembersSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        var searchTerm = ((SearchBar)sender).Text;
        if( searchTerm != "")
        {
            expenseEditorVM.ShowSelectableMembers = true;
            selectableMembersResult.ItemsSource = expenseEditorVM.RefreshPeopleList(searchTerm);
        }
        else
        {
            expenseEditorVM.ShowSelectableMembers = false;
        }
        var changedFocus = expenseMembersSearch.Focus();
    }

    private void SelectFriendsTab_Loaded(object sender, EventArgs e)
    {
        if (firstLoad)
        {
            CurrentPage = Children[selectedNavIndex];
            firstLoad = false;
            selectableMembersResult.ItemsSource = expenseEditorVM.RefreshPeopleList(null, true);
            expenseEditorVM.ShowSelectableMembers = true;

            if(expenseEditorVM.tempExpenseShares.Count == 1)
            {
                expenseEditorVM.tempExpenseShares[0].paidAmount = expenseEditorVM.ExpenseItem.amount;
                expenseEditorVM.ValidateSplit();
            }

            var changedFocus = expenseMembersSearch.Focus();
        }
    }

    public async void QuickAddPerson(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("Add a new Friend", "Enter name:", "Continue");
        if (result != "" && result is not null)
        {
            Person person = new Person();
            person.name = result;
            person.id = Guid.NewGuid().ToString();
            person.createdDate = DateTime.Now;
            person.modifiedDate = DateTime.Now;
            person.isDeleted = false;

            App._bvm.People.Insert(0, person);
            expenseMembersSearch.Text = result;
            selectableMembersResult.ItemsSource = expenseEditorVM.RefreshPeopleList(result);
            tempExpenseSharePaidMembers.ItemsSource = expenseEditorVM.tempExpenseShares;
            tempExpenseShareMemberSplits.ItemsSource = expenseEditorVM.tempExpenseShares;
        }
    }

    private async void expenseSplitTypeButton_Clicked(object sender, EventArgs e)
    {
        //Split Shares Equally
        expenseEditorVM.forceEqualSplit = true;
        if (MainThread.IsMainThread)
            expenseEditorVM.RecalculateSplit();

        else
            MainThread.BeginInvokeOnMainThread(expenseEditorVM.RecalculateSplit);

    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        expenseEditorVM.RecalculateSplit();
    }

    private async void SaveExpenseSplit_Clicked(object sender, EventArgs e)
    {
        if (expenseEditorVM.ValidateSplit())
        {
            expenseEditorVM.SaveExpenseSplit();
        }
        else
        {
            string message = expenseEditorVM.ExpenseSplitValidationMessage;
            if (expenseEditorVM.ExpenseItem.description == "" || expenseEditorVM.ExpenseItem.description is null) message += "You must enter a title\n";
            if (expenseEditorVM.ExpenseItem.amount == 0)
            {
                message += "You must enter an amount\n";
            }
            else if (expenseEditorVM.ExpenseItem.amount < 0)
            {
                message += "You must enter an amount greater than 0.\n";
            }

            await DisplayAlert("Cannot save the split details.", message, "OK");

            if (expenseEditorVM.ExpenseSplitValidationMessage.Contains("paid by"))
            {
                CurrentPage = Children[1];
            }
            else
            {
                CurrentPage = Children[2];
            }


        }
    }
}