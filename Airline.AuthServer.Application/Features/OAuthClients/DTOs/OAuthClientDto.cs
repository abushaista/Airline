using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.AuthServer.Application.Features.OAuthClients.DTOs;

public record OAuthClientDto(string Id,
        string ClientId,
        string DisplayName,
        string Type,
        string ConsentType,
        List<string> RedirectUris,
        List<string> PostLogoutRedirectUris,
        List<string> Permissions);
