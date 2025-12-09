using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Bdd.Mock;

public class IncomeRepositoryInMemory : IIncomeRepository
{
    List<Income> Incomes { get; set; }
    public IncomeRepositoryInMemory()
    {
        Incomes = new List<Income>();
    }
    public async Task AddIncome(Income newIncome)
    {
        Incomes.Add(newIncome);
        await Task.CompletedTask;
    }

    public Task<List<Income>?> GetAll()
    {
        return Task.FromResult(Incomes.ToList() ?? null);
    }

    public Task<List<Income>?> GetAllEarrings()
    {
        var earrings = Incomes.Where(i => i.IncomeStatus == IncomeStatus.EARRING).ToList();
        return Task.FromResult(earrings ?? null);

    }

    public Task<Income?> GetById(string idIncome)
    {
        var income = Incomes.FirstOrDefault(i => i.Id.ToString() == idIncome);
        return Task.FromResult(income);
    }

    public Task<bool> HasActiveIncomeByPatient(string idPatient)
    {
        var incomeFound =  Incomes.Where(i => i.Patient!.Id.ToString().Equals(idPatient)).FirstOrDefault();
        if (incomeFound == null) return Task.FromResult(false);
        return Task.FromResult(true);
    }

    public Task UpdateStatus(Guid id, IncomeStatus status)
    {
        var income = Incomes.FirstOrDefault(i => i.Id == id);
        if (income != null) income.IncomeStatus = status;
        return Task.CompletedTask;
    }
}
