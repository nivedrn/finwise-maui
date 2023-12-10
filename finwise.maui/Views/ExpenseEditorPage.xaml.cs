using CommunityToolkit.Maui.Core.Platform;
using finwise.maui.ViewModels;
using finwise.maui.Models;

namespace finwise.maui.Views;

public partial class ExpenseEditorPage : ContentPage
{
	ExpenseEditorViewModel expenseEditorViewModel;

    public ExpenseEditorPage(Expense expense)
	{
		InitializeComponent();

        expenseEditorViewModel = new ExpenseEditorViewModel(expense);
        this.BindingContext = expenseEditorViewModel;
    }

    protected override async void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        await activityDescription.HideKeyboardAsync(CancellationToken.None);
        base.OnNavigatedFrom(args);
    }

}