using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Domain.Interfaces;

public interface ISocialWorkServiceApi
{
    Task<SocialWork?> ExistingSocialWork(string idSocialWork);
    Task<bool> IsAffiliated(string affiliateNumber);
}
