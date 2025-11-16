using IngSw_Tfi.Application.DTOs;

namespace IngSw_Tfi.Application.Interfaces;

public interface IIncomesService
{
    Task<List<IncomeDto.Response>?> GetAllEarrings();
    Task<List<IncomeDto.Response>> GetById(int idIncome);
    Task<IncomeDto.Response> AddIncome(IncomeDto.Request newIncome);
}
