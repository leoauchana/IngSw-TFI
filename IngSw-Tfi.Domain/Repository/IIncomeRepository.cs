using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Domain.Repository;

public interface IIncomeRepository : IRepository<Income>
{
    Task<List<Income>?> GetAllEarrings();
}
