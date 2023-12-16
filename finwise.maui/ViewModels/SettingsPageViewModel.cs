using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using finwise.maui.Helpers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.ViewModels
{
    public partial class SettingsPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        bool isEditMode;

        [ObservableProperty]
        Dictionary<string, string> settings;

        public Dictionary<string, string> settingsBackup {  get; set; }

        [ObservableProperty]
        string monthlyBudgetLabel;

        public List<int> budgetStartDateOptions { get; set; }
        public List<string> currencyOptions { get; set; }
        public Dictionary<string,CurrencyData> currencyDataMap { get; set; }

        public SettingsPageViewModel()
        {
            this.Title = "Settings";
            this.IsEditMode = false;
            this.Settings = new Dictionary<string, string>(App._settings);
            this.settingsBackup = new Dictionary<string, string>(App._settings);
            this.MonthlyBudgetLabel = $"Monthly Budget ( {this.Settings["currentCurrencyCode"]} {this.Settings["currentCurrencySymbol"]} ) : ";

            budgetStartDateOptions = new List<int>();
            for (int i = 0; i < 31; i++)
            {
                budgetStartDateOptions.Add(i + 1);
            }

            currencyOptions = new List<string>();
            currencyDataMap = new Dictionary<string, CurrencyData>();
            foreach (CultureInfo ci in new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.AllCultures)))
            {
                if(ci.Name != "" && ci.CultureTypes.HasFlag(CultureTypes.SpecificCultures))
                {
                    var currencyData = new CurrencyData(ci);
                    if (!currencyDataMap.ContainsKey(currencyData.pickerLabel))
                    {
                        currencyDataMap[currencyData.pickerLabel] = currencyData;
                        currencyOptions.Add(currencyData.pickerLabel);
                    }
                }
            }

            currencyOptions.Sort();
        }

        [RelayCommand]
        public void SaveEditedSettings()
        {
            this.settingsBackup = new Dictionary<string, string>(this.Settings);
            MyStorage.SaveAppSettings(this.Settings);
            App._settings = this.Settings;
            this.IsEditMode = false;
        }

        public class CurrencyData
        {
            public string countryName { get; set; }
            public string currencyCode { get; set; }
            public string currencySymbol { get; set; }
            public string pickerLabel { get; set; }

            public CurrencyData(CultureInfo cultureInfo)
            {
                
                RegionInfo region = new RegionInfo(cultureInfo.Name);
                countryName = cultureInfo.DisplayName;
                currencyCode = region.ISOCurrencySymbol;
                currencySymbol = cultureInfo.NumberFormat.CurrencySymbol;

                pickerLabel = $"{currencyCode} - {currencySymbol}";
            }
        }
    }
}
