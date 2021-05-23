using System;

namespace Forsvik.Core.Model.Context
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public bool IsAdmin { get; set; }
    }
}
