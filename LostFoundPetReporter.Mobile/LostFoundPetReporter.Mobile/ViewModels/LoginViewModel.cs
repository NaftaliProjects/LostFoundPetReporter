using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Session;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using LostFoundPetReporter.Mobile.Services.Notification;

namespace LostFoundPetReporter.Mobile.ViewModels;


public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IUserApiService _userApiService;
    private readonly IUserSession _userSession;
    private readonly PushNotificationService _pushNotificationService;


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
        IUserApiService userApiService, IUserSession userSession, PushNotificationService pushNotificationService)
    {
        _userApiService = userApiService;
        _userSession = userSession;
        _pushNotificationService = pushNotificationService;
    }

    public async Task<bool> LoginAsync(LoginUser loginUser)
    {
        try
        {
            ErrorMessage = string.Empty;

            var loginResponse = await _userApiService.LoginAsync(loginUser);

            if (loginResponse == null)
            {
                ErrorMessage = "Invalid email or password.";
                return false;
            }

            _userSession.SetSession(loginResponse.User, loginResponse.Token, loginResponse.ExpiresAt);

            var fcmToken = await _pushNotificationService.GetTokenAsync();

            if (!string.IsNullOrWhiteSpace(fcmToken))
            {
                await _userApiService.RegisterDeviceAsync(
                    new RegisterDeviceTokenRequest
                    {
                        Token = fcmToken,
                        Platform = "Android"
                    });
            }


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