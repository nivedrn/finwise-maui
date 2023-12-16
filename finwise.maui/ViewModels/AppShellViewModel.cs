using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.ViewModels
{
    public partial class AppShellViewModel : BaseViewModel
    {
        [ObservableProperty]
        Dictionary<int, string> iconDictionary;

        public AppShellViewModel()
        {
            IconDictionary = new Dictionary<int, string> { 
                { 0,"receipts" },
                { 1,"chart" },
                { 2,"plus" },
                { 3,"users" },
                { 4,"user_profile" },
            };
        }
    }
}
