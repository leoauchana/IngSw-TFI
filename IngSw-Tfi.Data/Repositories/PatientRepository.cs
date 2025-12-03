using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;

namespace IngSw_Tfi.Data.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly PatientDao _patientDao;
    public PatientRepository(PatientDao patientDao)
    {
        _patientDao = patientDao;
    }
    public async Task<List<Patient>?> GetAll()
    {
        var data = await _patientDao.GetAll();
        return data?.Select(d => MapEntity(d)).ToList();
    }

    public async Task<List<Patient>?> GetByCuil(string cuilPatient)
    {
        var patientsFound = await _patientDao.GetByCuil(cuilPatient);
        return patientsFound?
                    .Select(p => MapEntity(p))
                    .ToList();
    }
    public async Task AddPatient(Patient newPatient)
    {
        if(newPatient.Affiliate == null)
        {
            await _patientDao.AddPatient(newPatient);
        }else await _patientDao.AddPatientWithSocialWork(newPatient);
    }

    public Task<Patient?> GetById(int id)
    {
        return Task.Run(async () =>
        {
            // Note: DAO stores id as Guid; try to parse int to Guid if possible or adapt as needed.
            // Here we assume incoming id may be convertible to Guid string.
            Guid guidId;
            if (Guid.TryParse(id.ToString(), out guidId))
            {
                var record = await _patientDao.GetById(guidId);
                return record == null ? null : MapEntity(record);
            }
            return null;
        });
    }
    public async Task<Patient?> GetByGuid(string id)
    {
        var patientFound = await _patientDao.GetById(Guid.Parse(id));
        if (patientFound == null) return null;
        return MapEntity(patientFound);
    }
    private Patient MapEntity(Dictionary<string, object> reader)
    {

        var hasAffiliate = reader.ContainsKey("memberNumber")
                   && reader["memberNumber"] != null
                   && reader["memberNumber"] != DBNull.Value;

        return new Patient
        {
            Id = reader.ContainsKey("id_patient") && Guid.TryParse(Convert.ToString(reader["id_patient"]), out var g) ? g : Guid.NewGuid(),
            Name = reader.ContainsKey("first_name") ? reader["first_name"]?.ToString() : reader.GetValueOrDefault("name")?.ToString(),
            LastName = reader.ContainsKey("last_name") ? reader["last_name"]?.ToString() : reader.GetValueOrDefault("last_name")?.ToString(),
            Cuil = reader.ContainsKey("patient_cuil") && reader["patient_cuil"] != null ? Cuil.Create(reader["patient_cuil"]?.ToString()) : null,
            Email = reader.ContainsKey("email") ? reader["email"]?.ToString() : string.Empty,
            Phone = reader.ContainsKey("phone") && reader["phone"] != DBNull.Value ? reader["phone"]?.ToString() : null,
            BirthDate = reader.ContainsKey("birth_date") && reader["birth_date"] != DBNull.Value
                ? Convert.ToDateTime(reader["birth_date"])
                : DateTime.MinValue,
            Domicilie = new Domicilie
            {
                Street = reader.ContainsKey("street_address") ? reader["street_address"]?.ToString() : reader.GetValueOrDefault("street_domicilie")?.ToString(),
                Number = reader.ContainsKey("number_address") ? Convert.ToInt32(reader["number_address"]) : (reader.GetValueOrDefault("number_domicilie") != null ? Convert.ToInt32(reader.GetValueOrDefault("number_domicilie")) : 0),
                Locality = reader.ContainsKey("town_address") ? reader["town_address"]?.ToString() : reader.GetValueOrDefault("locality_domicilie")?.ToString()
            },
            Affiliate = hasAffiliate
            ? new Affiliate
            {
                AffiliateNumber = reader.GetValueOrDefault("memberNumber")?.ToString() ?? string.Empty,
                SocialWork = new SocialWork
                {
                    Id = reader.ContainsKey("id_socialWork") && Guid.TryParse(Convert.ToString(reader["id_socialWork"]), out var idSW) ? idSW : Guid.NewGuid(),
                    Name = reader.GetValueOrDefault("socialWork_name")?.ToString() ?? string.Empty
                }
            }
            : null
        };
    }
}
