using Airline.MasterData.Domain.Entities;
using Airline.MasterData.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Application.Features.Equipments.Queries;
public record GetEquipmentByCodeQuery(string Code) : IRequest<GetEquipmentByCodeResponse>;

public record GetEquipmentByCodeResponse(bool success, Equipment? data, string ExceptionMessage);


public class GetEquipmentByCodeQueryHandler : IRequestHandler<GetEquipmentByCodeQuery, GetEquipmentByCodeResponse>
{
    private readonly ILogger<GetEquipmentByCodeQueryHandler> _logger;
    private readonly IEquipmentRepository _equipmentRepository;

    public GetEquipmentByCodeQueryHandler(ILogger<GetEquipmentByCodeQueryHandler> logger, IEquipmentRepository equipmentRepository)
    {
        _logger = logger;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<GetEquipmentByCodeResponse> Handle(GetEquipmentByCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _equipmentRepository.GetByCode(request.Code, cancellationToken);
            if (data != null)
            {
                return new GetEquipmentByCodeResponse(true, data, string.Empty);
            }
            return new GetEquipmentByCodeResponse(false, null, "404 - data not found");
        }
            
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new GetEquipmentByCodeResponse(false, null, ex.Message);
        }
    }
}