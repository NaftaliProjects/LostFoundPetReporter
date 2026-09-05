

using LostFoundPetReporter.Mobile.ViewModels;

namespace LostFoundPetReporter.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        var email = _viewModel.LoginUser.Email;
        var password = _viewModel.LoginUser.Password;

        var success = await _viewModel.LoginAsync(_viewModel.LoginUser);

        if (!success)
            return;

        await Shell.Current.GoToAsync("//home");
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//register");
    }
}

