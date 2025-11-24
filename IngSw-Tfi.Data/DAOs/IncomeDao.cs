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
            SELECT * FROM admission i
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
            SELECT * FROM admission i
            INNER JOIN patient p ON i.patient_id = p.patient_id
            INNER JOIN socialWork sw ON i.socialwork_id = sw.socialwork_id;
            """;
        return await ExecuteReader(query);
    }
    public async Task AddIncome(Income newIncome)
    {
        //var query = """
        //    INSERT INTO incomes (nurse_id_nurse, id_patient_id, status, level, stat_date, end_date_time, temperature,
        //    heart_rate, respiratory_rate, report, blood_pressure) VALUES (@IdNurse, @IdPatient, @IncomeStatus, @EmergencyLevel, 
        //    @StartDate, @EndDate, @Temperature, @HeartRate, @RespiratoryRate, @Report, @BloodPressure)
        //    """;
        var query = """
            INSERT INTO admission (id_admission, patient_id_patient, status, level, start_date, end_date_time, temperature,
            heart_rate, respiratory_rate, report, systolic_rate, diastolic_rate) VALUES (@IdAdmission, @IdPatient, @IncomeStatus, @EmergencyLevel, 
            @StartDate, @EndDate, @Temperature, @HeartRate, @RespiratoryRate, @Report, @SystolicRate, @DiastolicRate)
            """;
        var parameters = new[]{
            //new MySqlParameter("@IdNurse", newIncome.Nurse!.Id),
            //new MySqlParameter("@IdPatient", newIncome.Patient!.Id),
            new MySqlParameter("@IdAdmission", Guid.NewGuid()),
            new MySqlParameter("@IdPatient", newIncome.Patient!.Id),
            new MySqlParameter("@IncomeStatus", newIncome.IncomeStatus),
            new MySqlParameter("@EmergencyLevel", newIncome.EmergencyLevel),
            new MySqlParameter("@StartDate", newIncome.IncomeDate),
            new MySqlParameter("@EndDate", newIncome.IncomeDate),
            new MySqlParameter("@Temperature", newIncome.Temperature),
            new MySqlParameter("@HeartRate", newIncome.FrecuencyCardiac!.Value),
            new MySqlParameter("@RespiratoryRate", newIncome.FrecuencyRespiratory!.Value),
            new MySqlParameter("@Report", newIncome.Report),
            new MySqlParameter("@SystolicRate", newIncome.BloodPressure!.FrecuencySystolic!.Value),
            new MySqlParameter("@DiastolicRate", newIncome.BloodPressure.FrecuencyDiastolic!.Value)
        };
        await ExecuteNonQuery(query, parameters);
    }
}
