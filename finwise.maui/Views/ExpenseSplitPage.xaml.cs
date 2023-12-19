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

    public ExpenseSplitPage(ExpenseEditorViewModel viewModel)
	{
		InitializeComponent();
        this.BindingContext = expenseEditorVM = viewModel;
        Debug.WriteLine(expenseEditorVM.GetHashCode());
        //selectableMembersResult.ItemsSource = expenseEditorVM.RefreshPeopleList("");
        firstLoad = true;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var changedFocus = expenseMembersSearch.Focus();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CurrentPage = Children[1];

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
            CurrentPage = Children[0];
            firstLoad = false;
            expenseEditorVM.ShowSelectableMembers = false;
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
        string action = await DisplayActionSheet("Choose how to split the expense:", "Cancel", null, "Equally", "Unequally");
        if(action is not null & action != "Cancel")
        {
            expenseEditorVM.ExpenseItem.shareType = action;
            if (MainThread.IsMainThread)
                expenseEditorVM.RecalculateSplit();

            else
                MainThread.BeginInvokeOnMainThread(expenseEditorVM.RecalculateSplit);
        }
        
        Debug.WriteLine("Action: " + action);

    }
}