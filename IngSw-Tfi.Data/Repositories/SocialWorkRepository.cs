using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Tfi.Data.Repositories;

public class SocialWorkRepository : ISocialWorkRepository
{
    private readonly SocialWorkDao _socialWorkDao;

    public SocialWorkRepository(SocialWorkDao socialWorkDao)
    {
        _socialWorkDao = socialWorkDao;
    }

    public void AddSocialWork(SocialWork newSocialWork)
    {
        throw new NotImplementedException();
    }

    public async Task<List<SocialWork>?> GetAll()
    {
        var socialList = await _socialWorkDao.GetAll();
        if (socialList == null) return null;
        return socialList.Select(s => MapEntity(s)).ToList();
    }

    public async Task<SocialWork?> GetById(string idSocialWork)
    {
        var social = await _socialWorkDao.GetById(idSocialWork);
        if (social == null || !social.Any()) return null;
        return MapEntity(social.First());
    }

    public bool ValidateInsuranceAndMember(string name, string? memberNumber)
    {
        return true;
    }

    public async Task<SocialWork?> ExistingSocialWork(string idSocialWork)
    {
        var social = await _socialWorkDao.GetById(idSocialWork);
        if (social == null || !social.Any()) return null;
        return MapEntity(social.First());
    }

    private SocialWork MapEntity(Dictionary<string, object> value)
    {
        return new SocialWork
        {
            Id = value.ContainsKey("id_socialWork") && Guid.TryParse(value["id_socialWork"]?.ToString(), out var pid) ? pid : Guid.Empty,
            Name = value.GetValueOrDefault("name")!.ToString()
        };
    }
}
