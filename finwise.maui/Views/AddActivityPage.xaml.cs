using finwise.maui.ViewModels;

namespace finwise.maui.Views;

public partial class AddActivityPage : ContentPage
{
    public AddActivityPage(AddActivityViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
