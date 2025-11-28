using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Data.Repositories;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IngSw_Tfi.Data.DAOs;

namespace IngSw_Tfi.Data;

public static class ServiceExtensions
{
    public static void AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new SqlConnection(configuration.GetConnectionString("DbMySql")!));
        services.AddScoped<IIncomeRepository, IncomeRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        // Register DAOs so repositories can be resolved
        services.AddScoped<IncomeDao>();
        services.AddScoped<PatientDao>();
        services.AddScoped<EmployeeDao>();
        services.AddScoped<HealthInsuranceDao>();
        // Employee repository (used by AuthService) - basic implementation exists
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    }
}
