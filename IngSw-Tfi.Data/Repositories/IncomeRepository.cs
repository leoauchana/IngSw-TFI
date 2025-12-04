using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;
using System.Data;

namespace IngSw_Tfi.Data.Repositories;

public class IncomeRepository : IIncomeRepository
{
    private readonly IncomeDao _incomeDao;
    public IncomeRepository(IncomeDao incomeDao)
    {
        _incomeDao = incomeDao;
    }
    public async Task AddIncome(Income newIncome) => await _incomeDao.AddIncome(newIncome);
    public async Task<Income?> GetById(string idIncome)
    {
        var incomeData = await _incomeDao.GetById(idIncome);
        if (incomeData == null) return null;
        return MapEntity(incomeData);
    }
    public async Task<List<Income>?> GetAll()
    {
        var incomesData = await _incomeDao.GetAll();
        if (incomesData == null) return new List<Income>();
        return incomesData.Select(i => MapEntity(i)).ToList();
    }
    public async Task UpdateStatus(Guid id, IncomeStatus status)
    {
        await _incomeDao.UpdateIncomeStatus(id.ToString(), (int)status);
    }
    public async Task<List<Income>?> GetAllEarrings()
    {
        var incomesData = await _incomeDao.GetAll();
        if (incomesData == null) return null;
        var incomesList = incomesData!.Select(i => MapEntity(i)).ToList();
        return incomesList.Where(i => i.IncomeStatus != IncomeStatus.EARRING).ToList();
    }
    private Income MapEntity(Dictionary<string, object> value)
    {
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

        Guid incomeId = Guid.Empty;
        if (value.ContainsKey("id_admission") && Guid.TryParse(value["id_admission"]?.ToString(), out var iid))
            incomeId = iid;

        IncomeStatus? status = null;
        if (value.ContainsKey("status") && int.TryParse(value["status"]?.ToString(), out var statusInt))
            status = (IncomeStatus)statusInt;

        EmergencyLevel? emergencyLevel = null;
        if (value.ContainsKey("level") && int.TryParse(value["level"]?.ToString(), out var levelInt))
        {
            emergencyLevel = (EmergencyLevel)(levelInt);
        }

        DateTime? incomeDate = null;
        if (value.ContainsKey("start_date") && DateTime.TryParse(value["start_date"]?.ToString(), out var dt))
            incomeDate = dt;


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
            FrequencyCardiac = value.ContainsKey("heart_rate") &&
                   float.TryParse(value["heart_rate"]?.ToString(), out var hr)
                   && hr >= 60 && hr <= 100
                   ? new FrecuencyCardiac(hr)
                   : null,
            FrequencyRespiratory = value.ContainsKey("respiratory_rate") &&
                       float.TryParse(value["respiratory_rate"]?.ToString(), out var rr)
                       && rr >= 12 && rr <= 20
                       ? new FrecuencyRespiratory(rr)
                       : null
        };
            BloodPressure? bloodPressure = null;

        if (value.TryGetValue("systolic_rate", out var systolicObj) &&
            value.TryGetValue("diastolic_rate", out var diastolicObj) &&
            float.TryParse(systolicObj?.ToString(), out float systolicVal) &&
            float.TryParse(diastolicObj?.ToString(), out float diastolicVal))
        {
            bloodPressure = new BloodPressure(
                new FrecuencySystolic(systolicVal),
                new FrecuencyDiastolic(diastolicVal)
            );
        }
        income.BloodPressure = bloodPressure;
        return income;
    }
    public async Task<bool> HasActiveIncomeByPatient(string idPatient)
    {
        var incomeData = await _incomeDao.VerifyIncome(idPatient);
        if (incomeData == null) return false;
        return true;
    }
}
