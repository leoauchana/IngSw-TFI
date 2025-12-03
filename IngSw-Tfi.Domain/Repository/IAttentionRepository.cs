using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Domain.Repository;

public interface IAttentionRepository
{
    Task AddAttention(Attention newAttention);
    Task<List<Attention>?> GetAll();
}
