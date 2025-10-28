﻿using IngSw_Tfi.Data.Database;
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
                var row = new Dictionary<string, object>(reader.FieldCount);
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);

                result.Add(row);
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
}