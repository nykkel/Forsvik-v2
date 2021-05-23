using System;
using System.Collections.Generic;
using System.Text;

namespace Forsvik.Core.Model.External
{
    public class CurrentUserModel
    {
        public string Name { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsFirstTimeUser { get; set; }
    }
}
