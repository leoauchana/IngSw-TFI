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
    public async Task<List<Dictionary<string, object>>> GetAll()
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
        using (var conn = (System.Data.IDbConnection)_connection.CreateConnection())
        {
            conn.Open();
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    await AddIncome(newIncome, conn, tx);
                    tx.Commit();
                }
                catch
                {
                    try { tx.Rollback(); } catch { }
                    throw;
                }
            }
        }
    }

    public async Task AddIncome(Income newIncome, System.Data.IDbConnection conn, System.Data.IDbTransaction? tx)
    {
        // Discover existing columns to be resilient with DB variants
        var columnsInfo = await ExecuteReader("SHOW COLUMNS FROM admission", conn, tx);
        var existingCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (columnsInfo != null)
        {
            foreach (var c in columnsInfo)
            {
                if (c.TryGetValue("Field", out var fld) && fld != null)
                    existingCols.Add(fld.ToString()!);
            }
        }

        var id = Guid.NewGuid().ToString();
        var patientIdParam = newIncome.Patient?.Id?.ToString();
        var nurseIdParam = newIncome.Nurse?.Id?.ToString();
        
        // Usar zona horaria de Argentina (UTC-3)
        var argentinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Buenos_Aires");
        var startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, argentinaTimeZone);
        var endDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, argentinaTimeZone);
        
        var level = newIncome.EmergencyLevel.HasValue ? ((int)newIncome.EmergencyLevel.Value + 1) : 1;
        var status = (int?)(newIncome.IncomeStatus.HasValue ? (int)newIncome.IncomeStatus.Value : 0) ?? 0;

        var insertCols = new List<string>();
        var insertParams = new List<string>();
        var parametersList = new List<MySqlParameter>();

        void AddIfExists(string colName, string paramName, object? value)
        {
            if (existingCols.Contains(colName))
            {
                insertCols.Add(colName);
                insertParams.Add(paramName);
                parametersList.Add(new MySqlParameter(paramName, value ?? (object)DBNull.Value));
            }
        }

        AddIfExists("id_admission", "@Id", id);
        AddIfExists("nurse_id_nurse", "@NurseId", nurseIdParam);
        AddIfExists("patient_id_patient", "@PatientId", patientIdParam);
        AddIfExists("status", "@Status", status);
        AddIfExists("level", "@Level", level);
        AddIfExists("start_date", "@StartDate", startDate);
        AddIfExists("end_date_time", "@EndDateTime", endDateTime);
        AddIfExists("temperature", "@Temperature", newIncome.Temperature);
        AddIfExists("heart_rate", "@HeartRate", newIncome.FrequencyCardiac);
        AddIfExists("respiratory_rate", "@RespRate", newIncome.FrequencyRespiratory);
        AddIfExists("report", "@Description", newIncome.Description ?? string.Empty);
        AddIfExists("systolic_rate", "@Systolic", newIncome.SystolicRate);
        AddIfExists("diastolic_rate", "@Diastolic", newIncome.DiastolicRate);
        AddIfExists("blood_pressure", "@BloodPressure", newIncome.SystolicRate ?? (object)DBNull.Value);

        if (insertCols.Count == 0) throw new InvalidOperationException("No known columns to insert into admission table.");

        var columnsSql = string.Join(", ", insertCols);
        var paramsSql = string.Join(", ", insertParams);
        var sql = $"INSERT INTO admission ({columnsSql}) VALUES ({paramsSql})";
        await ExecuteNonQuery(sql, conn, tx, parametersList.ToArray());
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
}
