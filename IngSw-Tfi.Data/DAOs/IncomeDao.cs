using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Domain.Entities;
using MySql.Data.MySqlClient;
using System.Data;

namespace IngSw_Tfi.Data.DAOs;

public class IncomeDao : DaoBase
{
    public IncomeDao(SqlConnection connection) : base(connection) { }
    public async Task<IDataRecord?> GetById(int idIncome)
    {
        var query = "SELECT * FROM incomes where id=@IdIncome";
        var param = new MySqlParameter("@Id", idIncome);
        var income = await ExecuteReader(query, param);
        return income.FirstOrDefault();
    }
    public async Task<List<IDataRecord>?> GetAll()
    {
        var query = "SELECT * FROM incomes";
        return await ExecuteReader(query);
    }
    public async Task AddIncome(Income newIncome)
    {
        var query = "INSERT INTO incomes () VALUES ()";
        var parameters = new[]{
            new MySqlParameter(),
            new MySqlParameter()
        };
        await ExecuteNonQuery(query, parameters); 
    }
}
