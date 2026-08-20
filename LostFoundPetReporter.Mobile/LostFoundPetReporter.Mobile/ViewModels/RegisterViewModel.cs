

using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace LostFoundPetReporter.Mobile.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly IUserApiService _userApiService;
        

        public CreateUserRequest User { get; set; } = new();

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

        public event PropertyChangedEventHandler? PropertyChanged;


        public RegisterViewModel(IUserApiService userApiService)
        {
            _userApiService = userApiService;
            
        }

        public async Task<User> RegisterAsync(CreateUserRequest user)
        {
            var CreatedUser = await _userApiService.CreateUserAsync(user);

      

            return CreatedUser;
        }

        

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }

    }
}
