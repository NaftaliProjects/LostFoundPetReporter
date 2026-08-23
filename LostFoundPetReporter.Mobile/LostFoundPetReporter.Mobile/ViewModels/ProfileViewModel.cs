using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Session;
using System.Windows.Input;

public class ProfileViewModel
{
    private readonly IUserApiService _userApiService;
    private readonly IUserSession _userSession;

    public User CurrentUser { get; }

    public ICommand UpdateCommand { get; }

    public ProfileViewModel(
        IUserApiService userApiService,
        IUserSession userSession)
    {
        _userApiService = userApiService;
        _userSession = userSession;

        CurrentUser = _userSession.CurrentUser
            ?? throw new InvalidOperationException("No user is logged in.");

        UpdateCommand = new Command(async () => await UpdateUserAsync());
    }

    private async Task UpdateUserAsync()
    {
        var request = new UpdateUserRequest
        {
            Id = CurrentUser.Id,
            Name = CurrentUser.Name,
            Email = CurrentUser.Email,
            Phone = CurrentUser.Phone
        };

        await _userApiService.UpdateUserAsync(
            CurrentUser.Id,
            request);

        await Shell.Current.DisplayAlert(
            "Profile",
            "Profile updated successfully.",
            "OK");
    }
}