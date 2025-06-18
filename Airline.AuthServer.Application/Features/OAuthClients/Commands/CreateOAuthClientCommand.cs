using Airline.AuthServer.Domain.Entities;
using Airline.AuthServer.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.AuthServer.Application.Features.OAuthClients.Commands;

public record CreateOAuthClientCommand(string ClientId,
        string DisplayName,
        string? ClientSecret,
        string Type,
        string ConsentType,
        List<string> RedirectUris,
        List<string> PostLogoutRedirectUris,
        List<string> Permissions,
        List<string> Requirements) : IRequest<CreateOAuthClientResponse>;

public record CreateOAuthClientResponse(string Id, string ClientId, bool Succeeded, string[] Errors);

public class CreateOAuthClientCommandHandler : IRequestHandler<CreateOAuthClientCommand, CreateOAuthClientResponse>
{
    private readonly IOAuthApplicationRepository _applicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOAuthClientCommandHandler(IOAuthApplicationRepository applicationRepository, IUnitOfWork unitOfWork)
    {
        _applicationRepository = applicationRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<CreateOAuthClientResponse> Handle(CreateOAuthClientCommand request, CancellationToken cancellationToken)
    {
        var existingClient = await _applicationRepository.GetByClientIdAsync(request.ClientId, cancellationToken);
        if (existingClient != null)
        {
            return new CreateOAuthClientResponse(
                string.Empty, request.ClientId, false, new[] { "A client with this ClientId already exists." }
            );
        }

        var application = new OAuthApplication
        {
            ClientId = request.ClientId,
            DisplayName = request.DisplayName,
            ClientSecret = request.ClientSecret,
            Type = request.Type,
            ConsentType = request.ConsentType,
            RedirectUris = request.RedirectUris,
            PostLogoutRedirectUris = request.PostLogoutRedirectUris,
            Permissions = request.Permissions,
            Requirements = request.Requirements
        };

        await _applicationRepository.AddAsync(application, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new CreateOAuthClientResponse(
            application.Id, application.ClientId, true, Array.Empty<string>()
        );
    }
}
