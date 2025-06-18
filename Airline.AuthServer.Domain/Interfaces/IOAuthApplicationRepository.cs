using Airline.AuthServer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.AuthServer.Domain.Interfaces;

public interface IOAuthApplicationRepository
{
    Task<OAuthApplication?> GetByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
    Task<OAuthApplication?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OAuthApplication>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(OAuthApplication application, CancellationToken cancellationToken = default);
    Task UpdateAsync(OAuthApplication application, CancellationToken cancellationToken = default);
    Task DeleteAsync(OAuthApplication application, CancellationToken cancellationToken = default);
}
