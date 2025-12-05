using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Bdd.Mock;

public class PatientsRepositoyInMemory : IPatientRepository
{
    public List<Patient>? Patients { get; set; }
    public PatientsRepositoyInMemory()
    {
        Patients = new List<Patient>();
    }
    public Task<List<Patient>?> GetByCuil(string cuilPatient)
    {
        var patients = Patients!
            .Where(p => p.Cuil!.Value.Equals(cuilPatient))
            .ToList();

        return Task.FromResult(patients.Any() ? patients : null);
    }
    public Task<List<Patient>?> GetAll()
    {
        return Task.FromResult(Patients!.ToList() ?? null);
    }
    public Task AddPatient(Patient newPatient)
    {
        Patients!.Add(newPatient);
        return Task.CompletedTask;
    }
    public Task<Patient?> GetById(int id)
    {
        throw new NotImplementedException();
    }
    public Task<Patient?> GetByGuid(string idPatient)
    {
        var patientFound = Patients!.Where(e => e.Id.ToString().Equals(idPatient)).First();
        return Task.FromResult(patientFound ?? null);
    }
}
