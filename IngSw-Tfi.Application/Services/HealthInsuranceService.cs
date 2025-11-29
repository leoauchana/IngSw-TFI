using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Tfi.Application.Services;

public class HealthInsuranceService : IHealthInsuranceService
{
    private readonly IHealthInsuranceRepository _healthInsuranceRepository;
	public HealthInsuranceService(IHealthInsuranceRepository healthInsuranceRepository)
	{
		_healthInsuranceRepository = healthInsuranceRepository;
	}
	public async Task<List<SocialWorkDto.Response>> GetAll()
    {
		var list = await _healthInsuranceRepository.GetAll();
		if (list == null || !list.Any()) throw new NullException("No hay obras sociales registradas.");
		return list.Select(h => new SocialWorkDto.Response(h.Id, h.Name!)).ToList();
    }

    public Task<bool> ValidateInsuranceAndMember()
    {
        throw new NotImplementedException();
    }
}
