using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;

namespace IngSw_Tfi.Application.Services;

public class PatientsService : IPatientsService
{
    private readonly IPatientRepository _patientRepository;
    public PatientsService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientDto.Response?> AddPatient(PatientDto.Request patientData)
    {
        var patientFound = await _patientRepository.GetByCuil(patientData.cuilPatient);
        if (patientFound != null) 
            throw new BusinessConflicException($"El paciente de cuil {patientData.cuilPatient} ya se encuentra registrado");
        var newPatient = new Patient
        {
            Cuil = Cuil.Create(patientData.cuilPatient),
            Name = patientData.namePatient,
            LastName = patientData.lastNamePatient,
            Email = patientData.email,
            Domicilie = new Domicilie
            {
                Number = patientData.numberDomicilie,
                Street = patientData.streetDomicilie,
                Locality = patientData.localityDomicilie
            }
        };
        await _patientRepository.AddPatient(newPatient);
        return new PatientDto.Response
                (
                    newPatient.Cuil.Value!, newPatient.Name!, newPatient.LastName!, newPatient.Email!,
                    newPatient.Domicilie!.Street!, newPatient.Domicilie.Number, newPatient.Domicilie.Locality!);
    }

    public async Task<List<PatientDto.Response>?> GetByCuil(string cuilPatient)
    {
        var patientsFounds = await _patientRepository.GetByCuil(cuilPatient);
        if (patientsFounds == null || !(patientsFounds.Count > 0)) 
            throw new NullException($"No hay pacientes que coincidan con el cuil {cuilPatient} registrados.");
        return patientsFounds
                .Select(pr => new PatientDto.Response
                (
                    pr.Cuil.Value!, pr.Name!, pr.LastName!, pr.Email!, pr.Domicilie!.Street!,
                    pr.Domicilie.Number, pr.Domicilie.Locality!
                ))
                .ToList();
    }
}
