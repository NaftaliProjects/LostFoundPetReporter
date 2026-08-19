using System;
using System.Collections.Generic;
using System.Text;
using LostFoundPetReporter.Mobile.Models;

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

        public async Task<User?> CreateUserAsync(
            CreateLostReportRequest request)
        {
            return await _apiClient.PostAsync<CreateLostReportRequest, User>(
                "api/v1/User",
                request);
        }

        public async Task UpdateUserAsync(
            int id,
            CreateLostReportRequest request)
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
    }
}
