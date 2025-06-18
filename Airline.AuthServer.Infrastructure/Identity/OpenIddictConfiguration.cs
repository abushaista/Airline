using Airline.AuthServer.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Abstractions.OpenIddictConstants.Permissions;

namespace Airline.AuthServer.Infrastructure.Identity;
public static class OpenIddictConfiguration
{
    public static IServiceCollection AddOpenIddictServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<IdentityApplicationUser, IdentityRole>(options =>
        {
            options.ClaimsIdentity.UserNameClaimType = Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = Claims.Role;

            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<ApplicationDbContext>();
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("/connect/authorize")
                           .SetTokenEndpointUris("/connect/token")
                           .SetEndSessionEndpointUris("/connect/logout")
                           .SetUserInfoEndpointUris("/connect/userinfo");

                options.RegisterScopes(
                    "email", "profile", "roles", "offline_access"
                );

                options.AllowClientCredentialsFlow();
                options.AllowAuthorizationCodeFlow();
                options.AllowRefreshTokenFlow();
                options.AllowPasswordFlow();

                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableEndSessionEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableUserInfoEndpointPassthrough();

                options.RequireProofKeyForCodeExchange();

                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
                options.SetRefreshTokenLifetime(TimeSpan.FromDays(7));
                options.SetIdentityTokenLifetime(TimeSpan.FromMinutes(5));
            }).AddValidation(options =>
            {
                options.SetIssuer(configuration["OpenIddict:IssuerUri"]!);
                options.UseAspNetCore();
                options.UseSystemNetHttp();
            });

        return services;
    }
}
