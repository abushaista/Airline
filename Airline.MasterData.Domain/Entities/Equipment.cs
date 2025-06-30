using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Domain.Entities;
public class Equipment
{
    public Equipment() { }
    public Equipment(string code, string description)
    {
        Code = code;
        Description = description;
    }
    public int Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
}
