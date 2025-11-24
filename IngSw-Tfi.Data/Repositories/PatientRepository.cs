using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;
using System.Collections.Generic;

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
        var patientsFound = await _patientDao.GetAll();
        if (patientsFound == null) return null;
        return patientsFound?
                    .Select(p => MapEntity(p))
                    .ToList();
    }
    public async Task<List<Patient>?> GetByCuil(string cuilPatient)
    {
        var patientsFound = await _patientDao.GetByCuil(cuilPatient);
        return patientsFound?
                    .Select(p => MapEntity(p))
                    .ToList();
    }
    public async Task AddPatient(Patient newPatient) => await _patientDao.AddPatient(newPatient);
    public Task<Patient?> GetById(int id)
    {
        throw new NotImplementedException();
    }
    private Patient MapEntity(Dictionary<string, object> reader)
    {
        return new Patient
        {
            Id = Guid.Parse(reader.GetValueOrDefault("id_patient")!.ToString()!),
            Name = reader.GetValueOrDefault("first_name")!.ToString(),
            LastName = reader.GetValueOrDefault("last_name")!.ToString(),
            Cuil = Cuil.Create(reader.GetValueOrDefault("patient_cuil")!.ToString()!),
            Email = reader.GetValueOrDefault("email")!.ToString(),
            Domicilie = new Domicilie
            {
                Number = int.Parse(reader.GetValueOrDefault("number_address")!.ToString()!),
                Street = reader.GetValueOrDefault("street_address")!.ToString(),
                Locality = reader.GetValueOrDefault("town_address")!.ToString()
            }
        };
    }
}
