using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Domain.Common;
using MySql.Data.MySqlClient;
using System.Data;

namespace IngSw_Tfi.Data.DAOs;

public abstract class DaoBase
{
    private readonly SqlConnection _connection;
    protected DaoBase(SqlConnection connection)
    {
        _connection = connection;
    }
    protected async Task<List<IDataRecord>> ExecuteReader(string query, params MySqlParameter[] parameters)
    {
        var result = new List<IDataRecord>();

        using (var conn = _connection.CreateConnection())
        using (var cmd = new MySqlCommand(query, (MySqlConnection)conn))
        {
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            conn.Open();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while ( await reader.ReadAsync())
                    result.Add(reader);
            }
        }

        return result;
    }
    protected async Task<int> ExecuteNonQuery(string query, params MySqlParameter[] parameters)
    {
        using (var conn = _connection.CreateConnection())
        using (var cmd = new MySqlCommand(query, (MySqlConnection)conn))
        {
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}