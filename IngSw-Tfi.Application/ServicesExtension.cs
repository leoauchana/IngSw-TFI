using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IngSw_Tfi.Application;

public static class ServicesExtension
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IIncomesService, IncomesService>();
        services.AddScoped<IPatientsService, PatientsService>();
        services.AddScoped<IAuthService, AuthService>();
    }
}
