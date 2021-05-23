using System;
using System.Collections.Generic;
using System.Text;

namespace Forsvik.Core.Model.External
{
    public class LoginModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}

