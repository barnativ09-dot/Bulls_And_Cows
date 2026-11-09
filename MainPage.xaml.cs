using BullsAndCows.ViewModels;

namespace BullsAndCows.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // No dependency injection: the page creates its own ViewModel.
        BindingContext = new GameViewModel();
    }
}
