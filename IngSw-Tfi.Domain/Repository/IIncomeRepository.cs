using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;

namespace IngSw_Tfi.Domain.Repository;

public interface IIncomeRepository
{
    Task<List<Income>?> GetAllEarrings();
    Task<List<Income>?> GetAll();
    Task UpdateStatus(Guid id, IncomeStatus status);
    Task AddIncome(Income newIncome);
    Task<Income?> GetById(string idIncome);
    Task<bool> HasActiveIncomeByPatient(string idPatient);
}
