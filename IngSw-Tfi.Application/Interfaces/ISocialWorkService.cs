using IngSw_Tfi.Application.DTOs;

namespace IngSw_Tfi.Application.Interfaces;

public interface ISocialWorkService
{
    Task<List<SocialWorkDto.Response>> GetAll();
    Task<bool> ValidateInsuranceAndMember(SocialWorkDto.Validate socialData);
}
