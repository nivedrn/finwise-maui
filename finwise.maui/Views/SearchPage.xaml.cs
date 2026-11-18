using CommunityToolkit.Maui.Core.Platform;
using finwise.maui.ViewModels;

namespace finwise.maui.Views;

public partial class SearchPage : ContentPage
{
	public SearchPage(SearchViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        searchInput.Focus();
    }

    protected override async void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        try
        {
            await searchInput.HideKeyboardAsync(CancellationToken.None);
        }
        catch (Exception)
        {
            // The page is closing; failing to hide the keyboard is harmless.
        }
    }
}
