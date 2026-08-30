using LostFoundPetReporter.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Session
{
    public interface IUserSession
    {
        User? CurrentUser { get; }

        string? Token { get; }

        DateTime? TokenExpiresAt { get; }

        bool IsLoggedIn { get; }

        void SetSession(
            User user,
            string token,
            DateTime expiresAt);

        void Clear();
    }
}
