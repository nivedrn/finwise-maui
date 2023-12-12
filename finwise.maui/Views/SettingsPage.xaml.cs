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

        int selectedIndex = settingsPageVM.budgetStartDateOptions.IndexOf(int.Parse(settingsPageVM.Settings["budgetStartDay"]));
        budgetStartDatePicker.SelectedIndex = selectedIndex != -1 ? selectedIndex : 0;

        selectedIndex = settingsPageVM.currencyOptions.IndexOf($"{settingsPageVM.Settings["currentCurrencyCode"]} - {settingsPageVM.Settings["currentCurrencySymbol"]}");
        budgetCurrencyPicker.SelectedIndex = selectedIndex != -1 ? selectedIndex : 0;
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

        //bool answer = await DisplayAlert("Are you Sure?", "Changing the default currency will update the currency code on all expenses.", "Confirm", "Cancel");

        if (selectedIndex != -1 && true)
        {
            budgetCurrencyPicker.SelectedIndex = selectedIndex;
            settingsPageVM.Settings["currentCurrencyCode"] = settingsPageVM.currencyDataMap[settingsPageVM.currencyOptions[selectedIndex]].currencyCode;
            settingsPageVM.Settings["currentCurrencySymbol"] = settingsPageVM.currencyDataMap[settingsPageVM.currencyOptions[selectedIndex]].currencySymbol;
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

        int selectedIndex = settingsPageVM.budgetStartDateOptions.IndexOf(int.Parse(settingsPageVM.Settings["budgetStartDay"]));
        budgetStartDatePicker.SelectedIndex = selectedIndex != -1 ? selectedIndex : 0;

        selectedIndex = settingsPageVM.currencyOptions.IndexOf($"{settingsPageVM.Settings["currentCurrencyCode"]} - {settingsPageVM.Settings["currentCurrencySymbol"]}");
        budgetCurrencyPicker.SelectedIndex = selectedIndex != -1 ? selectedIndex : 0;
    }


    private void budgetStartDatePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            budgetStartDatePicker.SelectedIndex = selectedIndex;
            settingsPageVM.Settings["budgetStartDay"] = settingsPageVM.budgetStartDateOptions[selectedIndex].ToString();
        }
    }
}