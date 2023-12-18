using CommunityToolkit.Mvvm.Input;
using finwise.maui.ViewModels;
using System.Diagnostics;

namespace finwise.maui.Views;

public partial class SettingsPage : ContentPage
{
	SettingsPageViewModel settingsPageVM;

    public SettingsPage()
    {
        InitializeComponent();
        settingsPageVM = new SettingsPageViewModel();
        this.BindingContext = settingsPageVM;

        budgetCurrencyPicker.SelectedIndex = 0;
    }

    public void EditSettings(object sender, EventArgs e)
    {
        settingsPageVM.IsEditMode = true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("On Appearing: SettingsPage");
    }

    private void budgetCurrencyPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            budgetCurrencyPicker.SelectedIndex = selectedIndex;
            settingsPageVM.Settings["currentCurrencyCode"] = settingsPageVM.currencyDataMap[settingsPageVM.CurrencyOptions[selectedIndex]].currencyCode;
            settingsPageVM.Settings["currentCurrencySymbol"] = settingsPageVM.currencyDataMap[settingsPageVM.CurrencyOptions[selectedIndex]].currencySymbol;
            settingsPageVM.MonthlyBudgetLabel = $"Monthly Budget ( {settingsPageVM.Settings["currentCurrencyCode"]} {settingsPageVM.Settings["currentCurrencySymbol"]} ) : ";
        }
        else
        {
            budgetCurrencyPicker.SelectedIndex = 0;
        }

    }

    public void CancelEditSettings(object sender, EventArgs e)
    {
        settingsPageVM.IsEditMode = false;
        settingsPageVM.Settings = new Dictionary<string, string>(settingsPageVM.settingsBackup);
        settingsPageVM.MonthlyBudgetLabel = $"Monthly Budget ( {settingsPageVM.Settings["currentCurrencyCode"]} {settingsPageVM.Settings["currentCurrencySymbol"]} ) : ";

        int selectedIndex = settingsPageVM.CurrencyOptions.IndexOf($"{settingsPageVM.Settings["currentCurrencyCode"]} - {settingsPageVM.Settings["currentCurrencySymbol"]}");
        budgetCurrencyPicker.SelectedIndex = selectedIndex != -1 ? selectedIndex : 0;
    }

}