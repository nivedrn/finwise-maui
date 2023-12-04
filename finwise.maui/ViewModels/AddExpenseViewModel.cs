using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using finwise.maui.Models;
using finwise.maui.Handlers;
using CommunityToolkit.Mvvm.Input;

namespace finwise.maui.ViewModels
{
    public partial class AddExpenseViewModel : BaseViewModel
    {
        Expense activity;

        public AddExpenseViewModel()
        {
            activity = new Expense();
            activity.expenseDate = DateTime.Now;
            activity.amount = 0;
            activity.expenseType = "Expense";
        }


    }
}
