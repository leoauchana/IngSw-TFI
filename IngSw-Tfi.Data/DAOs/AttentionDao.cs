using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Data.DAOs;

public class AttentionDao : DaoBase
{
    public AttentionDao(SqlConnection connection) : base(connection)
    {
    }

    public Task<Dictionary<string,object>?> AddAttention(Attention newAttention)
    {
        throw new NotImplementedException();
    }
}
