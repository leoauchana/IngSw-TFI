using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;
using IngSw_Tfi.Domain.Repository;
using System.Data;

namespace IngSw_Tfi.Data.Repositories;

public class IncomeRepository : IIncomeRepository
{
    private readonly IncomeDao _incomeDao;
    public IncomeRepository(IncomeDao incomeDao)
    {
        _incomeDao = incomeDao;
    }
    public async Task Add(Income newIncome) => await _incomeDao.AddIncome(newIncome);
    public async Task<Income?> GetById(int idIncome)
    {
        var incomeData = await _incomeDao.GetById(idIncome);
        if (incomeData == null) return null;
        return MapEntity(incomeData);
    }
    public async Task<List<Income>?> GetAllEarrings()
    {
        var incomesData = await _incomeDao.GetAll();
        if (incomesData == null) return null;
        var listIncomes = incomesData!.Select(i => MapEntity(i)).ToList();
        return listIncomes
                    .Where(i => i.IncomeStatus == IncomeStatus.EARRING)
                    .ToList();
    }
    private Income MapEntity(Dictionary<string, object> value)
    {
        return new Income
        {

        };
    }
}
