using Airline.AuthServer.Domain.Entities;
using Airline.AuthServer.Domain.Interfaces;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.AuthServer.Infrastructure.Persistence.Repositories;
public class OAuthApplicationRepository : IOAuthApplicationRepository
{
    private readonly IOpenIddictApplicationManager _applicationManager;

    public OAuthApplicationRepository(IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    public async Task AddAsync(OAuthApplication application, CancellationToken cancellationToken = default)
    {
        var descriptor = MapToOpenIddictDescriptor(application);
        await _applicationManager.CreateAsync(descriptor, cancellationToken);
        var createdApp = await _applicationManager.FindByClientIdAsync(application.ClientId, cancellationToken);
        if (createdApp != null)
        {
            application.Id = await _applicationManager.GetIdAsync(createdApp, cancellationToken) ?? application.Id;
        }
    }

    private OpenIddictApplicationDescriptor MapToOpenIddictDescriptor(OAuthApplication application)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = application.ClientId,
            DisplayName = application.DisplayName,
            ClientSecret = application.ClientSecret,
            ClientType = application.Type,
            ConsentType = application.ConsentType
        };

        // Add RedirectUris individually
        foreach (var uri in application.RedirectUris)
        {
            descriptor.RedirectUris.Add(new Uri(uri));
        }

        // Add PostLogoutRedirectUris individually
        foreach (var uri in application.PostLogoutRedirectUris)
        {
            descriptor.PostLogoutRedirectUris.Add(new Uri(uri));
        }

        // Add Permissions individually
        foreach (var permission in application.Permissions)
        {
            descriptor.Permissions.Add(permission);
        }

        // Add Requirements individually
        foreach (var requirement in application.Requirements)
        {
            descriptor.Requirements.Add(requirement);
        }

        return descriptor;
    }

    public async Task DeleteAsync(OAuthApplication application, CancellationToken cancellationToken = default)
    {
        var openIddictApp = await _applicationManager.FindByIdAsync(application.Id, cancellationToken);
        if (openIddictApp == null)
        {
            return;
        }
        await _applicationManager.DeleteAsync(openIddictApp, cancellationToken);
    }

    public async Task<IEnumerable<OAuthApplication>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var applications = new List<OAuthApplication>();
        await foreach (var app in _applicationManager.ListAsync(cancellationToken: cancellationToken))
        {
            if (app is OpenIddictEntityFrameworkCoreApplication efCoreApp)
            {
                var domainApp = MapToDomain(efCoreApp);
                if (domainApp != null)
                {
                    applications.Add(domainApp);
                }
            }
        }
        return applications;
    }

    public async Task<OAuthApplication?> GetByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        var openIddictApp = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken) as OpenIddictEntityFrameworkCoreApplication;
        return MapToDomain(openIddictApp);
    }

    private OAuthApplication? MapToDomain(OpenIddictEntityFrameworkCoreApplication? openIddictApp)
    {
        if (openIddictApp == null) return null;

        var domainApplication = new OAuthApplication
        {
            Id = openIddictApp.Id?.ToString() ?? Guid.NewGuid().ToString(),
            ClientId = openIddictApp.ClientId ?? string.Empty,
            DisplayName = openIddictApp.DisplayName ?? string.Empty,
            ClientSecret = openIddictApp.ClientSecret,
            Type = openIddictApp.ClientType ?? string.Empty,
            ConsentType = openIddictApp.ConsentType ?? string.Empty,
            // CreatedAt is already initialized in the OAuthApplication constructor
        };

        // Conditionally add RedirectUris
        if (openIddictApp.RedirectUris != null)
        {
            foreach (var uri in openIddictApp.RedirectUris)
            {
                domainApplication.RedirectUris.Add(uri.ToString());
            }
        }

        // Conditionally add PostLogoutRedirectUris
        if (openIddictApp.PostLogoutRedirectUris != null)
        {
            foreach (var uri in openIddictApp.PostLogoutRedirectUris)
            {
                domainApplication.PostLogoutRedirectUris.Add(uri.ToString());
            }
        }

        // Conditionally add Permissions
        if (openIddictApp.Permissions != null)
        {
            foreach (var permission in openIddictApp.Permissions)
            {
                domainApplication.Permissions.Add(permission.ToString());
            }
        }

        // Conditionally add Requirements
        if (openIddictApp.Requirements != null)
        {
            foreach (var requirement in openIddictApp.Requirements)
            {
                domainApplication.Requirements.Add(requirement.ToString());
            }
        }

        return domainApplication;
    }

    public async Task<OAuthApplication?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var openIddictApp = await _applicationManager.FindByIdAsync(id, cancellationToken) as OpenIddictEntityFrameworkCoreApplication;
        return MapToDomain(openIddictApp);
    }

    public async Task UpdateAsync(OAuthApplication application, CancellationToken cancellationToken = default)
    {
        var openIddictApp = await _applicationManager.FindByIdAsync(application.Id, cancellationToken);
        if (openIddictApp == null)
        {
            throw new InvalidOperationException($"OAuth application with ID {application.Id} not found for update.");
        }

        var descriptor = MapToOpenIddictDescriptor(application);
        await _applicationManager.UpdateAsync(openIddictApp, descriptor, cancellationToken);
    }
}
