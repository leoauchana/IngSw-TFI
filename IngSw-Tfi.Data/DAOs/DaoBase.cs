using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Domain.Common;
using MySql.Data.MySqlClient;
using System.Data;

namespace IngSw_Tfi.Data.DAOs;

public abstract class DaoBase
{
    protected readonly SqlConnection _connection;
    protected DaoBase(SqlConnection connection)
    {
        _connection = connection;
    }
    protected async Task<List<Dictionary<string, object>>?> ExecuteReader(string query, params MySqlParameter[] parameters)
    {
        var result = new List<Dictionary<string, object>>();

        using (var conn = _connection.CreateConnection())
        using (var cmd = new MySqlCommand(query, (MySqlConnection)conn))
        {
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            conn.Open();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>(reader.FieldCount);
                    for (int i = 0; i < reader.FieldCount; i++)
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    result.Add(row);
                }
            }
        }

        return result.Count > 0 ? result : null;
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
    protected async Task ExecuteInTransaction(Func<MySqlConnection, MySqlTransaction, Task> action)
    {
        using var conn = _connection.CreateConnection();
        await conn.OpenAsync();

        using var trans = await conn.BeginTransactionAsync();

        try
        {
            await action((MySqlConnection)conn, (MySqlTransaction)trans);
            await trans.CommitAsync();
        }
        catch
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    //// Overloads that use an existing connection and transaction (do not dispose connection)
    //protected async Task<List<Dictionary<string, object>>?> ExecuteReader(IDbConnection conn, IDbTransaction tx, string query, params MySqlParameter[] parameters)
    //{
    //    var result = new List<Dictionary<string, object>>();
    //    using (var cmd = new MySqlCommand(query, (MySqlConnection)conn))
    //    {
    //        if (tx != null)
    //            cmd.Transaction = (MySqlTransaction)tx;
    //        if (parameters != null && parameters.Length > 0)
    //            cmd.Parameters.AddRange(parameters);

    //        using (var reader = await cmd.ExecuteReaderAsync())
    //        {
    //            while (await reader.ReadAsync())
    //            {
    //                var row = new Dictionary<string, object>(reader.FieldCount);
    //                for (int i = 0; i < reader.FieldCount; i++)
    //                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
    //                result.Add(row);
    //            }
    //        }
    //    }
    //    return result.Count > 0 ? result : null;
    //}

    //protected async Task<int> ExecuteNonQuery(IDbConnection conn, IDbTransaction tx, string query, params MySqlParameter[] parameters)
    //{
    //    using (var cmd = new MySqlCommand(query, (MySqlConnection)conn))
    //    {
    //        if (tx != null)
    //            cmd.Transaction = (MySqlTransaction)tx;
    //        if (parameters != null && parameters.Length > 0)
    //            cmd.Parameters.AddRange(parameters);
    //        return await cmd.ExecuteNonQueryAsync();
    //    }
    //}
    //// Overloads that use an existing connection/transaction (caller manages transaction/connection lifetime)
    //protected async Task<List<Dictionary<string, object>>?> ExecuteReader(string query, IDbConnection conn, IDbTransaction? tx, params MySqlParameter[] parameters)
    //{
    //    var result = new List<Dictionary<string, object>>();
    //    var mysqlConn = (MySqlConnection)conn;
    //    using (var cmd = new MySqlCommand(query, mysqlConn))
    //    {
    //        if (parameters != null && parameters.Length > 0)
    //            cmd.Parameters.AddRange(parameters);
    //        if (tx != null)
    //            cmd.Transaction = (MySqlTransaction)tx;

    //        var mustClose = conn.State != ConnectionState.Open;
    //        if (mustClose) conn.Open();
    //        using (var reader = await cmd.ExecuteReaderAsync())
    //        {
    //            while (await reader.ReadAsync())
    //            {
    //                var row = new Dictionary<string, object>(reader.FieldCount);
    //                for (int i = 0; i < reader.FieldCount; i++)
    //                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
    //                result.Add(row);
    //            }
    //        }
    //        if (mustClose) conn.Close();
    //    }
    //    return result.Count > 0 ? result : null;
    //}
    //protected async Task<int> ExecuteNonQuery(string query, IDbConnection conn, IDbTransaction? tx, params MySqlParameter[] parameters)
    //{
    //    var mysqlConn = (MySqlConnection)conn;
    //    using (var cmd = new MySqlCommand(query, mysqlConn))
    //    {
    //        if (parameters != null && parameters.Length > 0)
    //            cmd.Parameters.AddRange(parameters);
    //        if (tx != null)
    //            cmd.Transaction = (MySqlTransaction)tx;

    //        var mustClose = conn.State != ConnectionState.Open;
    //        if (mustClose) conn.Open();
    //        var res = await cmd.ExecuteNonQueryAsync();
    //        if (mustClose) conn.Close();
    //        return res;
    //    }
    //}
}