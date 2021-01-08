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
                    PostLogoutRedirectUris = { "https://localhost:44303/" },
                    FrontChannelLogoutUri = "https://localhost:44303/signout-oidc",

                    AllowedScopes = Resources
                        .IdentityResources
                        .Select(x => x.Name)
                        .ToList(),

                    ClientClaimsPrefix = "",
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true
                }, new Client
                {
                    ClientId = "TicketMVC",

                    Enabled = true,
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    AllowOfflineAccess = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    RedirectUris = { "https://localhost:40000/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:40000/" },
                    FrontChannelLogoutUri = "https://localhost:40000/signout-oidc",

                    AllowedScopes = Resources
                        .IdentityResources
                        .Select(x => x.Name)
                        .ToList(),

                    ClientClaimsPrefix = "",
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true
                }
            };

            return clients;
        }
    }
}
