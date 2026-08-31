using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Session;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Api
{
    public class UserApiService : IUserApiService
    {
        private readonly IApiClient _apiClient;

        public UserApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<User>?> GetUsersAsync()
        {
            return await _apiClient.GetAsync<IEnumerable<User>>(
                "api/v1/User");
        }

        public async Task<User?> GetUserAsync(int id)
        {
            return await _apiClient.GetAsync<User>(
                $"api/v1/User/{id}");
        }

        public async Task<User?> CreateUserAsync(CreateUserRequest request)
        {
            return await _apiClient.PostAsync<CreateUserRequest, User>(
                "api/v1/User/Register",
                request);
        }

        public async Task UpdateUserAsync(int id , UpdateUserRequest request)
        {
            request.Id = id;

            await _apiClient.PutAsync(
                $"api/v1/User/{id}",
                request);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _apiClient.DeleteAsync(
                $"api/v1/User/{id}");
        }


        public async Task<LoginResponse?> LoginAsync(LoginUser user)
        {
            return await _apiClient.PostAsync<LoginUser, LoginResponse>(
                "api/v1/User/Login",
                user);
        }

        public async Task RegisterDeviceAsync(RegisterDeviceTokenRequest request)
        {
            await _apiClient.PostAsync<RegisterDeviceTokenRequest, RegisterDeviceResponse>("api/v1/User/RegisterDevice", request);
        }
    }
}
