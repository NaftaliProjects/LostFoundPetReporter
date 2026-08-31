using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class RegisterDeviceResponse
    {
        public string Message { get; set; } = "";
        public int UserId { get; set; }
        public string Token { get; set; } = "";
        public string Platform { get; set; } = "";
    }
}
