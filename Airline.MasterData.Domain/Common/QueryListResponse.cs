using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Domain.Common;
public class QueryListResponse<T> where T : class
{
    public int Count = 0;
    public int Page = 0;
    public IEnumerable<T> Items { get; set; } = new List<T>();
}
