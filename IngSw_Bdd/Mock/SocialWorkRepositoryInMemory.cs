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

    public Task<SocialWork?> ExistingSocialWork(string idSocialWork)
    {
        var socialWork = SocialWorks.FirstOrDefault(i => i.Id.ToString() == idSocialWork);
        return Task.FromResult(socialWork);
    }

    public Task<SocialWork?> GetById(string idSocialWork)
    {
        var socialWork = SocialWorks.FirstOrDefault(i => i.Id.ToString() == idSocialWork);
        return Task.FromResult(socialWork);
    }

    public bool ValidateInsuranceAndMember(string nameSocial, string? memerNumber)
    {
        return true;
    }
}
