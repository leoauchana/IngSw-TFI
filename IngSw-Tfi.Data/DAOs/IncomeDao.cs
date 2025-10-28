using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Domain.Entities;
using MySql.Data.MySqlClient;

namespace IngSw_Tfi.Data.DAOs;

public class IncomeDao : DaoBase
{
    public IncomeDao(SqlConnection connection) : base(connection) { }
    public async Task<Dictionary<string, object>?> GetById(int idIncome)
    {
        var query = """
            SELECT * FROM incomes i
            INNER JOIN patient p ON i.patient_id = p.patient_id
            INNER JOIN socialWork sw ON i.socialwork_id = sw.socialwork_id
            WHERE i.id = @IdIncome;
            """;
        var param = new MySqlParameter("@Id", idIncome);
        var income = await ExecuteReader(query, param);
        return income.FirstOrDefault();
    }
    public async Task<List<Dictionary<string, object>>> GetAll()
    {
        var query = """
            SELECT * FROM incomes i
            INNER JOIN patient p ON i.patient_id = p.patient_id
            INNER JOIN socialWork sw ON i.socialwork_id = sw.socialwork_id;
            """;
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
