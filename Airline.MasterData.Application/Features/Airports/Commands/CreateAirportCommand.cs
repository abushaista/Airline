using Airline.MasterData.Domain.Entities;
using Airline.MasterData.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Application.Features.Airports.Commands;
public record CreateAirportCommand(string Iata, string Icao, string Code, string Name, string City, string Region, string Country, double Latitude, double Longitude, int ElevationFt, string Timezone) : IRequest<CreateAirportResponse>;


public record CreateAirportResponse(bool status, Airport? createdData);


public class CreateAirportCommandHandler : IRequestHandler<CreateAirportCommand, CreateAirportResponse>
{
    private readonly ILogger<CreateAirportCommandHandler> _logger;
    private readonly IAirportRepository _airportRepository;

    public CreateAirportCommandHandler(ILogger<CreateAirportCommandHandler> logger, IAirportRepository airportRepository)
    {
        _logger = logger;
        _airportRepository = airportRepository;
    }

    public async Task<CreateAirportResponse> Handle(CreateAirportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var data = new Airport(request.Iata,request.Icao,request.Code, request.Name, request.City,request.Region, request.Country, request.Latitude, request.Longitude,request.ElevationFt, request.Timezone);
            var result = await _airportRepository.AddAsync(data);
            _logger.LogInformation("airport created..");
            return new CreateAirportResponse(true, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new CreateAirportResponse(false, null);
        }
    }
}