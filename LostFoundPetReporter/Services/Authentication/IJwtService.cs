namespace LostFoundPetReporter.API.Services.Authentication
{
    public interface IJwtService
    {
        string CreateToken(int userId, string email, out DateTime expiresAt);
    }
}
