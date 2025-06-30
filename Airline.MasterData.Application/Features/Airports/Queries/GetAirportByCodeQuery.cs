using Airline.MasterData.Domain.Entities;
using Airline.MasterData.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Application.Features.Airports.Queries;
public record GetAirportByCodeQuery(string code) : IRequest<GetAirportByCodeResponse>;

public record GetAirportByCodeResponse(bool success, Airport? data);

public class GetAirportByCodeQueryHandler : IRequestHandler<GetAirportByCodeQuery, GetAirportByCodeResponse>
{
    private readonly ILogger<GetAirportByCodeQueryHandler> _logger;
    private readonly IAirportRepository _airportRepository;

    public GetAirportByCodeQueryHandler(ILogger<GetAirportByCodeQueryHandler> logger, IAirportRepository airportRepository)
    {
        _logger = logger;
        _airportRepository = airportRepository;
    }

    public async Task<GetAirportByCodeResponse> Handle(GetAirportByCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _airportRepository.GetByCode(request.code, cancellationToken);
            if (data == null) {
                _logger.LogWarning($"{request.code} is not found");
                return new GetAirportByCodeResponse(false, null);
            }
            return new GetAirportByCodeResponse(true, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);  
            return new GetAirportByCodeResponse(false, null);
        }
    }
}
