using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;
using IngSw_Tfi.Domain.Repository;
using System.Data;

namespace IngSw_Tfi.Data.Repositories;

public class IncomeRepository : IIncomeRepository
{
    private readonly IncomeDao _incomeDao;
    public IncomeRepository(IncomeDao incomeDao)
    {
        _incomeDao = incomeDao;
    }
    public async Task Add(Income newIncome) => await _incomeDao.AddIncome(newIncome);
    public async Task AddTransactional(System.Data.IDbConnection conn, System.Data.IDbTransaction tx, Income newIncome) => await _incomeDao.AddIncome(newIncome, conn, tx);
    public async Task Add(Income newIncome, System.Data.IDbConnection conn, System.Data.IDbTransaction? tx) => await _incomeDao.AddIncome(newIncome, conn, tx);
    public async Task<Income?> GetById(int idIncome)
    {
        var incomeData = await _incomeDao.GetById(idIncome);
        return MapEntity(incomeData);
    }
    public async Task<List<Income>?> GetAll()
    {
        var incomesData = await _incomeDao.GetAll();
        if (incomesData == null) return new List<Income>();
        return incomesData.Select(i => MapEntity(i)).ToList();
    }

    public async Task<List<Income>?> GetAllEarrings()
    {
        var allIncomes = await GetAll();
        return allIncomes?
                    .Where(i => i.IncomeStatus == IncomeStatus.EARRING)
                    .ToList();
    }
    public async Task UpdateStatus(Guid id, IncomeStatus status)
    {
        await _incomeDao.UpdateIncomeStatus(id.ToString(), (int)status);
    }
    private Income MapEntity(Dictionary<string, object> value)
    {
        // Extraer y validar CUIL antes de crear el ValueObject
        string? cuilValue = null;
        if (value.ContainsKey("patient_cuil"))
        {
            var rawCuil = value["patient_cuil"];
            if (rawCuil != null && rawCuil != DBNull.Value)
            {
                var cuilStr = rawCuil.ToString();
                if (!string.IsNullOrWhiteSpace(cuilStr))
                {
                    cuilValue = cuilStr;
                }
            }
        }

        var patient = new Patient
        {
            Id = value.ContainsKey("id_patient") && Guid.TryParse(value["id_patient"]?.ToString(), out var pid) ? pid : Guid.Empty,
            Name = value.ContainsKey("first_name") ? value["first_name"]?.ToString() : value.GetValueOrDefault("name")?.ToString(),
            LastName = value.ContainsKey("last_name") ? value["last_name"]?.ToString() : value.GetValueOrDefault("lastname")?.ToString(),
            Cuil = cuilValue != null ? Domain.ValueObjects.Cuil.Create(cuilValue) : null,
            Email = value.ContainsKey("email") ? value["email"]?.ToString() : null,
            Domicilie = new Domicilie
            {
                Street = value.ContainsKey("street_address") ? value["street_address"]?.ToString() : value.GetValueOrDefault("street_domicilie")?.ToString(),
                Number = value.ContainsKey("number_address") ? Convert.ToInt32(value["number_address"]) : (value.GetValueOrDefault("number_domicilie") != null ? Convert.ToInt32(value.GetValueOrDefault("number_domicilie")) : 0),
                Locality = value.ContainsKey("town_address") ? value["town_address"]?.ToString() : value.GetValueOrDefault("locality_domicilie")?.ToString()
            }
        };

        // Mapear ID de la admisión
        Guid incomeId = Guid.Empty;
        if (value.ContainsKey("id_admission") && Guid.TryParse(value["id_admission"]?.ToString(), out var iid))
            incomeId = iid;

        // Mapear Status (0-based en enum IncomeStatus)
        IncomeStatus? status = null;
        if (value.ContainsKey("status") && int.TryParse(value["status"]?.ToString(), out var statusInt))
            status = (IncomeStatus)statusInt;

        // Mapear Level (en DB es 1-5, en enum EmergencyLevel es 0-4)
        EmergencyLevel? emergencyLevel = null;
        if (value.ContainsKey("level") && int.TryParse(value["level"]?.ToString(), out var levelInt))
        {
            // Convert from 1-based (DB) to 0-based (enum)
            emergencyLevel = (EmergencyLevel)(levelInt - 1);
        }

        // Mapear fecha
        DateTime? incomeDate = null;
        if (value.ContainsKey("start_date") && DateTime.TryParse(value["start_date"]?.ToString(), out var dt))
            incomeDate = dt;

        // Mapear enfermera
        Nurse? nurse = null;
        if (value.ContainsKey("nurse_id") && value["nurse_id"] != null && value["nurse_id"] != DBNull.Value)
        {
            if (Guid.TryParse(value["nurse_id"]?.ToString(), out var nurseId))
            {
                nurse = new Nurse
                {
                    Id = nurseId,
                    Name = value.ContainsKey("nurse_name") && value["nurse_name"] != DBNull.Value ? value["nurse_name"]?.ToString() : null,
                    LastName = value.ContainsKey("nurse_lastname") && value["nurse_lastname"] != DBNull.Value ? value["nurse_lastname"]?.ToString() : null,
                    Registration = value.ContainsKey("nurse_dni") && value["nurse_dni"] != DBNull.Value ? value["nurse_dni"]?.ToString() : null
                };
            }
        }

        var income = new Income
        {
            Id = incomeId,
            Patient = patient,
            Nurse = nurse,
            Description = value.ContainsKey("report") ? value["report"]?.ToString() : null,
            IncomeDate = incomeDate,
            IncomeStatus = status,
            EmergencyLevel = emergencyLevel,
            Temperature = value.ContainsKey("temperature") && float.TryParse(value["temperature"]?.ToString(), out var temp) ? temp : null,
            FrequencyCardiac = value.ContainsKey("heart_rate") && float.TryParse(value["heart_rate"]?.ToString(), out var hr) ? hr : null,
            FrequencyRespiratory = value.ContainsKey("respiratory_rate") && float.TryParse(value["respiratory_rate"]?.ToString(), out var rr) ? rr : null,
            SystolicRate = value.ContainsKey("systolic_rate") && float.TryParse(value["systolic_rate"]?.ToString(), out var sys) ? sys : null,
            DiastolicRate = value.ContainsKey("diastolic_rate") && float.TryParse(value["diastolic_rate"]?.ToString(), out var dia) ? dia : null
        };
        return income;
    }
}
