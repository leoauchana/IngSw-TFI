using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Tfi.Application.Services;

public class IncomesService : IIncomesService
{
    private readonly IRepository<Income> _incomeRepository;

    public IncomesService(IRepository<Income> incomeRepository)
    {
        _incomeRepository = incomeRepository;
    }

    public Task<IncomeDto.Response> AddIncome(IncomeDto.Request newIncome)
    {
        throw new NotImplementedException();
    }

    public async Task<List<IncomeDto.Response>?> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<IncomeDto.Response>> GetById(int idIncome)
    {
        throw new NotImplementedException();
    }
}
