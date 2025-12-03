using IngSw_Tfi.Application.DTOs;

namespace IngSw_Tfi.Application.Interfaces;

public interface IPatientsService
{
    Task<PatientDto.Response?> AddPatient(PatientDto.Request patientData);
    Task<List<PatientDto.Response>?> GetByCuil(string cuilPatient);
    Task<List<PatientDto.Response>?> GetAll();
    Task<List<PatientDto.Response>> GetByDni(string dni);
    Task<PatientDto.Response?> GetById(int id);
}
