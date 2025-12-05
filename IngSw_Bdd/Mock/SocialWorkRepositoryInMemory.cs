using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Bdd.Mock;

public class SocialWorkRepositoryInMemory : ISocialWorkRepository
{
    public List<SocialWork> SocialWorks { get; set; }
    public SocialWorkRepositoryInMemory()
    {
        SocialWorks = new List<SocialWork>();
    }
    public void AddSocialWork(SocialWork newSocialWork)
    {
        SocialWorks.Add(newSocialWork);
    }
    public Task<List<SocialWork>?> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<SocialWork?> GetById(string idSocialWork)
    {
        var socialWork = SocialWorks.FirstOrDefault(i => i.Id.ToString() == idSocialWork);
        return Task.FromResult(socialWork);
    }

    public Task<bool> ValidateInsuranceAndMember(string nameSocial, int memerNumber)
    {
        throw new NotImplementedException();
    }
}
