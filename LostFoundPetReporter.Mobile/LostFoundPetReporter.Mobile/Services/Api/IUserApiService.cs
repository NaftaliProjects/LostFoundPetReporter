using System;
using System.Collections.Generic;
using System.Text;
using LostFoundPetReporter.Mobile.Models;


namespace LostFoundPetReporter.Mobile.Services.Api
{
    public interface IUserApiService
    {
        Task<IEnumerable<User>?> GetUsersAsync();

        Task<User?> GetUserAsync(int id);

        Task<User?> CreateUserAsync(CreateUserRequest request);

        Task UpdateUserAsync(int id, CreateUserRequest request);

        Task DeleteUserAsync(int id);
    }
}
