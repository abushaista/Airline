using Airline.MasterData.Domain.Common;
using Airline.MasterData.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.MasterData.Domain.Interfaces;
public interface IEquipmentRepository
{
    Task<Equipment> GetById(int id, CancellationToken token);
    Task<Equipment> GetByCode(string code, CancellationToken token);
    Task<QueryListResponse<Equipment>> GetAll(int page, int row, CancellationToken token);
    Task<Equipment> AddAsync(Equipment entity, CancellationToken token);
}
