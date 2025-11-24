using IngSw_Tfi.Domain.Interfaces;

namespace IngSw_Tfi.Transversal.Services;

public class SocialWorkServiceApi : ISocialWorkServiceApi
{
    public Task<bool> ExistingSocialWork(string socialWork)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsAffiliated(string affiliateNumber)
    {
        throw new NotImplementedException();
    }
}
