using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Data.Repositories;
using IngSw_Tfi.Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IngSw_Tfi.Data;

public static class ServiceExtensions
{
    public static void AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new SqlConnection(configuration.GetConnectionString("MySqlDb")!));
        services.AddScoped<IIncomeRepository, IncomeRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IncomeDao>();
        services.AddScoped<PatientDao>();
        services.AddScoped<EmployeeDao>();
    }
}
