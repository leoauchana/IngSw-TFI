using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Tfi.Application.Services;

public class SocialWorkService : ISocialWorkService
{
    private readonly ISocialWorkRepository _socialWorkRepository;
    public SocialWorkService(ISocialWorkRepository socialWorkRepository)
    {
        _socialWorkRepository = socialWorkRepository;
    }
    public async Task<List<SocialWorkDto.Response>> GetAll()
    {
        var list = await _socialWorkRepository.GetAll();
        if (list == null || !list.Any()) throw new NullException("No hay obras sociales registradas.");
        return list.Select(h => new SocialWorkDto.Response(h.Id, h.Name!)).ToList();
    }
    public bool ValidateInsuranceAndMember(SocialWorkDto.Validate socialData) => 
        _socialWorkRepository.ValidateInsuranceAndMember(socialData.name, socialData.memberNumber);
    
}
