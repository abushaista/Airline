using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.AuthServer.Domain.Entities;

public class OAuthApplication
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ClientId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ClientSecret { get; set; }
    public string Type { get; set; } = string.Empty;
    public string ConsentType { get; set; } = string.Empty;

    public ICollection<string> RedirectUris { get; set; } = new List<string>();
    public ICollection<string> PostLogoutRedirectUris { get; set; } = new List<string>();
    public ICollection<string> Permissions { get; set; } = new List<string>();
    public ICollection<string> Requirements { get; set; } = new List<string>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
