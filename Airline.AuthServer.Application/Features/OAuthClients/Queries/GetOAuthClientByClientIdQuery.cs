using Airline.AuthServer.Application.Features.OAuthClients.DTOs;
using Airline.AuthServer.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.AuthServer.Application.Features.OAuthClients.Queries;

public record GetOAuthClientByClientIdQuery(string ClientId) : IRequest<OAuthClientDto?>;

public class GetOAuthClientByClientIdQueryHandler : IRequestHandler<GetOAuthClientByClientIdQuery, OAuthClientDto?>
{
    private readonly IOAuthApplicationRepository _applicationRepository;

    public GetOAuthClientByClientIdQueryHandler(IOAuthApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }
    public async Task<OAuthClientDto?> Handle(GetOAuthClientByClientIdQuery request, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByClientIdAsync(request.ClientId, cancellationToken);

        if (application == null)
        {
            return null;
        }

        return new OAuthClientDto(
            application.Id,
            application.ClientId,
            application.DisplayName,
            application.Type,
            application.ConsentType,
            application.RedirectUris.ToList(),
            application.PostLogoutRedirectUris.ToList(),
            application.Permissions.ToList()
        );
    }
}
