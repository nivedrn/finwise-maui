using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Windows.Input;
using finwise.maui.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace finwise.maui.Views;

public partial class AddExpensePage : ContentPage
{
    AddExpenseViewModel addActivityViewModel;
    public AddExpensePage()
	{
		InitializeComponent();
        addActivityViewModel = new AddExpenseViewModel();
        this.BindingContext = addActivityViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("On Appearing: Add Activity");
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Debug.WriteLine("On Navigated To: Add Activity");
    }

    [RelayCommand]
    private static async Task ChangeActivityType(Object obj)
    {
        Debug.WriteLine(obj);
        //await Shell.Current.GoToAsync(nameof(EditAccount));
    }
}