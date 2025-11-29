using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;

namespace IngSw_Tfi.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeDao _employeeDao;
    public EmployeeRepository(EmployeeDao employeeDao)
    {
        _employeeDao = employeeDao;
    }
    public async Task<Employee?> GetByEmail(string email)
    {
        // Buscar en nurse
        var nurseData = await _employeeDao.GetByEmailNurse(email);
        if (nurseData is not null)
            return MapEntity(nurseData.FirstOrDefault()!);

        // Buscar en doctor
        var doctorData = await _employeeDao.GetByEmailDoctor(email);
        if (doctorData is not null)
            return MapEntity(doctorData.FirstOrDefault()!);

        return null;
    }

    public Task<Employee?> Register(Employee employee)
    {
        throw new NotImplementedException();
    }

    private Employee MapEntity(Dictionary<string, object> value)
    {
        bool isDoctor = value.ContainsKey("license_number");

        Employee employee = isDoctor ? new Doctor() : new Nurse();

        employee.Cuil = Cuil.Create(value.GetValueOrDefault("cuil")?.ToString()!);
        employee.Name = value.GetValueOrDefault("first_name")?.ToString();
        employee.LastName = value.GetValueOrDefault("last_name")?.ToString();
        employee.Email = value.GetValueOrDefault("email")?.ToString();
        employee.PhoneNumber = value.GetValueOrDefault("phone_number")?.ToString();
       // employee.User = null;
        if (isDoctor)
        {
            ((Doctor)employee).Registration = value.GetValueOrDefault("licence_number")?.ToString()!;
        }

        return employee;
    }
}
