using MySql.Data.MySqlClient;
using System.Data;

namespace IngSw_Tfi.Data.Database;

public class SqlConnection
{
    public class SqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
