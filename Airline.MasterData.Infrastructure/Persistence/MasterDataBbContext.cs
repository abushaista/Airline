using Airline.MasterData.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Infrastructure.Persistence;
public class MasterDataBbContext : DbContext
{
    public DbSet<AirlineData> AirlineDatas { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<AircraftData> AircraftDatas { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public MasterDataBbContext(DbContextOptions<MasterDataBbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
