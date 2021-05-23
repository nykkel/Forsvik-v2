using System;
using System.Collections.Generic;
using System.Text;

namespace Forsvik.Core.Model.External
{
    public class UserListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public bool IsAdmin { get; set; }
    }
}
