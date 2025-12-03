using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Domain.Entities;
using MySql.Data.MySqlClient;

namespace IngSw_Tfi.Data.DAOs;

public class SocialWorkDao : DaoBase
{
    public SocialWorkDao(SqlConnection connection) : base(connection)
    {
    }
    public async Task<List<Dictionary<string, object>>?> GetAll()
    {
        string query = "select * from socialWork";
        return await ExecuteReader(query);
    }
    public async Task<List<Dictionary<string, object>>?> GetById(string idSocialWork)
    {
        string query = "select * from socialWork where id_socialWork = @IdSocialWork";
        var parameters = new[]
        {
            new MySqlParameter("@IdSocialWork", idSocialWork)
        };
        return await ExecuteReader(query, parameters);
    }
    public async Task<Dictionary<string, object>> GetByNameAndMemberNumber(string nameSocial, int memberNumber)
    {
        throw new NotImplementedException();
    }
}
