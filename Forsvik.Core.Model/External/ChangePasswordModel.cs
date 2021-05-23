using System;
using System.Collections.Generic;
using System.Text;

namespace Forsvik.Core.Model.External
{
    public class ChangePasswordModel
    {
        public Guid Id { get; set; }

        public string Password { get; set; }
    }
}
