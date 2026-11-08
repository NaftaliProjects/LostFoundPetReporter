using LostFoundPetReporter.API.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Data.SqlClient;

namespace LostFoundPetReporter.Services.UserServie
{
    public enum UserError { None, EmailTaken, NotFound, InvalidCredentials }

    public record UserResponse(int Id, string Name, string? Email, string? Phone);
    public record UpdateUserRequest(int Id, string? Name, string? Email, string? Phone,string? Password);
    public record RegisterUserRequest(int Id, string Name, string? Email, string? Phone, string Password);
    public record ServiceResult<T>(T? Value, UserError Error = UserError.None)
    {
        public bool Succeeded => Error == UserError.None;
    }

    public interface IUserService
    {
        Task<ServiceResult<UserResponse>> RegisterAsync(RegisterUserRequest req, CancellationToken ct);
        Task<ServiceResult<string>> LoginAsync(LoginRequest req, CancellationToken ct);
        Task<UserResponse?> GetAsync(int id, CancellationToken ct);
        Task<ServiceResult<UserResponse>> UpdateAsync(int id, UpdateUserRequest req, CancellationToken ct);
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

        public async Task<ServiceResult<UserResponse>> RegisterAsync(RegisterUserRequest req, CancellationToken ct)
        {
            var user = new User { Id = 0, Name = req.Name, HashedPassword = "", Email = req.Email, Phone = req.Phone };
            user.HashedPassword = _hasher.HashPassword(user, req.Password);

            _db.Users.Add(user);
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return new(null, UserError.EmailTaken);
            }
            return new(new UserResponse(user.Id, user.Name, user.Email, user.Phone));
        }

        public async Task<ServiceResult<string>> LoginAsync(LoginRequest req, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email, ct);
            if (user is null ||
                _hasher.VerifyHashedPassword(user, user.HashedPassword, req.Password) == PasswordVerificationResult.Failed)
                return new(null, UserError.InvalidCredentials);

            return new(_jwtService.CreateToken(user.Id, user.Name, out var expiresAt));
        }

        public Task<UserResponse?> GetAsync(int id, CancellationToken ct) =>
            _db.Users.AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserResponse(u.Id, u.Name, u.Email, u.Phone))
                .FirstOrDefaultAsync(ct);

        public async Task<ServiceResult<UserResponse>> UpdateAsync(int id, UpdateUserRequest req, CancellationToken ct)
        {
            try
            {
                var rows = await _db.Users.Where(u => u.Id == id)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.Name, req.Name)
                        .SetProperty(u => u.Email, req.Email)
                        .SetProperty(u => u.Phone, req.Phone), ct);

                return rows == 0
                    ? new(null, UserError.NotFound)
                    : new(new UserResponse(id, req.Name, req.Email, req.Phone));
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return new(null, UserError.EmailTaken);
            }
        }

        private static bool IsUniqueViolation(DbUpdateException ex) =>
            ex.InnerException is SqlException { Number: 2601 or 2627 };
    }
}
