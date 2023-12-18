using CommunityToolkit.Mvvm.ComponentModel;
using finwise.maui.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.ViewModels
{
    public partial class ChartsPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        string logsExpenses;

        public ChartsPageViewModel()
        {
            LogsExpenses = "";
        }

    }
}
