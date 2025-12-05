using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Domain.Repository;

public interface ISocialWorkRepository
{
    Task<List<SocialWork>?> GetAll();
    Task<bool> ValidateInsuranceAndMember(string nameSocial, int memerNumber);
    void AddSocialWork(SocialWork newSocialWork);
    Task<SocialWork?> GetById(string idSocialWork);
}
