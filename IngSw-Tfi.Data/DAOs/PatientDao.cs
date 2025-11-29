using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Domain.Entities;
using MySql.Data.MySqlClient;

namespace IngSw_Tfi.Data.DAOs;

public class PatientDao : DaoBase
{
    public PatientDao(SqlConnection connection) : base(connection) { }

    public async Task AddPatient(Patient newPatient)
    {
        var query = """
            INSERT INTO patient (id_patient, health_insurance_id, first_name, last_name, patient_cuil, email, street_address, number_address, town_address) 
            VALUES (@Id, @IdHealth, @Name, @LastName, @Cuil, @Email, @Street, @Number, @Locality)
            """;
        var parameters = new[]{
            new MySqlParameter("@Id", newPatient.Id),
            new MySqlParameter("@IdHealth", newPatient.Affiliate?.Id ?? (object)DBNull.Value),
            new MySqlParameter("@Name", newPatient.Name),
            new MySqlParameter("@LastName", newPatient.LastName),
            new MySqlParameter("@Cuil", newPatient.Cuil!.Value),
            new MySqlParameter("@Email", newPatient.Email),
            new MySqlParameter("@Street", newPatient.Domicilie!.Street),
            new MySqlParameter("@Number", newPatient.Domicilie.Number),
            new MySqlParameter("@Locality", newPatient.Domicilie.Locality)
        };
        await ExecuteNonQuery(query, parameters);
    }
    public async Task<List<Dictionary<string, object>>?> GetAll()
    {
        var query = "SELECT * FROM patient";
        return await ExecuteReader(query);
    }
    public async Task<List<Dictionary<string, object>>?> GetByCuil(string cuilPatient)
    {
        var query = "SELECT * FROM patient WHERE patient_cuil LIKE @CuilPatient";
        var parameters = new MySqlParameter("@CuilPatient", cuilPatient + "%");
        return await ExecuteReader(query, parameters);
    }
}
