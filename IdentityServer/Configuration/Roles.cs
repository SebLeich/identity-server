using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IdentityServer.Configuration
{
    public class Roles
    {
        public static IEnumerable<IdentityRole> Defaults()
        {
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Administrator"
                }
            };
            return roles;
        }
    }
}
