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
}
