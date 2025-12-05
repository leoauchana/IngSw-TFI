using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Domain.Repository;

public interface ISocialWorkRepository
{
    Task<List<SocialWork>?> GetAll();
    void AddSocialWork(SocialWork newSocialWork);
    bool ValidateInsuranceAndMember(string nameSocial, int memerNumber);
    Task<SocialWork?> GetById(string idSocialWork);
}
