using IngSw_Tfi.Data.Database;
using MySql.Data.MySqlClient;

namespace IngSw_Tfi.Data.DAOs;

public class EmployeeDao : DaoBase
{
    public EmployeeDao(SqlConnection connection) : base(connection) { }

    public async Task<Dictionary<string, object>?> GetByEmail(string email)
    {
        var query = """
            SELECT 
                u.idusuario, u.email, u.password,
                n.id_nurse, n.first_name AS nurse_first_name, n.last_name AS nurse_last_name, n.phone_number AS nurse_phone, n.dni AS nurse_dni,
                d.id_doctor, d.first_name AS doctor_first_name, d.last_name AS doctor_last_name, d.phone_number AS doctor_phone, d.license_number AS doctor_license, d.cuil AS doctor_cuil
            FROM `user` u
            LEFT JOIN nurse n ON n.user_idusuario = u.idusuario
            LEFT JOIN doctor d ON d.user_idusuario = u.idusuario
            WHERE u.email = @Email
            LIMIT 1;
            """;
        var param = new MySqlParameter("@Email", email);
        var res = await ExecuteReader(query, param);
        return res?.FirstOrDefault();
    }
}



