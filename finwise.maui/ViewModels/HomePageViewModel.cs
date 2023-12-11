﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using finwise.maui.Models;
using finwise.maui.Views;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace finwise.maui.ViewModels
{
    public partial class HomePageViewModel: BaseViewModel
    {
        public BaseViewModel localBVM { get; set; }

        [ObservableProperty]
        Dictionary<string, string> filterParams;

        public HomePageViewModel() {
        
            Title = "Home";
            localBVM = App._bvm;
            filterParams = new Dictionary<string, string> { { "searchTerm", "" } };
        }

        [RelayCommand]
        public static async Task OpenExpenseDetail(Object obj)
        {
            if(obj is not null)
            {
                await Shell.Current.Navigation.PushModalAsync(new ExpenseDetailPage((Expense)obj), true);
            }
        }

        public ObservableCollection<Expense> RefreshExpenseList()
        {
            if (this.FilterParams["searchTerm"] != "")
            {
                return new ObservableCollection<Expense>(localBVM.Expenses.Where(exp => exp.description.Contains(this.FilterParams["searchTerm"], StringComparison.OrdinalIgnoreCase))?.ToList());
            }

            return new ObservableCollection<Expense>(App._bvm.Expenses); 
        }

    }
}
