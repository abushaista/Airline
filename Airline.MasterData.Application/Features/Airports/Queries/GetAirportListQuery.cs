using Airline.MasterData.Application.Commons;
using Airline.MasterData.Domain.Common;
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
public record GetAirportListQuery() : QueryList, IRequest<GetAirportListResponse>;

public record GetAirportListResponse(bool success, QueryListResponse<Airport>? data, string ErrorMessage);

public class GetAirportListQueryHandler : IRequestHandler<GetAirportListQuery, GetAirportListResponse>
{
    private readonly ILogger<GetAirportListQueryHandler> _logger;
    private readonly IAirportRepository _airportRepository;

    public GetAirportListQueryHandler(ILogger<GetAirportListQueryHandler> logger, IAirportRepository airportRepository)
    {
        _logger = logger;
        _airportRepository = airportRepository;
    }

    public async Task<GetAirportListResponse> Handle(GetAirportListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _airportRepository.GetAll(request.page, request.row, cancellationToken);
            return new GetAirportListResponse(true, data, string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new GetAirportListResponse(false, null, ex.Message);
        }
    }
}