using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;

namespace IngSw_Tfi.Data.Repositories;

public class AttentionRepository : IAttentionRepository
{
    private readonly AttentionDao _attentionDao;
    public AttentionRepository(AttentionDao attentionDao)
    {
        _attentionDao = attentionDao;
    }
    public async Task AddAttention(Attention newAttention) => await _attentionDao.AddAttention(newAttention);

    public async Task<List<Attention>?> GetAll()
    {
        var attentionsRegistered = await _attentionDao.GetAll();
        if (attentionsRegistered == null) return new List<Attention>();
        return attentionsRegistered.Select(a => MapEntity(a)).ToList();
    }
    private Attention MapEntity(Dictionary<string, object> value)
    {
        string? cuilValue = null;
        if (value.TryGetValue("cuil", out var rawCuil) && rawCuil != DBNull.Value)
        {
            var cuilStr = rawCuil?.ToString();
            if (!string.IsNullOrWhiteSpace(cuilStr))
                cuilValue = cuilStr;
        }

        var patient = new Patient
        {
            Id = value.TryGetValue("id_patient", out var pidObj) &&
                 Guid.TryParse(pidObj?.ToString(), out var pid)
                    ? pid : Guid.Empty,

            Name = value.GetValueOrDefault("first_name")?.ToString(),
            LastName = value.GetValueOrDefault("last_name")?.ToString(),
            Email = value.GetValueOrDefault("email")?.ToString(),

            Cuil = cuilValue != null ? Cuil.Create(cuilValue) : null,

            Domicilie = new Domicilie
            {
                Street = value.GetValueOrDefault("street_address")?.ToString(),
                Number = int.TryParse(value.GetValueOrDefault("number_address")?.ToString(), out var num)
                            ? num : 0,
                Locality = value.GetValueOrDefault("town_address")?.ToString()
            },

            Phone = value.GetValueOrDefault("phone")?.ToString(),

            BirthDate = value.TryGetValue("birth_date", out var bdObj) &&
                        DateTime.TryParse(bdObj?.ToString(), out var bd)
                            ? bd : DateTime.UtcNow
        };
        Guid incomeId = Guid.Empty;
        if (value.TryGetValue("id_admission", out var incObj) &&
            Guid.TryParse(incObj?.ToString(), out var iid))
            incomeId = iid;

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

        DateTime? incomeDate = null;
        if (value.TryGetValue("start_date", out var dtObj) &&
            DateTime.TryParse(dtObj?.ToString(), out var dt))
            incomeDate = dt;
        Nurse? nurse = null;
        if (value.TryGetValue("nurse_id", out var nObj) &&
            Guid.TryParse(nObj?.ToString(), out var nurseId))
        {
            nurse = new Nurse
            {
                Id = nurseId,
                Name = value.GetValueOrDefault("nurse_name")?.ToString(),
                LastName = value.GetValueOrDefault("nurse_lastname")?.ToString(),
                Registration = value.GetValueOrDefault("nurse_dni")?.ToString()
            };
        }
        var income = new Income
        {
            Id = incomeId,
            Patient = patient,
            Nurse = nurse,
            Description = value.GetValueOrDefault("admission_report")?.ToString(),
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
        Doctor? doctor = null;
        if (value.TryGetValue("doctor_id", out var docObj) &&
            Guid.TryParse(docObj?.ToString(), out var doctorId))
        {
            doctor = new Doctor
            {
                Id = doctorId,
                Name = value.GetValueOrDefault("doctor_name")?.ToString(),
                LastName = value.GetValueOrDefault("doctor_lastname")?.ToString(),
                Registration = value.GetValueOrDefault("doctor_licence_number")?.ToString()
            };
        }
        var attention = new Attention
        {
            Id = value.TryGetValue("id_consultation", out var idConsObj) &&
                 Guid.TryParse(idConsObj?.ToString(), out var idCons)
                    ? idCons : Guid.Empty,

            Income = income,
            Doctor = doctor,

            Report = value.GetValueOrDefault("consultation_report")?.ToString()
        };

        return attention;
    }
}
