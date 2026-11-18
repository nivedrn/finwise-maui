using CommunityToolkit.Mvvm.ComponentModel;

namespace finwise.maui.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string Title { get; set; } = string.Empty;
    }
}
