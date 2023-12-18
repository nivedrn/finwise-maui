using finwise.maui.Models;
using finwise.maui.ViewModels;

namespace finwise.maui.Views;

public partial class ExpenseSplitPage : TabbedPage
{
    ExpenseEditorViewModel expenseEditorVM;
    public bool firstLoad;

    public ExpenseSplitPage(ExpenseEditorViewModel viewModel)
	{
		InitializeComponent();
        this.BindingContext = expenseEditorVM = viewModel;
        expenseEditorVM.ShowSelectableMembers = false;
        firstLoad = true;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CurrentPage = Children[1];
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

    }

    private void selectableMembersResult_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Person selectedPerson)
        {
            expenseEditorVM.ShowSelectableMembers = false;
            expenseEditorVM.tempExpenseShares.Add(new ExpenseShare(selectedPerson.id, false));
        }
    }

    private void SelectFriendsTab_Loaded(object sender, EventArgs e)
    {
        if (firstLoad)
        {
            CurrentPage = Children[0];
            firstLoad = false;

        }
    }

}