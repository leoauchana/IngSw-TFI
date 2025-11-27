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
    public async Task AddTransactional(System.Data.IDbConnection conn, System.Data.IDbTransaction tx, Income newIncome) => await _incomeDao.AddIncome(newIncome, conn, tx);
    public async Task Add(Income newIncome, System.Data.IDbConnection conn, System.Data.IDbTransaction? tx) => await _incomeDao.AddIncome(newIncome, conn, tx);
    public async Task<Income?> GetById(int idIncome)
    {
        var incomeData = await _incomeDao.GetById(idIncome);
        return MapEntity(incomeData);
    }
    public async Task<List<Income>?> GetAllEarrings()
    {
        var incomesData = await _incomeDao.GetAll();
        var listIncomes = incomesData!.Select(i => MapEntity(i)).ToList();
        return listIncomes
                    .Where(i => i.IncomeStatus == IncomeStatus.EARRING)
                    .ToList();
    }
    private Income MapEntity(Dictionary<string, object> value)
    {
        var patient = new Patient
        {
            Name = value.ContainsKey("name") ? value["name"]?.ToString() : value.GetValueOrDefault("p.name")?.ToString(),
            LastName = value.ContainsKey("last_name") ? value["last_name"]?.ToString() : value.GetValueOrDefault("p.last_name")?.ToString(),
            Cuil = value.ContainsKey("cuil") ? Domain.ValueObjects.Cuil.Create(value["cuil"]?.ToString()) : (value.GetValueOrDefault("p.cuil") != null ? Domain.ValueObjects.Cuil.Create(value.GetValueOrDefault("p.cuil")?.ToString()) : null),
            Domicilie = new Domicilie
            {
                Street = value.ContainsKey("street_domicilie") ? value["street_domicilie"]?.ToString() : value.GetValueOrDefault("p.street_domicilie")?.ToString(),
                Number = value.ContainsKey("number_domicilie") ? Convert.ToInt32(value["number_domicilie"]) : (value.GetValueOrDefault("p.number_domicilie") != null ? Convert.ToInt32(value.GetValueOrDefault("p.number_domicilie")) : 0),
                Locality = value.ContainsKey("locality_domicilie") ? value["locality_domicilie"]?.ToString() : value.GetValueOrDefault("p.locality_domicilie")?.ToString()
            }
        };

        var income = new Income
        {
            Patient = patient,
            Description = value.ContainsKey("description") ? value["description"]?.ToString() : null,
            IncomeDate = value.ContainsKey("income_date") && DateTime.TryParse(value["income_date"]?.ToString(), out var dt) ? dt : (DateTime?)null
        };
        return income;
    }
}
