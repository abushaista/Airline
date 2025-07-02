using Airline.MasterData.Domain.Common;
using Airline.MasterData.Domain.Entities;
using Airline.MasterData.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Infrastructure.Persistence.Repositories;
public class AircraftDataRepository : IAircraftDataRepository
{
    private readonly MasterDataBbContext _context;
    private ILogger<AircraftDataRepository> _logger;

    public AircraftDataRepository(MasterDataBbContext context, ILogger<AircraftDataRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AircraftData> AddAsync(AircraftData model)
    {
        _ = await _context.AircraftDatas.AddAsync(model);
         await _context.SaveChangesAsync();
        return model;
    }

    public async Task<QueryListResponse<AircraftData>> GetAllAsync(int Page, int Row, CancellationToken token)
    {
        try
        {
            var count = await _context.AircraftDatas.CountAsync(token);
            var data = await _context.AircraftDatas.Skip(Page * Row).Take(Row).ToListAsync(token);
            return new QueryListResponse<AircraftData>() { Count = count, Items = data, Page = Page };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new QueryListResponse<AircraftData>() { Count = 0, Items = new List<AircraftData>(), Page = 1 };
        }
        
        
    }

    public async Task<AircraftData?> GetByCodeAsync(string code, CancellationToken token)
    {
        try
        {
            var data = await _context.AircraftDatas.FirstOrDefaultAsync(x => x.IataCode == code, token);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }
}
