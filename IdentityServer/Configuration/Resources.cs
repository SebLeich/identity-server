using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer.Configuration
{
    public class Resources
    {
        // identity resources represent identity data about a user that can be requested via the scope parameter (OpenID Connect)
        public static readonly ICollection<IdentityResource> IdentityResources =
            new[]
            {
                // some standard scopes from the OIDC spec
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),

                // custom identity resource with some consolidated claims
                new IdentityResource("custom.profile", new[] {
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Email,
                    "location",
                    JwtClaimTypes.Address
                }),

                new IdentityResource
                {
                    Name = "roles",
                    UserClaims = new List<string>{ "role" }
                }
            };

        // API scopes represent values that describe scope of access and can be requested by the scope parameter (OAuth)
        public static readonly IEnumerable<ApiScope> ApiScopes =
            new[]
            {
                // local API scope
                new ApiScope(LocalApi.ScopeName)
            };

        // API resources are more formal representation of a resource with processing rules and their scopes (if any)
        public static readonly IEnumerable<ApiResource> ApiResources =
            new[]
            {
                new ApiResource("resource1", "Resource 1")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },

                    Scopes = { "resource1.scope1", "shared.scope" }
                },

                new ApiResource("resource2", "Resource 2")
                {
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // additional claims to put into access token
                    UserClaims =
                    {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email
                    },

                    Scopes = { "resource2.scope1", "shared.scope" }
                }
            };
    }
}
