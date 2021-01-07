using IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using IdentityServer.Configuration;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Helpers
{
    public class DbInitializer
    {
        public async static Task InitializeAsync(IDSDbContext iDSDbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            iDSDbContext.Database.Migrate();

            foreach (IdentityRole role in Roles.Defaults())
            {
                if (!(await roleManager.RoleExistsAsync(role.Name)))
                {
                    Log.Logger.Information($"create role {role.Name}");
                    await roleManager.CreateAsync(role);
                }
            }

            foreach (User user in Users.Defaults())
            {
                if ((await userManager.FindByNameAsync(user.UserName)) == null)
                {
                    var password = Users.GetUserDefaultPassword(user.UserName);

                    if (password == null)
                    {
                        Log.Logger.Error($"cannot create user {user.UserName}: no default password found");
                        continue;
                    }

                    Log.Logger.Information($"create user {user.UserName}");

                    await userManager.CreateAsync(user);
                    await userManager.AddPasswordAsync(user, password);

                    var defaultRoles = Users.GetUserDefaultRoles(user.UserName);

                    if (defaultRoles == null)
                    {
                        Log.Error($"no default roles defined for the user {user.UserName}");
                        continue;
                    }

                    foreach (string role in defaultRoles)
                    {
                        if(await roleManager.RoleExistsAsync(role))
                        {
                            if (!(await userManager.IsInRoleAsync(user, role)))
                            {
                                await userManager.AddToRoleAsync(user, role);
                            }
                        } else
                        {
                            Log.Error($"cannot find a role with the name {role}");
                        }
                    }
                }
            }
        }
    }
}
