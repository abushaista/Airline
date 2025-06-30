using Airline.MasterData.Domain.Entities;
using Airline.MasterData.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Application.Features.AircraftModels.Queries;
public record GetAirCraftByCodeQuery(string code): IRequest<AircraftByCodeResponse>;

public record AircraftByCodeResponse(bool Success, AircraftData? Aircraft);

public class GetAirCraftByCodeQueryHandler : IRequestHandler<GetAirCraftByCodeQuery, AircraftByCodeResponse>
{
    private readonly IAircraftDataRepository _repository;
    private readonly ILogger<GetAirCraftByCodeQueryHandler> _logger;
    public GetAirCraftByCodeQueryHandler(IAircraftDataRepository aircraftModelRepository, ILogger<GetAirCraftByCodeQueryHandler> logger)
    {
        _repository = aircraftModelRepository;
        _logger = logger;
    }
    public async Task<AircraftByCodeResponse> Handle(GetAirCraftByCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _repository.GetByCodeAsync(request.code, cancellationToken);
            return new AircraftByCodeResponse(data != null, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new AircraftByCodeResponse(false,null);
        }
    }
}
