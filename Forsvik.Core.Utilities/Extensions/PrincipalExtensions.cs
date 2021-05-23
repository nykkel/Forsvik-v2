using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Forsvik.Core.Utilities.Extensions
{
    public static class PrincipalExtensions
    {
        public static bool IsAdmin(this IPrincipal user)
        {
            return user.Identity.Name.EndsWith(":a");
        }
    }
}
