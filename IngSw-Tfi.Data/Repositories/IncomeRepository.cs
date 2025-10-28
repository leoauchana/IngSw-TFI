using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;
using System.Data;

namespace IngSw_Tfi.Data.Repositories;

public class IncomeRepository : IRepository<Income>
{
    private readonly IncomeDao _incomeDao;
    public IncomeRepository(IncomeDao incomeDao)
    {
        _incomeDao = incomeDao;
    }
    public async Task<List<Income>?> GetAll()
    {
        var incomesData = await _incomeDao.GetAll();
        var listIncomes = incomesData!.Select(i => MapEntity(i)).ToList();
        return listIncomes;
    }
    public async Task<Income?> GetById(int idIncome)
    {
        var incomeData = await _incomeDao.GetById(idIncome);
        return MapEntity(incomeData);
    }
    public async Task Add(Income newIncome) => await _incomeDao.AddIncome(newIncome);
    public Task Update(Income entity)
    {
        throw new NotImplementedException();
    }
    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    protected Income MapEntity(Dictionary<string, object>? reader)
    {
        return new Income
        {

        };
    }
}
