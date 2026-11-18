using finwise.maui.ViewModels;

namespace finwise.maui.Views;

public partial class ActivityPage : ContentPage
{
	public ActivityPage(ActivityViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
