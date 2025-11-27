using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Domain.Interfaces;
namespace IngSw_Tfi.Application.Services;

public class PatientsService : IPatientsService
{
    private readonly IPatientRepository _patientRepository;
    private readonly ISocialWorkServiceApi _socialWorkServiceApi;
    public PatientsService(IPatientRepository patientRepository, ISocialWorkServiceApi socialWorkServiceApi)
    {
        _patientRepository = patientRepository;
        _socialWorkServiceApi = socialWorkServiceApi;
    }
    public async Task<PatientDto.Response?> AddPatient(PatientDto.Request patientData)
    {
        var patientFound = await _patientRepository.GetByCuil(patientData.cuilPatient);
        if (patientFound != null)
            throw new BusinessConflicException($"El paciente de cuil {patientData.cuilPatient} ya se encuentra registrado");

        var campos = new Dictionary<string, object?>
        {
            { "Apellido", patientData.lastNamePatient },
            { "Nombre", patientData.namePatient },
            { "Calle", patientData.streetDomicilie },
            { "Localidad", patientData.localityDomicilie }
        };
        foreach (var campo in campos)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(campo.Value)))
                throw new ArgumentException($"El campo '{campo.Key}' no puede ser omitido.");
        }
        if (patientData.numberDomicilie <= 0 || patientData.numberDomicilie > 9999)
            throw new ArgumentException("El campo 'Número' no puede ser omitido o exceder el límite permitido.");

        Affiliate? affiliation = null;
        bool oneCompleted = string.IsNullOrEmpty(patientData.nameSocialWork) != string.IsNullOrEmpty(patientData.affiliateNumber);
        if (oneCompleted)
        {
            throw new ArgumentException("Si se ingresa la obra social, también debe ingresarse el número de afiliado (y viceversa).");
        }
        if (!string.IsNullOrEmpty(patientData.nameSocialWork) && !string.IsNullOrEmpty(patientData.affiliateNumber))
        {
            if (!await _socialWorkServiceApi.ExistingSocialWork(patientData.nameSocialWork))
                throw new BusinessConflicException("La obra social no existe, por lo tanto no se puede registrar al paciente.");
            if (!await _socialWorkServiceApi.IsAffiliated(patientData.affiliateNumber))
                throw new BusinessConflicException("El paciente no es afiliado de la obra social, por lo tanto no se puede registrar al paciente.");
            var socialWork = new SocialWork { Id = Guid.NewGuid(), Name = patientData.nameSocialWork };
            affiliation = new Affiliate { Id = Guid.NewGuid(), SocialWork = socialWork, AffiliateNumber = patientData.affiliateNumber };
        }
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
            },
            Affiliate = affiliation
        };
        newPatient.Id = Guid.NewGuid();
        await _patientRepository.AddPatient(newPatient);
        return new PatientDto.Response(newPatient.Id.Value, newPatient.Cuil.Value!, newPatient.Name!, newPatient.LastName!, newPatient.Email!,
                    newPatient.Domicilie!.Street!, newPatient.Domicilie.Number, newPatient.Domicilie.Locality!);
    }
    public async Task<List<PatientDto.Response>?> GetByCuil(string cuilPatient)
    {
        //var cuilValid = Cuil.Create(cuilPatient);
        var patientsFounds = await _patientRepository.GetByCuil(cuilPatient);
        if (patientsFounds == null || !(patientsFounds.Count > 0))
            throw new NullException($"No hay pacientes que coincidan con el cuil {cuilPatient} registrados.");
        return patientsFounds.Select(pr => new PatientDto.Response(pr.Id ?? Guid.Empty, pr.Cuil!.Value!, pr.Name!, pr.LastName!,
                    pr.Email!, pr.Domicilie!.Street!, pr.Domicilie.Number, pr.Domicilie.Locality!))
                    .ToList();
    }
    public async Task<List<PatientDto.Response>?> GetAll()
    {
        var patients = await _patientRepository.GetAll();
        if (patients == null || patients.Count == 0) return new List<PatientDto.Response>();
        return patients.Select(p => new PatientDto.Response(p.Id ?? Guid.Empty, p.Cuil?.Value ?? string.Empty, p.Name ?? string.Empty, p.LastName ?? string.Empty,
            p.Email ?? string.Empty, p.Domicilie?.Street ?? string.Empty, p.Domicilie?.Number ?? 0, p.Domicilie?.Locality ?? string.Empty)).ToList();
    }

    public async Task<PatientDto.Response?> GetById(int id)
    {
        var patient = await _patientRepository.GetById(id);
        if (patient == null) return null;
        return new PatientDto.Response(patient.Id ?? Guid.Empty, patient.Cuil?.Value ?? string.Empty, patient.Name ?? string.Empty, patient.LastName ?? string.Empty,
            patient.Email ?? string.Empty, patient.Domicilie?.Street ?? string.Empty, patient.Domicilie?.Number ?? 0, patient.Domicilie?.Locality ?? string.Empty);
    }
}
