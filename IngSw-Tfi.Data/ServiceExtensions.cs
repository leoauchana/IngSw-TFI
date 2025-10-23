using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Data.Repositories;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IngSw_Tfi.Data;

public static class ServiceExtensions
{
    public static void AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new SqlConnection(configuration.GetConnectionString("DbMySql")!));
        services.AddScoped<IRepository<Income>, IncomeRepository>();
    }
}
