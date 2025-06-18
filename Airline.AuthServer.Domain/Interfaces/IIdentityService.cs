using Airline.AuthServer.Domain.Entities;
using Airline.AuthServer.Domain.Enums;
using Airline.AuthServer.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Airline.AuthServer.Domain.Interfaces;

public interface IIdentityService
{
    Task<(LoginResult Result, ApplicationUser? User)> PasswordSignInAsync(string email, string password, bool rememberMe, bool lockoutOnFailure);
    Task SignInAsync(ApplicationUser user, bool isPersistent);
    Task SignOutAsync();
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<string> GetUserIdAsync(ApplicationUser user);
    Task<string> GetUserNameAsync(ApplicationUser user);
    Task<string?> GetEmailAsync(ApplicationUser user);
    Task<bool> IsSignedIn(ClaimsPrincipal principal);
    Task<bool> IsEmailConfirmedAsync(ApplicationUser user);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);

    Task<(DomainResult Result, ApplicationUser? User)> CreateUserAsync(ApplicationUser user, string password);
    Task AddUserToRoleAsync(ApplicationUser user, string roleName);
}
