using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LostFoundPetReporter.Mobile.Models
{
    public class LoginUser
    {
        private string _email = string.Empty;
        private string _password = string.Empty;

        public string Email
        {
            get => _email;
            set
            {
                if (_email == value)
                    return;

                _email = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password == value)
                    return;

                _password = value;
                OnPropertyChanged();
            }
        }

    }
}
