using IngSw_Tfi.Application.DTOs;

namespace IngSw_Tfi.Application.Interfaces;

public interface IIncomesService
{
    List<IncomeDto.Response>? GetAllEarrings();
    Task<IncomeDto.Response?> GetById(string idIncome);
    Task<IncomeDto.Response?> AddIncome(string idUser, IncomeDto.RequestT newIncome);
    Task<List<IncomeDto.Response>?> GetAll();
}
