using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Session;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LostFoundPetReporter.Mobile.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IUserApiService _userApiService;
    private readonly IUserSession _userSession;

    public LoginUser LoginUser { get; } = new();

    private string _errorMessage = string.Empty;

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage == value)
                return;

            _errorMessage = value;
            OnPropertyChanged();
        }
    }




    public LoginViewModel(
        IUserApiService userApiService,
        IUserSession userSession)
    {
        _userApiService = userApiService;
        _userSession = userSession;
    }

    public async Task<bool> LoginAsync(LoginUser loginUser)
    {
        try
        {
            ErrorMessage = string.Empty;

            var user = await _userApiService.LoginAsync(loginUser);

            if (user == null)
            {
                ErrorMessage = "Invalid email or password.";
                return false;
            }

            _userSession.SetUser(user);

            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}