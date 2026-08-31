using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class RegisterDeviceTokenRequest
    {
        public string Token { get; set; } = "";
        public string Platform { get; set; } = "";
    }
}
