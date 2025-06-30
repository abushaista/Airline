using Airline.MasterData.Domain.Entities;
using Airline.MasterData.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Application.Features.AirlineDatas.Commands;
public record CreateAirlineCommand(string Iata, string Icao, string BusinessName, string CommonName) : IRequest<CreateAirlineRepsonse>;

public record CreateAirlineRepsonse(bool Success, AirlineData? Data);


public class CreateAirlineCommandHandler : IRequestHandler<CreateAirlineCommand, CreateAirlineRepsonse>
{
    private readonly ILogger<CreateAirlineCommandHandler> _logger;
    private readonly IAirlineRepository _airlineRepository;

    public CreateAirlineCommandHandler(ILogger<CreateAirlineCommandHandler> logger, IAirlineRepository airlineRepository)
    {
        _logger = logger;
        _airlineRepository = airlineRepository;
    }

    public async Task<CreateAirlineRepsonse> Handle(CreateAirlineCommand request, CancellationToken cancellationToken)
    {
        var data = new AirlineData(request.Iata, request.Icao, request.BusinessName, request.CommonName);

        try
        {
            var result = await _airlineRepository.AddAsync(data);
            return new CreateAirlineRepsonse(true, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new CreateAirlineRepsonse(false, null);
        }
    }

}