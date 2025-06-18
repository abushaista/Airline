using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Airline.AuthServer.Web;

public static class SeedData
{
    public static async Task Initialize(
            UserManager<Airline.AuthServer.Infrastructure.Identity.IdentityApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOpenIddictApplicationManager applicationManager,
            string issuerUri)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        if (await userManager.FindByEmailAsync("admin@example.com") == null)
        {
            var adminUser = new Airline.AuthServer.Infrastructure.Identity.IdentityApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User"
            };
            var createResult = await userManager.CreateAsync(adminUser, "P@ssword123!");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        if (await userManager.FindByEmailAsync("user@example.com") == null)
        {
            var regularUser = new Airline.AuthServer.Infrastructure.Identity.IdentityApplicationUser
            {
                UserName = "user@example.com",
                Email = "user@example.com",
                EmailConfirmed = true,
                FirstName = "Regular",
                LastName = "User"
            };
            var createResult = await userManager.CreateAsync(regularUser, "P@ssword123!");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser, "User");
            }
        }

        if (await applicationManager.FindByClientIdAsync("spa-client") == null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "spa-client",
                ClientSecret = null,
                DisplayName = "SPA Client Application",
                ConsentType = ConsentTypes.Explicit,
                ClientType = ClientTypes.Public,
                RedirectUris = { new Uri("https://localhost:4200/auth-callback") },
                PostLogoutRedirectUris = { new Uri("https://localhost:4200/logout-callback") },
                Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,
                        "ept:logout",
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.GrantTypes.Password,

                        Permissions.ResponseTypes.Code,

                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                        "scp:offline_access"
                    },
                Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
            });
        }

        if (await applicationManager.FindByClientIdAsync("confidential-client") == null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "confidential-client",
                ClientSecret = "super-secret-client-secret-123",
                DisplayName = "Confidential API Client",
                ClientType = ClientTypes.Confidential,
                Permissions =
                        {
                            Permissions.Endpoints.Token,
                            Permissions.GrantTypes.ClientCredentials,
                            "scp:openid",
                            Permissions.Scopes.Profile
                        }
            });
        }

        if (await applicationManager.FindByClientIdAsync("mvc-client") == null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "mvc-client",
                ClientSecret = "mvc-client-secret-xyz",
                DisplayName = "MVC Web Application",
                ConsentType = ConsentTypes.Explicit,
                ClientType = ClientTypes.Confidential,
                RedirectUris = { new Uri("https://localhost:5002/signin-oidc") },
                PostLogoutRedirectUris = { new Uri("https://localhost:5002/signout-callback-oidc") },
                Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,
                        "ept:logout",
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,

                        Permissions.ResponseTypes.Code,

                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                        "scp:offline_access"
                    }
            });
        }
    }
}
