using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Domain.Repository;

public interface IHealthInsuranceRepository
{
    Task<List<SocialWork>> GetAll();
}
