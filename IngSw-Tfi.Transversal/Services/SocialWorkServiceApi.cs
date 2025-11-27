using IngSw_Tfi.Domain.Interfaces;

namespace IngSw_Tfi.Transversal.Services;

public class SocialWorkServiceApi : ISocialWorkServiceApi
{
    // Implementación simple de desarrollo: asumir que la obra social existe y el número de afiliado es válido.
    public Task<bool> ExistingSocialWork(string socialWork)
    {
        return Task.FromResult(true);
    }

    public Task<bool> IsAffiliated(string affiliateNumber)
    {
        return Task.FromResult(true);
    }
}
