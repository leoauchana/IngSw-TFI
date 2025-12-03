using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Tfi.Data.Repositories;

public class AttentionRepository : IAttentionRepository
{
    private readonly AttentionDao _attentionDao;
    public AttentionRepository(AttentionDao attentionDao)
    {
        _attentionDao = attentionDao;
    }
    public async Task AddAttention(Attention newAttention) => await _attentionDao.AddAttention(newAttention);
}
