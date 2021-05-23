using System;
using System.Collections.Generic;
using System.Text;

namespace Forsvik.Core.Model.External
{
    public class SetAdminModel
    {
        public Guid UserId { get; set; }

        public bool IsAdmin { get; set; }
    }
}
