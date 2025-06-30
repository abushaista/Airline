using Airline.MasterData.Domain.Common;
using Airline.MasterData.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Domain.Interfaces;
public interface IAirlineRepository
{
    Task<AirlineData> GetByCode(string code, CancellationToken token);
    Task<QueryListResponse<AirlineData>> GetAll(int page, int row, CancellationToken token);
    Task<AirlineData> AddAsync(AirlineData airlineData);
}
