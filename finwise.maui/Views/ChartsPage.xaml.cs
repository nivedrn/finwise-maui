using finwise.maui.Helpers;
using finwise.maui.ViewModels;

namespace finwise.maui.Views;

public partial class ChartsPage : ContentPage
{
	ChartsPageViewModel chartsPageVM;

    public ChartsPage()
	{
		InitializeComponent();
		this.BindingContext = chartsPageVM = new ChartsPageViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        //chartsPageVM.LogsExpenses = await Logger.ReadLogsAsync();
    }
}