namespace LostFoundPetReporter.API.Services.Authentication
{
    public interface IJwtService
    {
        string CreateToken(string username, out DateTime expiresAt);
    }
}
