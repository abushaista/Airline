using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Domain.Entities;
public class Airport
{
    public Airport(string iata, string icao, string code, string name, string city, string region, string country, double latitude, double longitude, int elevationFt, string timeZone)
    {
        Iata = iata;
        Icao = icao;
        Code = code;
        Name = name;
        City = city;
        Region = region;
        Country = country;
        Latitude = latitude;
        Longitude = longitude;
        ElevationFt = elevationFt;
        TimeZone = timeZone;
    }

    public int Id { get; set; }
    public string Iata { get; set; } = string.Empty;
    public string Icao { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int ElevationFt { get; set; }
    public string TimeZone { get; set; } = string.Empty;
}
