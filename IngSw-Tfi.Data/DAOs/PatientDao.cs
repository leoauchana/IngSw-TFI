using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Domain.Entities;
using MySql.Data.MySqlClient;
using System.Data;

namespace IngSw_Tfi.Data.DAOs;

public class PatientDao : DaoBase
{
    public PatientDao(SqlConnection connection) : base(connection) { }

    public async Task AddPatient(Patient newPatient)
    {
        var query = """
            INSERT INTO patient (id_patient, health_insurance_id, patient_cuil, first_name, last_name, email, phone, birth_date, street_address, number_address, town_address) 
            VALUES (@Id, @HealthInsuranceId, @Cuil, @FirstName, @LastName, @Email, @Phone, @BirthDate, @Street, @Number, @Locality)
            """;
        var parameters = new[]{
            new MySqlParameter("@Id", newPatient.Id?.ToString() ?? Guid.NewGuid().ToString()),
            new MySqlParameter("@HealthInsuranceId", DBNull.Value),
            new MySqlParameter("@Cuil", newPatient.Cuil?.Value ?? string.Empty),
            new MySqlParameter("@FirstName", newPatient.Name ?? string.Empty),
            new MySqlParameter("@LastName", newPatient.LastName ?? string.Empty),
            new MySqlParameter("@Email", newPatient.Email ?? string.Empty),
            new MySqlParameter("@Phone", (object?)newPatient.Phone ?? DBNull.Value),
            new MySqlParameter("@BirthDate", newPatient.BirthDate),
            new MySqlParameter("@Street", newPatient.Domicilie!.Street),
            new MySqlParameter("@Number", newPatient.Domicilie.Number),
            new MySqlParameter("@Locality", newPatient.Domicilie.Locality)
        };
        await ExecuteNonQuery(query, parameters);
    }
    
    public async Task<List<Dictionary<string, object>>?> GetByCuil(string cuilPatient)
    {
        var query = "SELECT * FROM patient WHERE patient_cuil LIKE @CuilPatient";
        var parameters = new MySqlParameter("@CuilPatient", cuilPatient + "%");
        return await ExecuteReader(query, parameters);
    }
    public async Task<List<Dictionary<string, object>>?> GetAll()
    {
        var query = "SELECT * FROM patient";
        return await ExecuteReader(query);
    }

    public async Task<Dictionary<string, object>?> GetById(Guid id)
    {
        var query = "SELECT * FROM patient WHERE id_patient = @Id LIMIT 1";
        var param = new MySqlParameter("@Id", id.ToString());
        var result = await ExecuteReader(query, param);
        return result?.FirstOrDefault();
    }
    // Variants using existing connection/transaction
    public async Task<List<Dictionary<string, object>>?> GetByCuil(string cuilPatient, System.Data.IDbConnection conn, System.Data.IDbTransaction? tx)
    {
        var query = "SELECT * FROM patient WHERE patient_cuil LIKE @CuilPatient";
        var parameters = new MySqlParameter("@CuilPatient", cuilPatient + "%");
        return await ExecuteReader(query, conn, tx, parameters);
    }
    public async Task<Dictionary<string, object>?> GetById(Guid id, System.Data.IDbConnection conn, System.Data.IDbTransaction? tx)
    {
        var query = "SELECT * FROM patient WHERE id_patient = @Id LIMIT 1";
        var param = new MySqlParameter("@Id", id.ToString());
        var result = await ExecuteReader(query, conn, tx, param);
        return result?.FirstOrDefault();
    }
    public async Task AddPatient(Patient newPatient, System.Data.IDbConnection conn, System.Data.IDbTransaction? tx)
    {
        var query = """
            INSERT INTO patient (id_patient, health_insurance_id, patient_cuil, first_name, last_name, email, phone, birth_date, street_address, number_address, town_address) 
            VALUES (@Id, @HealthInsuranceId, @Cuil, @FirstName, @LastName, @Email, @Phone, @BirthDate, @Street, @Number, @Locality)
            """;
        var parameters = new[]{
            new MySqlParameter("@Id", newPatient.Id?.ToString() ?? Guid.NewGuid().ToString()),
            new MySqlParameter("@HealthInsuranceId", DBNull.Value),
            new MySqlParameter("@Cuil", newPatient.Cuil?.Value ?? string.Empty),
            new MySqlParameter("@FirstName", newPatient.Name ?? string.Empty),
            new MySqlParameter("@LastName", newPatient.LastName ?? string.Empty),
            new MySqlParameter("@Email", newPatient.Email ?? string.Empty),
            new MySqlParameter("@Phone", (object?)newPatient.Phone ?? DBNull.Value),
            new MySqlParameter("@BirthDate", newPatient.BirthDate),
            new MySqlParameter("@Street", newPatient.Domicilie!.Street),
            new MySqlParameter("@Number", newPatient.Domicilie.Number),
            new MySqlParameter("@Locality", newPatient.Domicilie.Locality)
        };
        await ExecuteNonQuery(query, conn, tx, parameters);
    }
}
