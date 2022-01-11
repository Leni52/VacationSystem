﻿using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace WorkForceManagement.WEB
{
    public class IdentityConfig
    {
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "WorkForceManagement",
                    AllowOfflineAccess = true,

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    // scopes that client has access to
                    AllowedScopes = { "users", "offline_access", "WorkForceManagement", "roles" }
                }
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResource("roles", new[] { "role" })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                    new ApiScope("users", "My API", new string[]{ ClaimTypes.Name, ClaimTypes.Role }),
                    new ApiScope("offline_access", "RefereshToken"),
                    new ApiScope("WorkForceManagement", "app")
            };
    }
}
