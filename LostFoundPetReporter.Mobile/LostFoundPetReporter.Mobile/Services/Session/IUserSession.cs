using LostFoundPetReporter.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Session
{
    public interface IUserSession
    {
        User? CurrentUser { get; }

        void SetUser(User user);

        void Clear();
    }
}
