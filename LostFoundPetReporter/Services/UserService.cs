using LostFoundPetReporter.API.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Data.SqlClient;

namespace LostFoundPetReporter.Services.UserService
{
    public enum UserError { None, EmailTaken, NotFound, InvalidCredentials }

    public record UserResponse(string UserName, string? Name, string? LastName, string? Email, string? Phone);
    public record UserRequest(string UserName, string? Name, string? LastName, string? Email, string? Phone, string? Password);
    public record UserLoginRequest(string UserName, string Password);
    public record ServiceResult<T>(T? Value, UserError Error = UserError.None)
    {
        public bool Succeeded => Error == UserError.None;
    }

    public interface IUserService
    {
        Task<ServiceResult<UserResponse>> RegisterAsync(UserRequest req, CancellationToken ct);
        Task<ServiceResult<string>> LoginAsync(UserLoginRequest req, CancellationToken ct);
        Task<ServiceResult<UserResponse>> GetCurrentUserAsync(string username, CancellationToken ct);
        Task<ServiceResult<UserResponse>> UpdateAsync(string username, UserRequest req, CancellationToken ct);
    }

    public class UserService : IUserService
    {
        private readonly PetReporterContext _db;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher<User> _hasher;

        public UserService(PetReporterContext db, IJwtService jwtService, IPasswordHasher<User> hasher)
        {
            _db = db; _jwtService = jwtService; _hasher = hasher;
        }



        public async Task<ServiceResult<UserResponse>> RegisterAsync(UserRequest req, CancellationToken ct)
        {

            if (await _db.Users.AnyAsync(u => req.Email != null && u.Email == req.Email, ct))
            {
                return new(null, UserError.EmailTaken);
            }

            var user = new User { UserName = req.UserName, Name = req.Name, LastName = req.LastName, HashedPassword = "", Email = req.Email, Phone = req.Phone };
            user.HashedPassword = _hasher.HashPassword(user, req.Password!);

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
            return new(new UserResponse(user.UserName, user.Name, user.LastName, user.Email, user.Phone));

            
        }

        public async Task<ServiceResult<string>> LoginAsync(UserLoginRequest req, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == req.UserName, ct);

            if (user is null)
                return new(null, UserError.NotFound);

            var verificationResults = _hasher.VerifyHashedPassword(user, user.HashedPassword, req.Password);

            if (verificationResults == PasswordVerificationResult.Failed)
                return new(null, UserError.InvalidCredentials);

            return new(_jwtService.CreateToken(user.UserName, out var expiresAt));  
        }

        public async Task<ServiceResult<UserResponse>> GetCurrentUserAsync(string username, CancellationToken ct) { 
      
            User? currentUser = await _db.Users.AsNoTracking()
                                .Where(u => u.UserName == username)
                                .FirstOrDefaultAsync(ct);

            if (currentUser is null)
                return new(null, UserError.NotFound);

            return new(new UserResponse(currentUser.UserName, currentUser.Name, currentUser.LastName, currentUser.Email, currentUser.Phone), UserError.None);
        }

        public async Task<ServiceResult<UserResponse>> UpdateAsync(string username, UserRequest req, CancellationToken ct)
        {

            if (await _db.Users.AnyAsync(u => req.Email != null && u.Email == req.Email, ct))
            {
                return new(null, UserError.EmailTaken);
            }


            var rows = await _db.Users.Where(u => u.UserName == username)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.Name, u => !string.IsNullOrEmpty(req.Name) ? req.Name : u.Name)
                    .SetProperty(u => u.Email, u => !string.IsNullOrEmpty(req.Email) ? req.Email : u.Email)
                    .SetProperty(u => u.Phone, u => !string.IsNullOrEmpty(req.Phone) ? req.Phone : u.Phone), ct);

            return rows == 0
                ? new(null, UserError.NotFound)
                : new(new UserResponse(req.UserName, req.Name, req.LastName,req.Email, req.Phone));

        }


    }
}
