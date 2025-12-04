using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Domain.Entities;
using MySql.Data.MySqlClient;

namespace IngSw_Tfi.Data.DAOs;

public class IncomeDao : DaoBase
{
    public IncomeDao(SqlConnection connection) : base(connection) { }
    public async Task<Dictionary<string, object>?> GetById(string idIncome)
    {
        var query = """
            SELECT a.*, p.*, h.*, 
                   n.id_nurse as nurse_id, n.first_name as nurse_name, n.last_name as nurse_lastname, n.dni as nurse_dni
            FROM admission a
            LEFT JOIN patient p ON a.patient_id_patient = p.id_patient
            LEFT JOIN health_insurance h ON p.health_insurance_id = h.id_health_insurance
            LEFT JOIN nurse n ON a.nurse_id_nurse = n.id_nurse
            WHERE a.id_admission = @IdIncome
            LIMIT 1;
            """;
        var param = new MySqlParameter("@IdIncome", idIncome);
        var income = await ExecuteReader(query, param);
        return income?.FirstOrDefault();
    }
    public async Task<List<Dictionary<string, object>>?> GetAll()
    {
        var query = """
            SELECT a.*, p.*, h.*, 
                   n.id_nurse as nurse_id, n.first_name as nurse_name, n.last_name as nurse_lastname, n.dni as nurse_dni
            FROM admission a
            LEFT JOIN patient p ON a.patient_id_patient = p.id_patient
            LEFT JOIN health_insurance h ON p.health_insurance_id = h.id_health_insurance
            LEFT JOIN nurse n ON a.nurse_id_nurse = n.id_nurse;
            """;
        return await ExecuteReader(query);
    }
    public async Task AddIncome(Income newIncome)
    {
        await ExecuteInTransaction(async (conn, tx) =>
        {
            await Add(newIncome, conn, tx);
        });
    }
    public async Task Add(Income newIncome, MySqlConnection conn, MySqlTransaction tx)
    {
        var query = """
        INSERT INTO admission (
            id_admission, nurse_id_nurse, patient_id_patient,
            status, level, start_date, end_date_time,
            temperature, heart_rate, respiratory_rate,
            report, systolic_rate, diastolic_rate
        )
        VALUES (
            @IdAdmission, @IdNurse, @IdPatient,
            @IncomeStatus, @EmergencyLevel, @StartDate, @EndDate,
            @Temperature, @HeartRate, @RespiratoryRate,
            @Report, @SystolicRate, @DiastolicRate
        );
        """;

        using var cmd = new MySqlCommand(query, conn, tx);

        cmd.Parameters.AddWithValue("@IdAdmission", newIncome.Id);
        cmd.Parameters.AddWithValue("@IdNurse", newIncome.Nurse!.Id);
        cmd.Parameters.AddWithValue("@IdPatient", newIncome.Patient!.Id);
        cmd.Parameters.AddWithValue("@IncomeStatus", newIncome.IncomeStatus);
        cmd.Parameters.AddWithValue("@EmergencyLevel", newIncome.EmergencyLevel);
        cmd.Parameters.AddWithValue("@StartDate", newIncome.IncomeDate);
        cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
        cmd.Parameters.AddWithValue("@Temperature", newIncome.Temperature);
        cmd.Parameters.AddWithValue("@HeartRate", newIncome.FrequencyCardiac!.Value);
        cmd.Parameters.AddWithValue("@RespiratoryRate", newIncome.FrequencyRespiratory!.Value);
        cmd.Parameters.AddWithValue("@Report", newIncome.Description);
        cmd.Parameters.AddWithValue("@SystolicRate", newIncome.BloodPressure!.FrecuencySystolic!.Value);
        cmd.Parameters.AddWithValue("@DiastolicRate", newIncome.BloodPressure!.FrecuencyDiastolic!.Value);

        await cmd.ExecuteNonQueryAsync();
    }
    public async Task UpdateIncomeStatus(string idAdmission, int newStatus)
    {
        var sql = "UPDATE admission SET status = @Status WHERE id_admission = @Id";
        var parameters = new[]
        {
            new MySqlParameter("@Status", newStatus),
            new MySqlParameter("@Id", idAdmission)
        };
        await ExecuteNonQuery(sql, parameters);
    }
    public async Task<Dictionary<string, object>?> VerifyIncome(string idPatient)
    {
        var query = """
            SELECT a.*, p.*, h.*, 
                   n.id_nurse as nurse_id, n.first_name as nurse_name, n.last_name as nurse_lastname, n.dni as nurse_dni
            FROM admission a
            LEFT JOIN patient p ON a.patient_id_patient = p.id_patient
            LEFT JOIN health_insurance h ON p.health_insurance_id = h.id_health_insurance
            LEFT JOIN nurse n ON a.nurse_id_nurse = n.id_nurse
            WHERE a.patient_id_patient = @IdPatient
            LIMIT 1;
            """;
        var param = new MySqlParameter("@IdPatient", idPatient);
        var income = await ExecuteReader(query, param);
        return income?.FirstOrDefault();
    }
}
