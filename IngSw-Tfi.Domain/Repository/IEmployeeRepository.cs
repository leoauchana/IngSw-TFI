using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Domain.Repository;

public interface IEmployeeRepository
{
    Task<Employee?> GetByEmail(string userEmail);
    Task<Employee?> Register(Employee employee);
}
