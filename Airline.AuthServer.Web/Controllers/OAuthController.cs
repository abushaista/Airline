using Airline.AuthServer.Application.Features.OAuthClients.Queries;
using Airline.AuthServer.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Airline.AuthServer.Web.Controllers;
public class OAuthController : Controller
{
    private readonly IIdentityService _identityService;
    private readonly IMediator _mediator;
    public OAuthController(IIdentityService identityService, IMediator mediator)
    {
        _identityService = identityService;
        _mediator = mediator;
    }

    [HttpPost("~/connect/token"), Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
        {
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        }

        if (request.IsClientCredentialsGrantType())
        {
            var clientDto = await _mediator.Send(new GetOAuthClientByClientIdQuery(request.ClientId!));
            if (clientDto == null)
            {
                throw new InvalidOperationException("The client application cannot be found.");
            }

            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(Claims.Subject, clientDto.ClientId);
            identity.AddClaim(Claims.Name, clientDto.DisplayName);

            identity.SetResources(request?.Resources);
            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        else if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType() || request.IsPasswordGrantType())
        {
            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            if (principal == null)
            {
                throw new InvalidOperationException("No principal found for token exchange.");
            }

            var userId = principal.GetClaim(Claims.Subject);
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User ID claim not found in principal.");
            }

            var user = await _identityService.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("The user corresponding to the authorization code/refresh token cannot be found.");
            }

            principal.SetClaim(Claims.Subject, user.Id);
            principal.SetClaim(Claims.Name, await _identityService.GetUserNameAsync(user));

            if (!string.IsNullOrEmpty(user.FirstName)) principal.AddClaim("first_name", user.FirstName);
            if (!string.IsNullOrEmpty(user.LastName)) principal.AddClaim("last_name", user.LastName);

            var roles = await _identityService.GetRolesAsync(user);
            foreach (var role in roles)
            {
                principal.AddClaim(Claims.Role, role);
            }

            principal.SetScopes(request.GetScopes());
            principal.SetResources(request.Resources);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        throw new InvalidOperationException("The specified grant type is not supported.");
    }

    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("~/connect/userinfo"), Produces("application/json")]
    public async Task<IActionResult> Userinfo()
    {
        var userId = User.GetClaim(Claims.Subject);
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException("Subject claim not found in user principal.");
        }

        var user = await _identityService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                // Fix: Wrap the dictionary in a new AuthenticationProperties object
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The access token is not valid."
                }));
        }

        var claims = new List<Claim>();
        claims.Add(new Claim(Claims.Subject, user.Id));

        if (User.HasScope(Scopes.Email))
        {
            claims.Add(new Claim(Claims.Email, await _identityService.GetEmailAsync(user)!));
            claims.Add(new Claim(Claims.EmailVerified, (await _identityService.IsEmailConfirmedAsync(user)).ToString()));
        }
        if (User.HasScope(Scopes.Profile))
        {
            claims.Add(new Claim(Claims.Name, await _identityService.GetUserNameAsync(user)!));
            claims.Add(new Claim("first_name", user.FirstName ?? string.Empty));
            claims.Add(new Claim("last_name", user.LastName ?? string.Empty));
        }

        if (User.HasScope(Scopes.Roles))
        {
            foreach (var role in await _identityService.GetRolesAsync(user))
            {
                claims.Add(new Claim(Claims.Role, role));
            }
        }

        return Ok(new ClaimsPrincipal(new ClaimsIdentity(claims,
            authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)));
    }


}
