using Airline.MasterData.Domain.Entities;
using Airline.MasterData.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Application.Features.AircraftModels.Commands;
public record CreateAircraftModelCommand(string Manufacturer, string Model, string EngineType, double MaxSpeedKnots, double CeilingFt, double GrossWeightLbs, double LengthFt, double HeightFt, double WingSpanFt, double RangeNauticalMiles) : IRequest<CreateAircraftModelResponse>;

public record CreateAircraftModelResponse(bool Succeeded, string[] Errors, AircraftData? AircraftModel);

public class CreateAircraftModelCommandHandler : IRequestHandler<CreateAircraftModelCommand, CreateAircraftModelResponse>
{
    private readonly IAircraftDataRepository _repository;
    private readonly ILogger<CreateAircraftModelCommandHandler> _logger;

    public CreateAircraftModelCommandHandler(IAircraftDataRepository repository, ILogger<CreateAircraftModelCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    public async Task<CreateAircraftModelResponse> Handle(CreateAircraftModelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var model = CommandToModel(request);
            var result = await _repository.AddAsync(model);
            _logger.LogInformation("aircraft created..");
            return new CreateAircraftModelResponse(true, [], result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,ex.Message);
            return new CreateAircraftModelResponse(false, [ ex.Message], AircraftModel: default);
        }
    }

    private AircraftData CommandToModel(CreateAircraftModelCommand command) {
        return new AircraftData()
        {
            Manufacturer = command.Manufacturer,
            Model = command.Model,
            EngineType = command.EngineType,
            MaxSpeedKnots = command.MaxSpeedKnots,
            CeilingFt = command.CeilingFt,
            GrossWeightLbs = command.GrossWeightLbs,
            LengthFt = command.LengthFt,
            HeightFt = command.HeightFt,
            WingSpanFt = command.WingSpanFt,
            RangeNauticalMiles = command.RangeNauticalMiles
        };
    }

}
