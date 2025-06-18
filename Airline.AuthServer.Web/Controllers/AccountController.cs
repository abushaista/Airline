using Airline.AuthServer.Application.Features.Auth.Commands;
using Airline.AuthServer.Application.Features.OAuthClients.Queries;
using Airline.AuthServer.Domain.Interfaces;
using Airline.AuthServer.Web.ViewModels;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Airline.AuthServer.Web.Controllers;
public class AccountController : Controller
{
    private readonly IMediator _mediator;
    private readonly IIdentityService _identityService;
    public AccountController(IMediator mediator, IIdentityService identityService)
    {
        _mediator = mediator;
        _identityService = identityService;
    }

    //[HttpGet("~/login")]
    //[HttpPost("~/login")]
    //public async Task<IActionResult> Login([FromQuery] string? returnUrl = null, LoginModel? model = null)
    //{
    //    var request = HttpContext.GetOpenIddictServerRequest();
    //    if (request != null)
    //    {
    //        returnUrl= request?.GetParameter("return_url")?.ToString();
    //    }

    //    if (await _identityService.IsSignedIn(User))
    //    {
    //        if (request != null && !string.IsNullOrEmpty(request?.GetParameter("return_url")?.ToString()))
    //        {
    //            return Redirect(request?.GetParameter("return_url")?.ToString());
    //        }
    //        return RedirectToAction("Index", "Home");
    //    }

    //    if (HttpContext.Request.Method == "GET")
    //    {
    //        ViewData["ReturnUrl"] = returnUrl ?? "http://localhost:";
    //        // Initialize model for GET request if it's null
    //        if (model == null)
    //        {
    //            model = new LoginModel();
    //        }
    //        return View(model);
    //    }

    //    // For POST request, model is bound automatically
    //    if (model == null || !ModelState.IsValid)
    //    {
    //        ViewData["ReturnUrl"] = returnUrl;
    //        return View(model);
    //    }

    //    var command = new LoginUserCommand(
    //        model.Email!, // Use model.Email
    //        model.Password!, // Use model.Password
    //        model.RememberMe
    //    );
    //    var result = await _mediator.Send(command);
    //    if (result.Succeeded)
    //    {
    //        if (request != null && !string.IsNullOrEmpty(request?.GetParameter("return_url")?.ToString()))
    //        {
    //            return Redirect(request?.GetParameter("return_url")?.ToString());
    //        }
    //        return RedirectToAction("Index", "Home");
    //    }

    //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    //    ViewData["ReturnUrl"] = returnUrl;
    //    return View(model);
    //}


    //[HttpGet("~/consent")]
    //[HttpPost("~/consent")]
    //[Authorize]
    //public async Task<IActionResult> Consent()
    //{
    //    var request = HttpContext.GetOpenIddictServerRequest();
    //    if (request == null)
    //    {
    //        return RedirectToAction("Index", "Home");
    //    }

    //    var clientDto = await _mediator.Send(new GetOAuthClientByClientIdQuery(request.ClientId!));
    //    if (clientDto == null)
    //    {
    //        throw new InvalidOperationException($"The client application with ID '{request.ClientId}' cannot be found.");
    //    }

    //    var viewModel = new ConsentViewModel
    //    {
    //        ApplicationName = clientDto.DisplayName,
    //        Scope = request.Scope,
    //    };

    //    if (HttpContext.Request.Method == "GET")
    //    {
    //        return View(viewModel);
    //    }
    //    if (HttpContext.Request.Form["submit.Accept"] == "Accept")
    //    {
    //        var principal = (await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme)).Principal;
    //        if (principal == null)
    //        {
    //            throw new InvalidOperationException("No authenticated principal found for consent.");
    //        }

    //        var userId = principal.GetClaim(Claims.Subject)!;
    //        var user = await _identityService.GetUserByIdAsync(userId);
    //        if (user == null)
    //        {
    //            throw new InvalidOperationException("User not found during consent processing.");
    //        }

    //        var claims = new List<Claim>();
    //        claims.AddRange(principal.Claims);

    //        if (request.HasScope(Scopes.OfflineAccess))
    //        {
    //            claims.Add(new Claim(Claims.Scope, Scopes.OfflineAccess));
    //        }
    //        foreach (var scope in request.GetScopes())
    //        {
    //            claims.Add(new Claim(Claims.Scope, scope));
    //        }

    //        if (!string.IsNullOrEmpty(user.FirstName)) claims.Add(new Claim("first_name", user.FirstName));
    //        if (!string.IsNullOrEmpty(user.LastName)) claims.Add(new Claim("last_name", user.LastName));

    //        var roles = await _identityService.GetRolesAsync(user);
    //        foreach (var role in roles)
    //        {
    //            claims.Add(new Claim(Claims.Role, role));
    //        }

    //        var identity = new ClaimsIdentity(principal.Identity, claims,
    //            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
    //            Claims.Name, Claims.Role);

    //        await HttpContext.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

    //        return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    //    }

    //    return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    //}

    [HttpGet("~/logout")]
    [HttpPost("~/logout")]
    public async Task<IActionResult> Logout()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        await _identityService.SignOutAsync();
        if (request != null)
        {
            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = request.PostLogoutRedirectUri
                });
        }

        return RedirectToAction("Index", "Home");
    }

}
