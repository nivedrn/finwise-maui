using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Windows.Input;
using finwise.maui.ViewModels;
namespace finwise.maui.Views;

public partial class AddActivityPage : ContentPage
{
    public AddActivityPage()
	{
		InitializeComponent();
        this.BindingContext = new AddActivityViewModel();
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
    
}