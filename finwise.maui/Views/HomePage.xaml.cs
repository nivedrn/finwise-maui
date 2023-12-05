using System.Diagnostics;
using finwise.maui.ViewModels;

namespace finwise.maui.Views;

public partial class HomePage : ContentPage
{
    public HomePageViewModel homePageViewModel;

    public HomePage()
    {
        InitializeComponent();
        homePageViewModel = new HomePageViewModel();
        this.BindingContext = homePageViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("On Appearing: HomePage");
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Debug.WriteLine("On Navigated To: HomePage");
    }

    public void OnSearchTextChanged(object sender, EventArgs e)
    {
        string text = ((SearchBar)sender).Text;
    }


}