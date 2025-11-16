using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Tfi.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    public Task<Employee?> GetByEmail(string userEmail)
    {
        throw new NotImplementedException();
    }

    public Task<Employee?> Register(Employee employee)
    {
        throw new NotImplementedException();
    }
}
