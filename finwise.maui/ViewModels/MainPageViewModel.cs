using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.ViewModels
{
    public class MainPageViewModel: BaseViewModel
    {
        public MainPageViewModel()
        {
            var rememberMe = Preferences.Default.Get<bool>("RememberMe", false);
            Debug.WriteLine("Checking if You Remember");
            if(rememberMe || true)
            {
                RememberMe = true;
                Debug.WriteLine("It Remembers");
            }
            else
            {
                Preferences.Default.Set<bool>("RememberMe", true);
            }
        }
    }
}
