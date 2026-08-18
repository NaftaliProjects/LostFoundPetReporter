using System.Collections.ObjectModel;
using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;

namespace LostFoundPetReporter.Mobile.ViewModels
{
    public class UsersViewModel
    {
        private readonly IUserApiService _userApiService;

        public ObservableCollection<User> Users { get; } = new();

        public UsersViewModel(IUserApiService userApiService)
        {
            _userApiService = userApiService;
        }

        public async Task LoadUsersAsync()
        {
            var users = await _userApiService.GetUsersAsync();

            Users.Clear();

            if (users == null)
                return;

            foreach (var user in users)
            {
                Users.Add(user);
            }
        }
    }
}

