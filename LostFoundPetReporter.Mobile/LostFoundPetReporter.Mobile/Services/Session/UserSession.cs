using LostFoundPetReporter.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Session
{
    public class UserSession : IUserSession
    {
        public User? CurrentUser { get; private set; }

        public void SetUser(User user)
        {
            CurrentUser = user;
        }

        public void Clear()
        {
            CurrentUser = null;
        }
    }
}
