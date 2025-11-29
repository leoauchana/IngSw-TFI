using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Interfaces;
using IngSw_Tfi.Transversal.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IngSw_Tfi.Transversal;

public static class ServiceExtensions
{
    public static void AddTransversalServices(this IServiceCollection services)
    {
        services.AddScoped<ISocialWorkServiceApi, SocialWorkServiceApi>();
    }
}
