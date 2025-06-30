using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Domain.Entities;
public class AircraftData
{
    public AircraftData() { }

    public AircraftData(string manufacturer, string model, string engineType, double maxSpeedKnots, double ceilingFt, double grossWeightLbs, double lengthFt, double heightFt, double wingSpanFt, double rangeNauticalMiles)
    {
        Manufacturer = manufacturer;
        Model = model;
        EngineType = engineType;
        MaxSpeedKnots = maxSpeedKnots;
        CeilingFt = ceilingFt;
        GrossWeightLbs = grossWeightLbs;
        LengthFt = lengthFt;
        HeightFt = heightFt;
        WingSpanFt = wingSpanFt;
        RangeNauticalMiles = rangeNauticalMiles;
    }

    public int Id { get; set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public string EngineType { get; set; }
    public double MaxSpeedKnots { get; set; }
    public double CeilingFt { get; set; }
    public double GrossWeightLbs { get; set; }
    public double LengthFt { get; set; }
    public double HeightFt { get; set; }
    public double WingSpanFt { get; set; }
    public double RangeNauticalMiles { get; set; }
}
