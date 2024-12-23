using IdentityServer4.Models;
using System.Collections.Generic;

namespace Swap.AuthServer.IdentityServerConfiguration
{
    public class InMemoryConfiguration
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[]
            {
                new ApiResource("Swap", "Swap")
            };
        }

        public static IEnumerable<Client> Clients()
        {
            int secondsInYear = 31536000;

            return new[]
            {
                new Client {
                    ClientId = "Swap",
                    ClientSecrets = new [] {new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] {"Swap", "offline_access"},
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = secondsInYear,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    UpdateAccessTokenClaimsOnRefresh = true
                }
            };
        }
    }
}
