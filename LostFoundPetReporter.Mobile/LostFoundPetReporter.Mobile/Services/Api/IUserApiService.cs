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

        Task<User?> CreateUserAsync(CreateLostReportRequest request);

        Task UpdateUserAsync(int id, CreateLostReportRequest request);

        Task DeleteUserAsync(int id);
    }
}
