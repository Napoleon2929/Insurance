using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Shared.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Roles.SystemRoles;

namespace Identity
{
    public static class Config
    {
        public const string CommonTestPassword = "Pass123$";
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[] {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("UserApi", "User API")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            return new[]
            {
                new Client
                {
                    ClientId = "SwaggerId",
                    ClientName = "Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RedirectUris =
                    {
                        SwaggerRedirect(configuration["UserApiUrlExternal"])
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Profile,
                        "UserApi"
                    }
                }
            };
        }

        public static Dictionary<ApplicationUser, string> GetTestSystemUsersWithRoles()
        {
            var individual = new ApplicationUser
            {
                UserName = "individual@test.com",
                Email = "individual@test.com",
                PhoneNumber = "+74158586273",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                FirstName = "Individual",
                LastName = "Person"
            };

            var legal = new ApplicationUser
            {
                UserName = "legal@test.com",
                Email = "legal@test.com",
                PhoneNumber = "+74158586273",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                FirstName = "legal",
                LastName = "Person"
            };

            var admin = new ApplicationUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                PhoneNumber = "+74158586273",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                FirstName = "admin",
                LastName = "user"
            };

            var agent = new ApplicationUser
            {
                UserName = "agent@test.com",
                Email = "agent@test.com",
                PhoneNumber = "+74158586273",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                FirstName = "agent",
                LastName = "user"
            };

            return new Dictionary<ApplicationUser, string>()
            {
                {
                    admin, AdminRoleName
                },
                {
                    agent, InsuranceAgentRoleName
                },
                {
                    legal, LegalPersonRoleName
                },
                {
                    individual, IndividualPersonRoleName
                }
            };
        }


        private static string SwaggerRedirect(string url) => $"{url}/swagger/oauth2-redirect.html";
    }
}
