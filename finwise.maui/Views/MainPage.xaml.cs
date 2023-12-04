using System.Diagnostics;
using finwise.maui.ViewModels;

namespace finwise.maui.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel mainPageViewModel;

        public MainPage()
        {
            InitializeComponent();
            mainPageViewModel = new MainPageViewModel();
            this.BindingContext = mainPageViewModel;

            if (mainPageViewModel.RememberMe)
            {
                Shell.Current.GoToAsync("//HomePage");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("On Appearing");
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            Debug.WriteLine("On Navigated To");
        }
    }
}
