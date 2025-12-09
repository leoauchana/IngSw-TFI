using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Bdd.Mock;

public class EmployeeRepositoryInMemory : IEmployeeRepository
{
    List<Employee> Employees { get; set; }
    public EmployeeRepositoryInMemory()
    {
        Employees = new List<Employee>();
    }
    public Task<Employee?> GetByEmail(string userEmail)
    {
        throw new NotImplementedException();
    }

    public Task<Employee?> GetById(string idEmployee)
    {
        return Task.FromResult(Employees.Where(e => e.Id.ToString().Equals(idEmployee)).First() ?? null);
    }

    public Task<Employee?> Register(Employee newEmployee)
    {
        Employees.Add(newEmployee);
        return Task.FromResult(newEmployee ?? null);
    }
}
