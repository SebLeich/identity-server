using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer.Configuration
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            var clients = new List<Client> {
                new Client
                {
                    ClientId = "AdminClient",

                    Enabled = true,
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    AllowOfflineAccess = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    RedirectUris = { "https://localhost:44303/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:44303/" },
                    FrontChannelLogoutUri = "http://localhost:44303/signout-oidc",

                    AllowedScopes = Resources
                        .IdentityResources
                        .Select(x => x.Name)
                        .ToList()
                }
            };

            return clients;
        }
    }
}
