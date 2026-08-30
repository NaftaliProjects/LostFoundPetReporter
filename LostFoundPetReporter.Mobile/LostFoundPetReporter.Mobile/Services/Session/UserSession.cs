using LostFoundPetReporter.Mobile.Models;

namespace LostFoundPetReporter.Mobile.Services.Session
{
    public class UserSession : IUserSession
    {
        public User? CurrentUser { get; private set; }

        public string? Token { get; private set; }

        public DateTime? TokenExpiresAt { get; private set; }

        public bool IsLoggedIn =>
            CurrentUser != null &&
            !string.IsNullOrWhiteSpace(Token);

        public void SetSession(
            User user,
            string token,
            DateTime expiresAt)
        {
            CurrentUser = user;
            Token = token;
            TokenExpiresAt = expiresAt;
        }

        public void Clear()
        {
            CurrentUser = null;
            Token = null;
            TokenExpiresAt = null;
        }
    }
}