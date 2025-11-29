using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;

namespace IngSw_Tfi.Domain.Repository;

public interface IIncomeRepository : IRepository<Income>
{
    Task<List<Income>?> GetAllEarrings();
    Task<List<Income>?> GetAll();
    Task UpdateStatus(Guid id, IncomeStatus status);
}
