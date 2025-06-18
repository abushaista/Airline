using Airline.AuthServer.Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.AuthServer.Infrastructure.Identity;

public class IdentityApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public IdentityApplicationUser() { }

    public IdentityApplicationUser(Domain.Entities.ApplicationUser domainUser)
    {
        Id = domainUser.Id;
        UserName = domainUser.UserName;
        Email = domainUser.Email;
        EmailConfirmed = domainUser.EmailConfirmed;
        FirstName = domainUser.FirstName;
        LastName = domainUser.LastName;
        CreatedAt = domainUser.CreatedAt;
    }

    public Domain.Entities.ApplicationUser ToDomainUser()
    {
        return new Domain.Entities.ApplicationUser
        {
            Id = Id,
            UserName = UserName,
            Email = Email!,
            EmailConfirmed = EmailConfirmed,
            FirstName = FirstName,
            LastName = LastName,
            CreatedAt = CreatedAt
        };
    }
}
