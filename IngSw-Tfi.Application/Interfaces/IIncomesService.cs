using IngSw_Tfi.Application.DTOs;

namespace IngSw_Tfi.Application.Interfaces;

public interface IIncomesService
{
    Task<List<IncomeDto.Response>?> GetAllEarringss();
    Task<List<IncomeDto.Response>?> GetAllEarrings();
    Task<IncomeDto.Response?> GetById(string idIncome);
    Task<IncomeDto.Response?> AddIncome(IncomeDto.Request newIncome);
    Task<IncomeDto.Response?> AddIncomeT(string idUser, IncomeDto.RequestT newIncome);
    Task<List<IncomeDto.Response>?> GetAll();
    Task<IncomeDto.Response?> UpdateIncomeStatus(string incomeId, string newStatus);
}
