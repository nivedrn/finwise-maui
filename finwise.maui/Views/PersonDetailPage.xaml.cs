using finwise.maui.ViewModels;

namespace finwise.maui.Views;

public partial class PersonDetailPage : ContentPage
{
    public PeoplePageViewModel peoplePageVM { get; set; }

    public PersonDetailPage(PeoplePageViewModel vmReference)
    {
        InitializeComponent();
        this.BindingContext = this.peoplePageVM = vmReference;
        relatedExpenseDebtsCollection.ItemsSource = peoplePageVM.FetchRelatedExpenses();
    }

    void OnSwiped(object sender, SwipedEventArgs e)
    {
        int oldIndex = peoplePageVM.currentIndex;
        switch (e.Direction)
        {
            case SwipeDirection.Right:
                peoplePageVM.currentIndex = peoplePageVM.currentIndex > 0 ? peoplePageVM.currentIndex - 1 : peoplePageVM.currentIndex;
                break;
            case SwipeDirection.Left:
                peoplePageVM.currentIndex = peoplePageVM.currentIndex < peoplePageVM.peopleCount - 1 ? peoplePageVM.currentIndex + 1 : peoplePageVM.currentIndex;
                break;
        }

        if (oldIndex != peoplePageVM.currentIndex)
        {
            peoplePageVM.CurrentIndexPerson = App._bvm.People[peoplePageVM.currentIndex];
        }
    }

    private async void DeleteFriend_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Are you Sure ?", "Deleting this person will disable them from being added to new expenses.", "Confirm", "Cancel");
        if (answer)
        {
            peoplePageVM.CurrentIndexPerson.isDeleted = true;
            await Shell.Current.Navigation.PopModalAsync();
        }
    }
}