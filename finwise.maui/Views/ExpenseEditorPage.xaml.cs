using CommunityToolkit.Maui.Core.Platform;
using finwise.maui.ViewModels;
using finwise.maui.Models;
using System.Diagnostics;

namespace finwise.maui.Views;

public partial class ExpenseEditorPage : ContentPage
{
    ExpenseEditorViewModel expenseEditorVM;

    public ExpenseEditorPage()
    {
        InitializeComponent();

        expenseEditorVM = new ExpenseEditorViewModel(null);
        this.BindingContext = expenseEditorVM;

    }

    public ExpenseEditorPage(Expense expense)
	{
		InitializeComponent();

        expenseEditorVM = new ExpenseEditorViewModel(expense);
        this.BindingContext = expenseEditorVM;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Debug.WriteLine("On Navigated To: ExpenseEditorPage");
        var changedFocus = expenseDescription.Focus();
        Debug.WriteLine(changedFocus);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        int selectedIndex = repeatExpensePicker.Items.IndexOf($"{expenseEditorVM.ExpenseItem.repeatExpense}");
        repeatExpensePicker.SelectedIndex = selectedIndex != -1 ? selectedIndex : 0;

        selectedIndex = expenseCategoryPicker.Items.IndexOf($"{expenseEditorVM.ExpenseItem.category}");
        expenseCategoryPicker.SelectedIndex = selectedIndex != -1 ? selectedIndex : 0;
    }

    protected override async void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        await expenseDescription.HideKeyboardAsync(CancellationToken.None);
        base.OnNavigatedFrom(args);
    }

    private async void SplitExpenseButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushModalAsync(new ExpenseSplitPage(expenseEditorVM, 0), false);
    }

    private async void ExpensePaidByButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushModalAsync(new ExpenseSplitPage(expenseEditorVM, 1), false);
    }

    private async void ExpenseSharesButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushModalAsync(new ExpenseSplitPage(expenseEditorVM, 2), false);
    }

    private void repeatExpensePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1 && true)
        {
            repeatExpensePicker.SelectedIndex = selectedIndex;
            expenseEditorVM.ExpenseItem.repeatExpense = picker.Items[selectedIndex];  
        }
        else
        {
            repeatExpensePicker.SelectedIndex = 0;
        }
    }

    private void expenseCategoryPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1 && true)
        {
            expenseCategoryPicker.SelectedIndex = selectedIndex;
            expenseEditorVM.ExpenseItem.category = picker.Items[selectedIndex];
        }
        else
        {
            expenseCategoryPicker.SelectedIndex = 0;
        }
    }

    private async void SaveExpenseButton_Clicked(object sender, EventArgs e)
    {
        if (expenseEditorVM.ExpenseItem.description != "" && expenseEditorVM.ExpenseItem.amount > 0)
        {
            expenseEditorVM.SaveExpense();
        }
        else
        {
            string message = "";
            if (expenseEditorVM.ExpenseItem.description == "" || expenseEditorVM.ExpenseItem.description is null) message += "You must enter a title\n";
            if (expenseEditorVM.ExpenseItem.amount == 0)
            {
                message += "You must enter an amount\n";
            }
            else if (expenseEditorVM.ExpenseItem.amount < 0)
            {
                message += "You must enter an amount greater than 0.\n";
            }

            await DisplayAlert("Cannot save Expense.", message, "OK");        
        }
    }

}