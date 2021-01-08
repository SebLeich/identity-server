using IdentityServer.Entities;
using System.Collections.Generic;

namespace IdentityServer.Configuration
{
    public class Users
    {
        public static IEnumerable<User> Defaults()
        {
            var users = new List<User>
            {
                new User { UserName = "Administrator", Email = "admin@identity-domain.cpls", LockoutEnabled = false },
                new User { UserName = "Benutzer", Email = "user@identity-domain.cpls", LockoutEnabled = true }
            };

            return users;
        }

        public static string GetUserDefaultPassword(string username)
        {
            switch (username)
            {
                case "Administrator":
                    return "Cpls-2020";
                case "Benutzer":
                    return "12345678";
            }
            return null;
        }

        public static IEnumerable<string> GetUserDefaultRoles(string username)
        {
            switch (username)
            {
                case "Administrator":
                    return new List<string> { "Administrator" };
                case "Benutzer":
                    return new List<string> { "Benutzer" };
            }

            return null;
        }
    }
}
