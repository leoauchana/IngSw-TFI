using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Domain.Repository;

public interface IPatientRepository
{
    Task<List<Patient>?> GetByCuil(string cuilPatient);
    Task<List<Patient>?> GetAll();
    Task AddPatient(Patient newPatient);
}
