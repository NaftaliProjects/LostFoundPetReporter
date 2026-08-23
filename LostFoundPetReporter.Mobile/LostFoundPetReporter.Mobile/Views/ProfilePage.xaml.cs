using LostFoundPetReporter.Mobile.ViewModels;

namespace LostFoundPetReporter.Mobile.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    private async void OnReturnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}