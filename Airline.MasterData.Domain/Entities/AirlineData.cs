using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Domain.Entities;
public class AirlineData
{
    public AirlineData() { }
    public AirlineData(string iata, string icao, string businessName, string commonName)
    {
        Iata = iata;
        Icao = icao;
        BusinessName = businessName;
        CommonName = commonName;
    }

    public int Id { get; set; }
    public string Iata { get; set; }
    public string Icao { get; set; }
    public string BusinessName { get; set; }
    public string CommonName    { get; set; }
}
