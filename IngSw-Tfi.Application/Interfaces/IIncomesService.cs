using IngSw_Tfi.Application.DTOs;

namespace IngSw_Tfi.Application.Interfaces;

public interface IIncomesService
{
    List<IncomeDto.ResponseTest> GetAllEarrings();
    Task<List<IncomeDto.Response>> GetById(int idIncome);
    Task<IncomeDto.ResponseTest> AddIncome(IncomeDto.Request newIncome);
}
