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
            new MySqlParameter("@Id", newPatient.Id.ToString() ?? Guid.NewGuid().ToString()),
            new MySqlParameter("@HealthInsuranceId", newPatient.Affiliate?.SocialWork?.Id ?? (object)DBNull.Value),
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

    public async Task AddPatientWithSocialWork(Patient newPatient)
    {
        await ExecuteInTransaction(async (conn, trans) =>
        {
            // 1) Insertar en health_insurance
            var hiQuery = """
            INSERT INTO health_insurance (id_health_insurance, id_socialWork, memberNumber)
            VALUES (@HealthInsuranceId, @SocialWorkId, @AffiliateNumber)
        """;
            var idHealthInsurance = Guid.NewGuid();
            using var hiCmd = new MySqlCommand(hiQuery, conn, trans);
            hiCmd.Parameters.AddWithValue("@HealthInsuranceId", idHealthInsurance.ToString());
            hiCmd.Parameters.AddWithValue("@SocialWorkId", newPatient.Affiliate!.SocialWork!.Id);
            hiCmd.Parameters.AddWithValue("@AffiliateNumber", newPatient.Affiliate!.AffiliateNumber);
            await hiCmd.ExecuteNonQueryAsync();

            // 2) Insertar en patient
            var patientQuery = """
            INSERT INTO patient
            (id_patient, health_insurance_id, patient_cuil, first_name, last_name, email, phone, birth_date, street_address, number_address, town_address)
            VALUES (@Id, @HealthInsuranceId, @Cuil, @FirstName, @LastName, @Email, @Phone, @BirthDate, @Street, @Number, @Locality)
        """;

            using var patientCmd = new MySqlCommand(patientQuery, conn, trans);
            patientCmd.Parameters.AddWithValue("@Id", newPatient.Id.ToString());
            patientCmd.Parameters.AddWithValue("@HealthInsuranceId", idHealthInsurance.ToString());
            patientCmd.Parameters.AddWithValue("@Cuil", newPatient.Cuil?.Value ?? "");
            patientCmd.Parameters.AddWithValue("@FirstName", newPatient.Name ?? "");
            patientCmd.Parameters.AddWithValue("@LastName", newPatient.LastName ?? "");
            patientCmd.Parameters.AddWithValue("@Email", newPatient.Email ?? "");
            patientCmd.Parameters.AddWithValue("@Phone", newPatient.Phone ?? (object)DBNull.Value);
            patientCmd.Parameters.AddWithValue("@BirthDate", newPatient.BirthDate);
            patientCmd.Parameters.AddWithValue("@Street", newPatient.Domicilie!.Street);
            patientCmd.Parameters.AddWithValue("@Number", newPatient.Domicilie.Number);
            patientCmd.Parameters.AddWithValue("@Locality", newPatient.Domicilie.Locality);

            await patientCmd.ExecuteNonQueryAsync();
        });
    }


    public async Task<List<Dictionary<string, object>>?> GetByCuil(string cuilPatient)
    {
        var query = "SELECT * FROM patient WHERE patient_cuil LIKE @CuilPatient";
        var parameters = new MySqlParameter("@CuilPatient", cuilPatient + "%");
        return await ExecuteReader(query, parameters);
    }
    public async Task<List<Dictionary<string, object>>?> GetAll()
    {
        var query = """
            SELECT
                p.id_patient,
                p.first_name,
                p.last_name,
                p.patient_cuil,
                p.email,
                p.phone,
                p.birth_date,
                p.street_address,
                p.number_address,
                p.town_address,
                h.memberNumber,
                sw.id_socialWork,
                sw.name AS socialWork_name
            FROM patient p
            LEFT JOIN health_insurance h 
                   ON p.health_insurance_id = h.id_health_insurance
            LEFT JOIN socialWork sw 
                   ON h.id_socialWork = sw.id_socialWork;
            """;
        return await ExecuteReader(query);
    }

    public async Task<Dictionary<string, object>?> GetById(Guid id)
    {
        var query = """
                    SELECT 
                        p.id_patient,
                        p.first_name,
                        p.last_name,
                        p.patient_cuil,
                        p.email,
                        p.phone,
                        p.birth_date,
                        p.street_address,
                        p.number_address,
                        p.town_address,
                        h.memberNumber,
                        sw.id_socialWork,
                        sw.name AS socialWork_name
                    FROM patient p
                    LEFT JOIN health_insurance h 
                           ON p.health_insurance_id = h.id_health_insurance
                    LEFT JOIN socialWork sw 
                           ON h.id_socialWork = sw.id_socialWork
                    WHERE p.id_patient = @Id
                    LIMIT 1;
                    """;
        var param = new MySqlParameter("@Id", id.ToString());
        var result = await ExecuteReader(query, param);
        return result?.FirstOrDefault();
    }
}
