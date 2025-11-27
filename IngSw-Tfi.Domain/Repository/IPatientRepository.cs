using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Domain.Repository;

public interface IPatientRepository
{
    Task<List<Patient>?> GetByCuil(string cuilPatient);
    Task AddPatient(Patient newPatient);
    Task<List<Patient>?> GetAll();
    Task<Patient?> GetById(int id);
    Task<Patient?> GetByGuid(string id);
}
