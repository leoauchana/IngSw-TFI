using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Domain.Entities;
using MySql.Data.MySqlClient;

namespace IngSw_Tfi.Data.DAOs;

public class AttentionDao : DaoBase
{
    public AttentionDao(SqlConnection connection) : base(connection)
    {
    }
    public async Task AddAttention(Attention newAttention)
    {
        // 1) Buscar si ya existe una atención para esa admisión
        var checkQuery = @"
            SELECT id_consultation
            FROM consultation
            WHERE admission_id_admission = @AdmissionId;
        ";

        var checkParam = new MySqlParameter("@AdmissionId", newAttention.Income!.Id.ToString());
        var existing = await ExecuteReader(checkQuery, checkParam);

        // 2) Si ya existe → actualizar el reporte
        if (existing != null && existing.Count > 0)
        {
            string consultationId = existing[0]["id_consultation"].ToString();

            var updateQuery = @"
            UPDATE consultation
            SET 
                report = @Report,
                doctor_id_doctor = @DoctorId
            WHERE id_consultation = @Id;
        ";

            var updateParams = new[]
            {
            new MySqlParameter("@Report", newAttention.Report ?? string.Empty),
            new MySqlParameter("@DoctorId", newAttention.Doctor!.Id.ToString()),
            new MySqlParameter("@Id", consultationId),
        };

            await ExecuteNonQuery(updateQuery, updateParams);
            return;
        }

        // 3) Si no existe → crear una nueva atención
        var query = """
            INSERT INTO consultation (id_consultation, doctor_id_doctor, admission_id_admission, report) 
            VALUES (@Id, @DoctorId, @AdmissionId, @Report)
            """;
        var parameters = new[]{
            new MySqlParameter("@Id", newAttention.Id.ToString() ?? Guid.NewGuid().ToString()),
            new MySqlParameter("@DoctorId", newAttention.Doctor!.Id.ToString() ?? (object)DBNull.Value),
            new MySqlParameter("@AdmissionId", newAttention.Income!.Id.ToString() ?? string.Empty),
            new MySqlParameter("@Report", newAttention.Report ?? string.Empty)
        };
        await ExecuteNonQuery(query, parameters);

    }
    public async Task<List<Dictionary<string, object>>?> GetAll()
    {
        var query = """
                    SELECT 
                        c.id_consultation,
                        c.report AS consultation_report,

                        a.*, 
                        a.report AS admission_report,

                        d.id_doctor as doctor_id,
                        d.first_name as doctor_name,
                        d.last_name as doctor_lastname,
                        d.dni as doctor_licence_number,

                        n.id_nurse as nurse_id, 
                        n.first_name as nurse_name, 
                        n.last_name as nurse_lastname, 
                        n.dni as nurse_dni,

                        p.*
                    FROM consultation c
                    LEFT JOIN admission a 
                        ON c.admission_id_admission = a.id_admission
                    LEFT JOIN nurse n
                        ON a.nurse_id_nurse = n.id_nurse
                    LEFT JOIN doctor d 
                        ON c.doctor_id_doctor = d.id_doctor
                    LEFT JOIN patient p
                        ON a.patient_id_patient = p.id_patient;
                    """;

        return await ExecuteReader(query);
    }
    public async Task<Dictionary<string, object>?> GetById(string idAdmission)
    {
        var query = """
                    SELECT 
                        c.id_consultation,
                        c.report AS consultation_report,

                        a.*, 
                        a.report AS admission_report,

                        d.id_doctor as doctor_id,
                        d.first_name as doctor_name,
                        d.last_name as doctor_lastname,
                        d.dni as doctor_licence_number,

                        n.id_nurse as nurse_id, 
                        n.first_name as nurse_name, 
                        n.last_name as nurse_lastname, 
                        n.dni as nurse_dni,

                        p.*
                    FROM consultation c
                    LEFT JOIN admission a 
                        ON c.admission_id_admission = a.id_admission
                    LEFT JOIN nurse n
                        ON a.nurse_id_nurse = n.id_nurse
                    LEFT JOIN doctor d 
                        ON c.doctor_id_doctor = d.id_doctor
                    LEFT JOIN patient p
                        ON a.patient_id_patient = p.id_patient
                    WHERE a.id_admission = @IdAdmission;
                    """;
        var param = new MySqlParameter("@IdAdmission", idAdmission);
        var income = await ExecuteReader(query, param);
        return income?.FirstOrDefault();
    }
}
