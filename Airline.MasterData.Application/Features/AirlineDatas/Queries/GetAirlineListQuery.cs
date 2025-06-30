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

namespace Airline.MasterData.Application.Features.AirlineDatas.Queries;
public record GetAirlineListQuery() : QueryList, IRequest<GetAirlineListResponse>;

public record GetAirlineListResponse(bool success, QueryListResponse<AirlineData>? data, string ErrorMessage);


public class GetAirlineListQueryHandler : IRequestHandler<GetAirlineListQuery, GetAirlineListResponse>
{
    private readonly ILogger<GetAirlineListQueryHandler> _logger;
    private readonly IAirlineRepository _airlineRepository;

    public GetAirlineListQueryHandler(ILogger<GetAirlineListQueryHandler> logger, IAirlineRepository airlineRepository)
    {
        _logger = logger;
        _airlineRepository = airlineRepository;
    }

    public async Task<GetAirlineListResponse> Handle(GetAirlineListQuery request, CancellationToken cancellationToken)
    {

        try
        {
            var data = await _airlineRepository.GetAll(request.page, request.row, cancellationToken);
            return new GetAirlineListResponse(true, data, string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new GetAirlineListResponse(false, null, ex.Message);
        }
    }
}