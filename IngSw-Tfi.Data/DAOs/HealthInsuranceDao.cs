using IngSw_Tfi.Data.Database;
using MySql.Data.MySqlClient;

namespace IngSw_Tfi.Data.DAOs;

public class HealthInsuranceDao : DaoBase
{
    public HealthInsuranceDao(SqlConnection connection) : base(connection) { }

    public async Task<List<Dictionary<string, object>>> GetAll()
    {
        var query = @"
            SELECT 
                id_health_insurance,
                name,
                member_number
            FROM health_insurance
            ORDER BY name;
        ";
        return await ExecuteReader(query) ?? new List<Dictionary<string, object>>();
    }

    public async Task<Dictionary<string, object>?> GetByNameAndMemberNumber(string name, int memberNumber)
    {
        var query = @"
            SELECT 
                id_health_insurance,
                name,
                member_number
            FROM health_insurance
            WHERE name = @Name AND member_number = @MemberNumber
            LIMIT 1;
        ";
        var parameters = new[]
        {
            new MySqlParameter("@Name", name),
            new MySqlParameter("@MemberNumber", memberNumber)
        };
        var results = await ExecuteReader(query, parameters);
        return results?.FirstOrDefault();
    }
}
