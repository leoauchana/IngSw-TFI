using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using System.Data;

namespace IngSw_Tfi.Data.Repositories;

public class IncomeRepository : RepositoryBase<Income>
{
    private readonly IncomeDao _incomeDao;
    public IncomeRepository(IncomeDao incomeDao)
    {
        _incomeDao = incomeDao;
    }
    public override async Task<List<Income>?> GetAll()
    {
        var incomesData = await _incomeDao.GetAll();
        var listIncomes = incomesData!.Select(i => MapEntity(i)).ToList();
        return listIncomes;
    }
    public override async Task<Income?> GetById(int idIncome)
    {
        var incomeData = await _incomeDao.GetById(idIncome);
        return MapEntity(incomeData);
    }
    public override async Task Add(Income newIncome) => await _incomeDao.AddIncome(newIncome);
    public override Task Update(Income entity)
    {
        throw new NotImplementedException();
    }
    public override Task Delete(int id)
    {
        throw new NotImplementedException();
    }



    protected override Income MapEntity(IDataRecord reader)
    {
        return new Income
        {

        };
    }
}
