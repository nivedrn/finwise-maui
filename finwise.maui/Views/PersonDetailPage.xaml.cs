using finwise.maui.ViewModels;

namespace finwise.maui.Views;

public partial class PersonDetailPage : ContentPage
{
	PeoplePageViewModel peoplePageVM;

    public PersonDetailPage(PeoplePageViewModel vmReference)
    {
        this.BindingContext = peoplePageVM = vmReference;
        InitializeComponent();
    }

    void OnSwiped(object sender, SwipedEventArgs e)
    {
        //int oldIndex = expenseDetailVM.currentIndex;
        //switch (e.Direction)
        //{
        //    case SwipeDirection.Right:
        //        expenseDetailVM.currentIndex = expenseDetailVM.currentIndex > 0 ? expenseDetailVM.currentIndex - 1 : expenseDetailVM.currentIndex;
        //        break;
        //    case SwipeDirection.Left:
        //        expenseDetailVM.currentIndex = expenseDetailVM.currentIndex < expenseDetailVM.expensesCount - 1 ? expenseDetailVM.currentIndex + 1 : expenseDetailVM.currentIndex;
        //        break;
        //}

        //if (oldIndex != expenseDetailVM.currentIndex)
        //{
        //    expenseDetailVM.ExpenseItem = App._bvm.Expenses[expenseDetailVM.currentIndex];
        //}

    }

}