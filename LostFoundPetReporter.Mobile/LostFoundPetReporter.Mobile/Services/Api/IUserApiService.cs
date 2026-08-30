using System;
using System.Collections.Generic;
using System.Text;
using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Session;


namespace LostFoundPetReporter.Mobile.Services.Api
{
    public interface IUserApiService
    {
        Task<IEnumerable<User>?> GetUsersAsync();

        Task<User?> GetUserAsync(int id);

        Task<User?> CreateUserAsync(CreateUserRequest request);

        Task<LoginResponse?> LoginAsync(LoginUser user);

        Task UpdateUserAsync(int id, UpdateUserRequest request);

        Task DeleteUserAsync(int id);
    }
}
