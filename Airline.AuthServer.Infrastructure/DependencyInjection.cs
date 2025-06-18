using Airline.AuthServer.Application.Common.Interfaces;
using Airline.AuthServer.Domain.Interfaces;
using Airline.AuthServer.Infrastructure.Identity;
using Airline.AuthServer.Infrastructure.Persistence;
using Airline.AuthServer.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Airline.AuthServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            options.UseOpenIddict();
        });

        // Configure ASP.NET Core Data Protection to store keys in the database
        services.AddDataProtection()
                .PersistKeysToDbContext<ApplicationDbContext>();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IOAuthApplicationRepository, OAuthApplicationRepository>();

        services.AddOpenIddictServer(configuration);

        return services;
    }
}
