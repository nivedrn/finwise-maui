using finwise.maui.Models;
using finwise.maui.ViewModels;

namespace finwise.maui.Views;

public partial class ExpenseDetailPage : ContentPage
{
    ExpenseDetailViewModel expenseDetailVM;

    public ExpenseDetailPage(Expense expense)
    {
        InitializeComponent();
        expenseDetailVM = new ExpenseDetailViewModel(expense);
        this.BindingContext = expenseDetailVM;
    }

    void OnSwiped(object sender, SwipedEventArgs e)
    {
        int oldIndex = expenseDetailVM.currentIndex;
        switch (e.Direction)
        {
            case SwipeDirection.Right:
                expenseDetailVM.currentIndex = expenseDetailVM.currentIndex > 0 ? expenseDetailVM.currentIndex-1 : expenseDetailVM.currentIndex;
                break;
            case SwipeDirection.Left:
                expenseDetailVM.currentIndex = expenseDetailVM.currentIndex < expenseDetailVM.expensesCount-1 ? expenseDetailVM.currentIndex+1 : expenseDetailVM.currentIndex;
                break;
        }

        if(oldIndex != expenseDetailVM.currentIndex)
        {
            expenseDetailVM.ExpenseItem = App._bvm.Expenses[expenseDetailVM.currentIndex];
        }

    }

}