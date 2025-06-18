using Airline.AuthServer.Application.Features.Auth.Commands;
using Airline.AuthServer.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Airline.AuthServer.Web.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public LoginModel(IMediator mediator, IIdentityService identityService)
        {
            _mediator = mediator;
            _identityService = identityService;
        }
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string? ErrorMessage { get; set; }

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }
        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var request = HttpContext.GetOpenIddictServerRequest();
            if (request != null)
            {
                ReturnUrl = request?.GetParameter("return_url")?.ToString();
            }

            if (await _identityService.IsSignedIn(User))
            {
                if (request != null && !string.IsNullOrEmpty(request?.GetParameter("return_url")?.ToString()))
                {
                    return Redirect(request?.GetParameter("return_url")?.ToString());
                }
                return RedirectToPage("/Index"); // Changed from RedirectToAction
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var command = new LoginUserCommand(
                Input.Email,
                Input.Password,
                Input.RememberMe
            );

            var result = await _mediator.Send(command);

            if (result.Succeeded)
            {
                var request = HttpContext.GetOpenIddictServerRequest();
                if (request != null && !string.IsNullOrEmpty(request?.GetParameter("return_url")?.ToString()))
                {
                    return Redirect(request?.GetParameter("return_url")?.ToString());
                }
                return LocalRedirect(ReturnUrl ?? "/"); // Changed from RedirectToAction
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "User account locked out.");
            }
            else if (result.RequiresTwoFactor)
            {
                // Handle 2FA if you implement it
                ModelState.AddModelError(string.Empty, "Two-factor authentication is required.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return Page();
        }
    }
}
