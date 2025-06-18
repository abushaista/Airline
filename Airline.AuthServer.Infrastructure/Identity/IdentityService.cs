using Airline.AuthServer.Domain.Entities;
using Airline.AuthServer.Domain.Enums;
using Airline.AuthServer.Domain.Interfaces;
using Airline.AuthServer.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Airline.AuthServer.Infrastructure.Identity;
public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityApplicationUser> _userManager;
    private readonly SignInManager<IdentityApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityService(
            UserManager<IdentityApplicationUser> userManager,
            SignInManager<IdentityApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }
    public async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        if (identityUser == null)
        {
            throw new InvalidOperationException($"Identity user with ID {user.Id} not found for role assignment.");
        }
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
        await _userManager.AddToRoleAsync(identityUser, roleName);
    }

    public async Task<(DomainResult Result, ApplicationUser? User)> CreateUserAsync(ApplicationUser user, string password)
    {
        var identityUser = new IdentityApplicationUser
        {
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        };

        var result = await _userManager.CreateAsync(identityUser, password);
        var domainResult = result.Succeeded
            ? DomainResult.Success()
            : DomainResult.Failed(result.Errors.Select(e => e.Description));

        if (result.Succeeded)
        {
            user.Id = identityUser.Id;
        }

        return (domainResult, user);
    }

    public async Task<string?> GetEmailAsync(ApplicationUser user)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        return await _userManager.GetEmailAsync(identityUser ?? new IdentityApplicationUser(user));
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        if (identityUser == null) return new List<string>();
        return await _userManager.GetRolesAsync(identityUser);
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);
        return identityUser?.ToDomainUser();
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        var identityUser = await _userManager.FindByIdAsync(userId);
        return identityUser?.ToDomainUser();
    }

    public async Task<string> GetUserIdAsync(ApplicationUser user)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        return await _userManager.GetUserIdAsync(identityUser ?? new IdentityApplicationUser(user));
    }

    public async Task<string> GetUserNameAsync(ApplicationUser user)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        return await _userManager.GetUserNameAsync(identityUser ?? new IdentityApplicationUser(user));
    }

    public async Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        return await _userManager.IsEmailConfirmedAsync(identityUser ?? new IdentityApplicationUser(user));
    }

    public async Task<bool> IsSignedIn(ClaimsPrincipal principal)
    {
        return await Task.Run(() => _signInManager.IsSignedIn(principal));
    }

    public async Task<(LoginResult Result, ApplicationUser? User)> PasswordSignInAsync(string email, string password, bool rememberMe, bool lockoutOnFailure)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);
        if (identityUser == null)
        {
            return (LoginResult.InvalidCredentials, null);
        }

        var result = await _signInManager.PasswordSignInAsync(identityUser, password, rememberMe, lockoutOnFailure);

        if (result.Succeeded)
        {
            return (LoginResult.Succeeded, identityUser.ToDomainUser());
        }
        if (result.RequiresTwoFactor)
        {
            return (LoginResult.RequiresTwoFactor, identityUser.ToDomainUser());
        }
        if (result.IsLockedOut)
        {
            return (LoginResult.IsLockedOut, null);
        }

        return (LoginResult.InvalidCredentials, null);
    }

    public async Task SignInAsync(ApplicationUser user, bool isPersistent)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id) ?? new IdentityApplicationUser(user);
        await _signInManager.SignInAsync(identityUser, isPersistent);
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
