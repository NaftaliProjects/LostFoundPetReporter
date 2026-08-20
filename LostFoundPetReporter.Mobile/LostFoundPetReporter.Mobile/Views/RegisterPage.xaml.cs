using LostFoundPetReporter.Mobile.ViewModels;

namespace LostFoundPetReporter.Mobile.Views;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;

    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    private async void OnRegisterClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            _viewModel.ErrorMessage = string.Empty;

            var createdUser =
                await _viewModel.RegisterAsync(
                    _viewModel.User);

            // Registration succeeded
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            _viewModel.ErrorMessage = ex.Message;
        }
    }
}