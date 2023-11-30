using finwise.maui.ViewModels;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Behaviors;
using System.Diagnostics;
using CommunityToolkit.Maui.Core.Platform;

namespace finwise.maui.Views;

public partial class SearchPage : ContentPage
{
	public SearchPage()
	{
		InitializeComponent();
        
        this.BindingContext = new SearchViewModel();
    }

    void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        string oldText = e.OldTextValue;
        string newText = e.NewTextValue;
        string myText = searchInput.Text;
    }

    void OnEntryCompleted(object sender, EventArgs e)
    {
        string text = ((Entry)sender).Text;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Debug.WriteLine("On Navigated To: Search Page");
        var changedFocus = searchInput.Focus();
        Debug.WriteLine(changedFocus);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        var changedFocus = searchInput.HideKeyboardAsync(CancellationToken.None);
        base.OnNavigatedFrom(args);
    }

}