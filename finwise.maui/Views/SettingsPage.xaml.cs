using finwise.maui.ViewModels;

namespace finwise.maui.Views;

public partial class SettingsPage : ContentPage
{
	SettingsPageViewModel settingsPageVM;

    public SettingsPage()
	{
		InitializeComponent();
        settingsPageVM = new SettingsPageViewModel();   

        this.BindingContext = settingsPageVM;

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        settingsPageVM.GetAllLogs();
    }
}