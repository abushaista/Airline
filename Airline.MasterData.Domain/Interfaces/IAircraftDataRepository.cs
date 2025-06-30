using Airline.MasterData.Domain.Common;
using Airline.MasterData.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Domain.Interfaces;
public interface IAircraftDataRepository
{
    Task<QueryListResponse<AircraftData>> GetAllAsync(int Page, int Row, CancellationToken token);
    Task<AircraftData?> GetByCodeAsync(string code, CancellationToken token);
    Task<AircraftData> AddAsync(AircraftData model);
}
