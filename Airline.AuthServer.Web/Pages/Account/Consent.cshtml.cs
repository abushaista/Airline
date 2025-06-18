using Airline.AuthServer.Application.Features.OAuthClients.Queries;
using Airline.AuthServer.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Airline.AuthServer.Web.Pages.Account
{
    [Authorize]
    public class ConsentModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public ConsentModel(IMediator mediator, IIdentityService identityService)
        {
            _mediator = mediator;
            _identityService = identityService;
        }

        [BindProperty]
        public ConsentViewModel ViewModel { get; set; } = new ConsentViewModel();

        public class ConsentViewModel
        {
            public string ApplicationName { get; set; } = string.Empty;
            public string? Scope { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request == null)
            {
                // This might happen if someone directly navigates to /consent without an OAuth flow
                return RedirectToPage("/Index");
            }

            var clientDto = await _mediator.Send(new GetOAuthClientByClientIdQuery(request.ClientId!));
            if (clientDto == null)
            {
                throw new InvalidOperationException($"The client application with ID '{request.ClientId}' cannot be found.");
            }

            ViewModel = new ConsentViewModel
            {
                ApplicationName = clientDto.DisplayName,
                Scope = request.Scope,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request == null)
            {
                return RedirectToPage("/Index");
            }

            if (Request.Form["submit.Accept"] == "Accept")
            {
                var principal = (await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme)).Principal;
                if (principal == null)
                {
                    throw new InvalidOperationException("No authenticated principal found for consent.");
                }

                var userId = principal.GetClaim(Claims.Subject)!;
                var user = await _identityService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found during consent processing.");
                }

                var claims = new List<Claim>();
                claims.AddRange(principal.Claims);

                if (request.HasScope(Scopes.OfflineAccess))
                {
                    claims.Add(new Claim(Claims.Scope, Scopes.OfflineAccess));
                }
                foreach (var scope in request.GetScopes())
                {
                    claims.Add(new Claim(Claims.Scope, scope));
                }

                if (!string.IsNullOrEmpty(user.FirstName)) claims.Add(new Claim("first_name", user.FirstName));
                if (!string.IsNullOrEmpty(user.LastName)) claims.Add(new Claim("last_name", user.LastName));

                var roles = await _identityService.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(Claims.Role, role));
                }

                var identity = new ClaimsIdentity(principal.Identity, claims,
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    Claims.Name, Claims.Role);

                await HttpContext.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                // Return a challenge to let OpenIddict process the consent result
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            // If "Deny" was clicked or another action, deny consent
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}
