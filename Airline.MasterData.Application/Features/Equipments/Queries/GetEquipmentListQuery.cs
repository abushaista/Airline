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

namespace Airline.MasterData.Application.Features.Equipments.Queries;
public record GetEquipmentListQuery() : QueryList, IRequest<GetEquipmentListQueryResponse>;

public record GetEquipmentListQueryResponse(bool success, QueryListResponse<Equipment>? data, string ErrorMessage);


public class GetEquipmentListQueryHandler : IRequestHandler<GetEquipmentListQuery, GetEquipmentListQueryResponse>
{
	private readonly ILogger<GetEquipmentListQueryHandler> _logger;
	private readonly IEquipmentRepository _equipmentRepository;

    public GetEquipmentListQueryHandler(ILogger<GetEquipmentListQueryHandler> logger, IEquipmentRepository equipmentRepository)
    {
        _logger = logger;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<GetEquipmentListQueryResponse> Handle(GetEquipmentListQuery request, CancellationToken cancellationToken)
    {
		try
		{
            var data = await _equipmentRepository.GetAll(request.page, request.row, cancellationToken);
            return new GetEquipmentListQueryResponse(true, data, string.Empty);
		}
		catch (Exception ex)
		{
            _logger.LogError(ex, ex.Message);
            return new GetEquipmentListQueryResponse(false, null, ex.Message);
		}
    }
}