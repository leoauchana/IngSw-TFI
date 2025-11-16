using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Tfi.Application.Services;

public class IncomesService : IIncomesService
{
    private readonly IIncomeRepository _incomeRepository;

    public IncomesService(IIncomeRepository incomeRepository)
    {
        _incomeRepository = incomeRepository;
    }

    public Task<IncomeDto.Response> AddIncome(IncomeDto.Request newIncome)
    {
        throw new NotImplementedException();
    }

    public async Task<List<IncomeDto.Response>?> GetAllEarrings()
    {
        var incomesEarrings = await _incomeRepository.GetAllEarrings();
        if (incomesEarrings == null || !(incomesEarrings.Count > 0)) throw new NullException("No hay ingresos pendientes.");
        //return incomesEarrings.Select(i => new IncomeDto.Response
        //{

        //});
        return null;
    }

    public Task<List<IncomeDto.Response>> GetById(int idIncome)
    {
        throw new NotImplementedException();
    }
}
