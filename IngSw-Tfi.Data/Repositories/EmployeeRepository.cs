using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Tfi.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeDao _employeeDao;

    public EmployeeRepository(EmployeeDao employeeDao)
    {
        _employeeDao = employeeDao;
    }

    public async Task<Employee?> GetByEmail(string userEmail)
    {
        var record = await _employeeDao.GetByEmail(userEmail);
        if (record == null) return null;

        var employee = new Employee();

        if (record.TryGetValue("idusuario", out var idObj) && Guid.TryParse(Convert.ToString(idObj), out var guid))
            employee.Id = guid;

        employee.Email = Convert.ToString(record.GetValueOrDefault("email"));

        var firstName = record.GetValueOrDefault("doctor_first_name") ?? record.GetValueOrDefault("nurse_first_name");
        var lastName = record.GetValueOrDefault("doctor_last_name") ?? record.GetValueOrDefault("nurse_last_name");
        var phone = record.GetValueOrDefault("doctor_phone") ?? record.GetValueOrDefault("nurse_phone");
        var registration = record.GetValueOrDefault("doctor_license") ?? string.Empty;
        var cuil = record.GetValueOrDefault("doctor_cuil") ?? string.Empty;

        employee.Name = Convert.ToString(firstName);
        employee.LastName = Convert.ToString(lastName);
        employee.PhoneNumber = Convert.ToString(phone);
        employee.Registration = Convert.ToString(registration);
        employee.Cuil = !string.IsNullOrEmpty(Convert.ToString(cuil)) ? IngSw_Tfi.Domain.ValueObjects.Cuil.Create(Convert.ToString(cuil)!) : null;

        employee.User = new User
        {
            Email = Convert.ToString(record.GetValueOrDefault("email")),
            Password = Convert.ToString(record.GetValueOrDefault("password"))
        };

        return employee;
    }

    public Task<Employee?> Register(Employee employee)
    {
        throw new NotImplementedException();
    }
}
