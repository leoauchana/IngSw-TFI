using IngSw_Tfi.Domain.Entities;

namespace IngSw_Bdd.Mock;

public interface IPatientsRepositoryInMemory
{
    void SavePatient(Patient patient);
    List<Patient>? GetPatients();
}
