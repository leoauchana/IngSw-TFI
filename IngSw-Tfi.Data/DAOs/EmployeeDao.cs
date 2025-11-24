using IngSw_Tfi.Data.Database;
using MySql.Data.MySqlClient;

namespace IngSw_Tfi.Data.DAOs;

public class EmployeeDao : DaoBase
{
    public EmployeeDao(SqlConnection connection) : base(connection)
    {
    }
    public async Task<List<Dictionary<string, object>>?> GetByEmailNurse(string email)
    {
        var query = @"
        SELECT * 
        FROM nurse n WHERE n.email = @Email";

        var param = new MySqlParameter("@Email", email);

        return await ExecuteReader(query, param);
    }

    public async Task<List<Dictionary<string, object>>?> GetByEmailDoctor(string email)
    {
        var query = @"
        SELECT * 
        FROM doctor d WHERE d.email = @Email";

        var param = new MySqlParameter("@Email", email);

        return await ExecuteReader(query, param);
    }

}
