using System.Diagnostics;
using System.Diagnostics.Contracts;
using CommunityToolkit.Mvvm.Input;
using finwise.maui.ViewModels;

#if ANDROID
using BottomSheetView = Google.Android.Material.BottomSheet.BottomSheetDialog;
#elif IOS || MACCATALYST
using BottomSheetView = UIKit.UIViewController;
#elif TIZEN
using BottomSheetView = Tizen.UIExtensions.NUI.Popup;
#else
using BottomSheetView = Microsoft.UI.Xaml.Controls.Primitives.Popup;
#endif

namespace finwise.maui.Views;

public partial class HomePage : ContentPage, IDisposable
{
#nullable enable
    BottomSheetView? bottomSheet;

    public HomePageViewModel homePageViewModel;

    public HomePage()
    {
        InitializeComponent();
        homePageViewModel = new HomePageViewModel();
        this.BindingContext = homePageViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("On Appearing: HomePage");
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Debug.WriteLine("On Navigated To: HomePage");
        expenseCollectioView.SelectedItem = null;
    }

    public void OnSearchTextChanged(object sender, EventArgs e)
    {
        string text = ((SearchBar)sender).Text;
    }

    private void ShowBottomSheet(object sender, EventArgs e)
    {
        bottomSheet = this.ShowBottomSheet(GetBottomSheetView(), true);
    }

    private View GetBottomSheetView()
    {
        var view = (View)SortAndFilterSheet.CreateContent();
        view.BindingContext = this.BindingContext;
        return view;
    }

    private void OnCloseClicked(object? sender, EventArgs e)
    {
        bottomSheet?.CloseBottomSheet();
    }

    public void Dispose()
    {
#if !WINDOWS
        bottomSheet?.Dispose();
#endif
    }

}