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

namespace Airline.MasterData.Application.Features.AircraftModels.Queries;
public record GetAllAircraftQuery() : QueryList, IRequest<GetAllAircraftQueryResponse>;

public record GetAllAircraftQueryResponse(bool Success, QueryListResponse<AircraftData>? data, string ErrorMessage);


public class GetAllAircraftQueryHandler : IRequestHandler<GetAllAircraftQuery, GetAllAircraftQueryResponse>
{
    private readonly ILogger<GetAllAircraftQueryHandler> _logger;
    private readonly IAircraftDataRepository _repository;
    public GetAllAircraftQueryHandler(ILogger<GetAllAircraftQueryHandler> logger, IAircraftDataRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    public async Task<GetAllAircraftQueryResponse> Handle(GetAllAircraftQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _repository.GetAllAsync(request.page, request.row, cancellationToken);
            return new GetAllAircraftQueryResponse(true, data, string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new GetAllAircraftQueryResponse(false, null, ex.Message);
        }
        
    }
}