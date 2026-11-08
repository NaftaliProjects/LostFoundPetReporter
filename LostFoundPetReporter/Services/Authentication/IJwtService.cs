namespace LostFoundPetReporter.API.Services.Authentication
{
    public interface IJwtService
    {
        string CreateToken(int userId, string name, out DateTime expiresAt);
    }
}
