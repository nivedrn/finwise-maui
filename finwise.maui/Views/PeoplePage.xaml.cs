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
        //peopleCollectionView.SelectedItem = null;
        //peopleCollectionView.ItemsSource = peoplePageVM.peopleCollection;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        peoplePageVM.Init();
        Debug.WriteLine("On Appearing: PeoplePage");
        peopleCollectionView.SelectedItem = null;
        peoplePageVM.RefreshPeopleList();
        peoplePageVM.InitUpdateOverallTallyAmount();
    }

    public async void AddPerson(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("Add a new Friend", "Enter name:", "Continue");
        if (result != "" && result is not null)
        {
            peoplePageVM.AddNewPerson(result);
            //peoplePageVM.RefreshPeopleList();
            //await Shell.Current.Navigation.PushModalAsync(new PersonDetailPage(peoplePageVM), true);
        }
    }
    
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Debug.WriteLine("On Navigated To: PeoplePage");
    }

    public void OnSearchTextChanged(object sender, EventArgs e)
    {
        this.peoplePageVM.FilterParams["searchTerm"] = ((SearchBar)sender).Text;
        peoplePageVM.RefreshPeopleList();
    }

    public void ClearAllFilters(object sender, EventArgs args)
    {
        peopleCollectionView.SelectedItem = null;
        searchInput.Text = "";
        peoplePageVM.ResetFilterParams();
        peoplePageVM.RefreshPeopleList();
        //peopleCollectionView.ItemsSource = peoplePageVM.localBVM.People;
        bottomSheet?.CloseBottomSheet();
    }

    private void ShowBottomSheet(object sender, EventArgs e)
    {
        peoplePageVM.SheetTitle = "Sort & Filter";
        bottomSheet = this.ShowBottomSheet(GetSortAndFilterBottomSheetView(), true);
    }

    private View GetSortAndFilterBottomSheetView()
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

    private void ApplyFilters_Clicked(object sender, EventArgs e)
    {
        peoplePageVM.RefreshPeopleList();
        bottomSheet?.CloseBottomSheet();
    }
}