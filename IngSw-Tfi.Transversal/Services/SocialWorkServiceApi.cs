using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Interfaces;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Tfi.Transversal.Services;

public class SocialWorkServiceApi : ISocialWorkServiceApi
{
    private readonly ISocialWorkRepository _socialWorkRepository;
    // Implementación simple de desarrollo: asumir que la obra social existe y el número de afiliado es válido.
    public SocialWorkServiceApi(ISocialWorkRepository socialWorkRepository)
    {
        _socialWorkRepository = socialWorkRepository;
    }
    public async Task<SocialWork?> ExistingSocialWork(string idSocialWork)
    {
        var socialWorkFound = await _socialWorkRepository.GetById(idSocialWork);
        if (socialWorkFound == null) return null;
        return socialWorkFound;
    }

    public Task<bool> IsAffiliated(string affiliateNumber)
    {
        return Task.FromResult(true);
    }
}
