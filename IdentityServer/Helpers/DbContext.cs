using IdentityServer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityServer.Helpers
{
    public class DbContext : IdentityDbContext<User>
    {
        public DbContext() : base()
        {

        }
    }
}
