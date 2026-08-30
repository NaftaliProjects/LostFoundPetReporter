using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = "";

        public DateTime ExpiresAt { get; set; }

        public User User { get; set; } = new();
    }
}
