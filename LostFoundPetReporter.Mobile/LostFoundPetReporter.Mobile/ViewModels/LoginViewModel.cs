using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Session;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LostFoundPetReporter.Mobile.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IUserApiService _userApiService;
    private readonly IUserSession _userSession;

    public LoginUser loginUser { get; set; } = new LoginUser();




    public LoginViewModel(
        IUserApiService userApiService,
        IUserSession userSession)
    {
        _userApiService = userApiService;
        _userSession = userSession;
    }

    public async Task LoginAsync()
    {
        
        var user = await _userApiService.LoginAsync(loginUser);

        if (user == null) { <what to do  ?>}

        _userSession.SetUser(user);
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