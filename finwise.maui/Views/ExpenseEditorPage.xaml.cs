using CommunityToolkit.Maui.Core.Platform;
using finwise.maui.ViewModels;
using finwise.maui.Models;
using System.Diagnostics;


#if ANDROID
using BottomSheetView = Google.Android.Material.BottomSheet.BottomSheetDialog;
#elif IOS || MACCATALYST
using BottomSheetView = UIKit.UIViewController;
#elif TIZEN
using BottomSheetView = Tizen.UIExtensions.NUI.Popup;
#else
using BottomSheetView = Microsoft.UI.Xaml.Controls.Primitives.Popup;
#endif

namespace finwise.maui.Views;

public partial class ExpenseEditorPage : ContentPage
{
#nullable enable
    BottomSheetView? bottomSheet;

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

    protected override async void OnAppearing()
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

    private async void ShareExpense_Clicked(object sender, EventArgs e)
    {
        //bottomSheet = this.ShowBottomSheet(GetSharedExpenseBottomSheetView(), true);
        //await Shell.Current.GoToAsync("ExpenseSplitPage");
        await Shell.Current.Navigation.PushModalAsync(new ExpenseSplitPage(expenseEditorVM), false);
    }

    private void modifyExpenseSplit_Clicked(object sender, EventArgs e)
    {
        bottomSheet = this.ShowBottomSheet(GetExpenseSplitBottomSheetView(), true);
    }

    private View GetSharedExpenseBottomSheetView()
    {
        var view = (View)ShareExpenseSheet.CreateContent();
        view.BindingContext = this.BindingContext;
        return view;
    }

    private View GetExpenseSplitBottomSheetView()
    {
        var view = (View)ExpenseSplitChangeSheet.CreateContent();
        view.BindingContext = this.BindingContext;
        return view;
    }

    private void OnCloseClicked(object? sender, EventArgs e)
    {
        bottomSheet?.CloseBottomSheet();
    }

    public void Dispose()
    {
#if !WINDOWS
        bottomSheet?.Dispose();
#endif
    }

    private void AddExpenseTag_Clicked(object sender, EventArgs e)
    {

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

    private void ShareExpenseWithGroup_Clicked(object sender, EventArgs e)
    {

    }
}