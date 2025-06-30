using Airline.MasterData.Domain.Entities;
using Airline.MasterData.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Application.Features.AirlineDatas.Queries;
public record GetAirlineByCodeQuery(string Code) : IRequest<GetAirlineByCodeResponse>;

public record GetAirlineByCodeResponse(bool Success, AirlineData? Airline, string ErrorMessage);

public class GetAirlineByCodeQueryHandler : IRequestHandler<GetAirlineByCodeQuery, GetAirlineByCodeResponse>
{
    private readonly ILogger<GetAirlineByCodeQueryHandler> _logger;
    private readonly IAirlineRepository _airlineRepository;

    public GetAirlineByCodeQueryHandler(ILogger<GetAirlineByCodeQueryHandler> logger, IAirlineRepository airlineRepository)
    {
        _logger = logger;
        _airlineRepository = airlineRepository;
    }

    public async Task<GetAirlineByCodeResponse> Handle(GetAirlineByCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _airlineRepository.GetByCode(request.Code, cancellationToken);
            if (data == null) {
                return new GetAirlineByCodeResponse(false, null, "404 - data not found");
            }
            return new GetAirlineByCodeResponse(true, data, string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new GetAirlineByCodeResponse(false,null, ex.Message);
        }
    }
}

