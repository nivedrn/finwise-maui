using finwise.maui.ViewModels;
using System.Diagnostics;

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

public partial class PeoplePage : ContentPage
{
#nullable enable
    BottomSheetView? bottomSheet;

    PeoplePageViewModel peoplePageVM;

	public PeoplePage()
	{
		InitializeComponent();
		this.BindingContext = peoplePageVM = new PeoplePageViewModel();

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("On Appearing: PeoplePage");
        expenseCollectioView.SelectedItem = null;
        expenseCollectioView.ItemsSource = peoplePageVM.RefreshPeopleList();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Debug.WriteLine("On Navigated To: PeoplePage");
    }

    public void OnSearchTextChanged(object sender, EventArgs e)
    {
        this.peoplePageVM.FilterParams["searchTerm"] = ((SearchBar)sender).Text;
        expenseCollectioView.ItemsSource = peoplePageVM.RefreshPeopleList();
    }

    public void ClearAllFilters(object sender, EventArgs args)
    {
        expenseCollectioView.SelectedItem = null;
        searchInput.Text = "";
        expenseCollectioView.ItemsSource = peoplePageVM.localBVM.Expenses;
        bottomSheet?.CloseBottomSheet();
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