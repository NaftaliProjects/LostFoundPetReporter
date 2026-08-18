using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class CreateUserRequest
    {
        public int? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
    }
}
