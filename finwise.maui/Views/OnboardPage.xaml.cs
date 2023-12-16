using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using finwise.maui.ViewModels;

namespace finwise.maui.Views
{
    public partial class OnboardPage : ContentPage
    {

        public OnboardPage()
        {
            InitializeComponent();
            var rememberMe = Preferences.Default.Get<bool>("RememberMe", false);

            Debug.WriteLine("Checking if You Remember");
            if (rememberMe)
            {
                //Preferences.Default.Set("RememberMe", false);

                Debug.WriteLine("It Remembers");
                
                if (DeviceInfo.Idiom == DeviceIdiom.Phone)
                {
                    Shell.Current.GoToAsync("//HomePage");
                }
                else if (DeviceInfo.Idiom != DeviceIdiom.Phone)
                {
                    Shell.Current.GoToAsync("HomePage");
                }
            }

            this.BindingContext = this;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("On Appearing: OnboardingPage");
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            Debug.WriteLine("On Navigated To: OnboardingPage");

        }

        [RelayCommand]
        private static async Task OnboardComplete()
        {
            Preferences.Default.Set<bool>("RememberMe", true);

            if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                await Shell.Current.GoToAsync("//HomePage");
            }
            else if (DeviceInfo.Idiom != DeviceIdiom.Phone)
            {
                await Shell.Current.GoToAsync("HomePage");
            }
        }

    }
}
