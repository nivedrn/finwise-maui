using CommunityToolkit.Mvvm.ComponentModel;
using finwise.maui.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.ViewModels
{
    public partial class SettingsPageViewModel: BaseViewModel
    {
        [ObservableProperty]
        string logs;

        public SettingsPageViewModel()
        {
            this.Title = "Logs";          
        }

        public async Task GetAllLogs()
        {
            Logs = await Logger.ReadLogsAsync();
        }

    }
}
