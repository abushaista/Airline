using Airline.MasterData.Domain.Entities;
using Airline.MasterData.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Application.Features.Equipments.Commands;
public record CreateEquipmentCommand(string Code, string Description) : IRequest<CreateEquipmentCommandResponse>;

public record CreateEquipmentCommandResponse(bool success, Equipment? data, string ErrorMessage);

public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, CreateEquipmentCommandResponse>
{
    private readonly ILogger<CreateEquipmentCommandHandler> _logger;
    private readonly IEquipmentRepository _equipmentRepository;

    public CreateEquipmentCommandHandler(ILogger<CreateEquipmentCommandHandler> logger, IEquipmentRepository equipmentRepository)
    {
        _logger = logger;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<CreateEquipmentCommandResponse> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var equipment = new Equipment(request.Code, request.Description);
            var data = await _equipmentRepository.AddAsync(equipment, cancellationToken);
            return new CreateEquipmentCommandResponse(true, data, string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new CreateEquipmentCommandResponse(false, null, ex.Message);            
        }
    }
}