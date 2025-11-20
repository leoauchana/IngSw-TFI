using IngSw_Tfi.Domain.Interfaces;

namespace IngSw_Tfi.Transversal.Services;

public class SocialWorkServiceApi : ISocialWorkServiceApi
{
    public async Task<bool> ExistingSocialWork(string socialWork)
    {
        // TODO: Implementar lógica para verificar si la obra social existe
        // Por ahora retornamos true como placeholder
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> IsAffiliated(string affiliateNumber)
    {
        // TODO: Implementar lógica para verificar si el afiliado existe
        // Por ahora retornamos true como placeholder
        await Task.CompletedTask;
        return true;
    }
}
