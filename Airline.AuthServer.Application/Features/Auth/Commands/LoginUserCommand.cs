using Airline.AuthServer.Domain.Entities;
using Airline.AuthServer.Domain.Enums;
using Airline.AuthServer.Domain.Interfaces;
using MediatR;

namespace Airline.AuthServer.Application.Features.Auth.Commands;


public record LoginUserCommand(string Email, string Password, bool RememberMe) : IRequest<LoginResponse>;

public record LoginResponse(bool Succeeded, bool RequiresTwoFactor, bool IsLockedOut, string[] Errors, ApplicationUser? User);

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;

    public LoginUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<LoginResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var (result, user) = await _identityService.PasswordSignInAsync(
                request.Email, request.Password, request.RememberMe, lockoutOnFailure: true);

        switch (result)
        {
            case LoginResult.Succeeded:
                return new LoginResponse(true, false, false, Array.Empty<string>(), user);
            case LoginResult.RequiresTwoFactor:
                return new LoginResponse(false, true, false, Array.Empty<string>(), user);
            case LoginResult.IsLockedOut:
                return new LoginResponse(false, false, true, new[] { "User account locked out." }, null);
            case LoginResult.InvalidCredentials:
                return new LoginResponse(false, false, false, new[] { "Invalid login attempt." }, null);
            default:
                return new LoginResponse(false, false, false, new[] { "An unexpected error occurred." }, null);
        }
    }
}
