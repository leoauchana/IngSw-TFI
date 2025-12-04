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
        var employeeFound = await _employeeDao.GetByEmail(userEmail);
        if (employeeFound == null) return null;
        return MapEntity(employeeFound);
    }
    public async Task<Employee?> GetById(string idEmployee)
    {
        var employeeFound = await _employeeDao.GetById(idEmployee);
        if (employeeFound == null) return null;
        return MapEntity(employeeFound);
    }
    private Employee MapEntity(Dictionary<string, object> record)
    {
        var idNurse = record.GetValueOrDefault("id_nurse");
        var idDoctor = record.GetValueOrDefault("id_doctor");

        Employee employee;
        string? targetId = null;

        if (idNurse != null && !string.IsNullOrEmpty(idNurse.ToString()))
        {
            employee = new Nurse();
            targetId = idNurse.ToString();
        }
        else if (idDoctor != null && !string.IsNullOrEmpty(idDoctor.ToString()))
        {
            employee = new Doctor();
            targetId = idDoctor.ToString();
        }
        else
        {
            employee = new Employee();
        }
        employee.Id = Guid.Parse(targetId!);

        employee.Email = Convert.ToString(record.GetValueOrDefault("email"));

        var firstName = record.GetValueOrDefault("doctor_first_name") ?? record.GetValueOrDefault("nurse_first_name");
        var lastName = record.GetValueOrDefault("doctor_last_name") ?? record.GetValueOrDefault("nurse_last_name");
        var phone = record.GetValueOrDefault("doctor_phone") ?? record.GetValueOrDefault("nurse_phone");
        var registration = record.GetValueOrDefault("doctor_license") ?? string.Empty;
        var cuil = record.GetValueOrDefault("doctor_cuil") ?? string.Empty;

        employee.Name = Convert.ToString(firstName);
        employee.LastName = Convert.ToString(lastName);
        employee.PhoneNumber = Convert.ToString(phone);
        employee.Registration = Convert.ToString(registration) ?? string.Empty;
        employee.Cuil = !string.IsNullOrEmpty(Convert.ToString(cuil)) ? IngSw_Tfi.Domain.ValueObjects.Cuil.Create(Convert.ToString(cuil)!) : null;

        var user = new User();

        if (record.TryGetValue("idusuario", out var idObj))
        {
            targetId = idObj?.ToString();
        }
        if (targetId != null)
        {
            user.Id = Guid.Parse(targetId);
        }

        user.Email = Convert.ToString(record.GetValueOrDefault("email"));
        user.Password = Convert.ToString(record.GetValueOrDefault("password"));

        employee.User = user;
        return employee;
    }
    public Task<Employee?> Register(Employee employee)
    {
        throw new NotImplementedException();
    }
}
