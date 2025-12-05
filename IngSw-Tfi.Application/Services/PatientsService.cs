using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Data.DAOs;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Interfaces;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;


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
            { "Telefono", patientData.phone },
            { "Calle", patientData.streetDomicilie },
            { "Localidad", patientData.localityDomicilie }
        };
        foreach (var campo in campos)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(campo.Value)))
                throw new ArgumentException($"El campo '{campo.Key}' no puede ser omitido.");
        }
        if (patientData.numberDomicilie <= 0 || patientData.numberDomicilie > 999999)
            throw new ArgumentException("El campo 'Número' no puede ser omitido o exceder el límite permitido.");
        if (patientData.birthDate < new DateTime(1900, 1, 1) || patientData.birthDate > DateTime.Today)
            throw new ArgumentException("El campo 'Fecha de nacimiento' es inválido o está fuera del rango permitido.");

        Affiliate? affiliation = null;
        //bool oneCompleted = string.IsNullOrEmpty(patientData.idSocialWork) != string.IsNullOrEmpty(patientData.affiliateNumber);
        bool faltaUno =
            string.IsNullOrEmpty(patientData.idSocialWork) ^
            string.IsNullOrEmpty(patientData.affiliateNumber);
        if (faltaUno)
        {
            throw new ArgumentException("Si se ingresa la obra social, también debe ingresarse el número de afiliado (y viceversa).");
        }
        if (!string.IsNullOrEmpty(patientData.idSocialWork) && !string.IsNullOrEmpty(patientData.affiliateNumber))
        {
            var socialWorkFound = await _socialWorkServiceApi.ExistingSocialWork(patientData.idSocialWork);
            if (socialWorkFound == null)
                throw new BusinessConflicException("La obra social no existe, por lo tanto no se puede registrar al paciente.");
            //if (!await _socialWorkServiceApi.IsAffiliated(patientData.affiliateNumber))
            //    throw new BusinessConflicException("El paciente no es afiliado de la obra social, por lo tanto no se puede registrar al paciente.");
            affiliation = new Affiliate {SocialWork = socialWorkFound, AffiliateNumber = patientData.affiliateNumber };
        }
        var newPatient = new Patient
        {
            Cuil = Cuil.Create(patientData.cuilPatient),
            Name = patientData.namePatient,
            LastName = patientData.lastNamePatient,
            Email = patientData.email,
            BirthDate = patientData.birthDate,
            Phone = patientData.phone,
            Domicilie = new Domicilie
            {
                Number = patientData.numberDomicilie,
                Street = patientData.streetDomicilie,
                Locality = patientData.localityDomicilie
            },
            Affiliate = affiliation
        };
        await _patientRepository.AddPatient(newPatient);

        return new PatientDto.Response(newPatient.Id, newPatient.Cuil.Value!, newPatient.Name!, newPatient.LastName!, newPatient.Email!,
                    newPatient.BirthDate, newPatient.Phone, newPatient.Domicilie!.Street!, newPatient.Domicilie.Number, newPatient.Domicilie.Locality!,
                    new AffiliateDto.Response(newPatient.Affiliate?.SocialWork?.Name, newPatient.Affiliate?.AffiliateNumber));
    }
    public async Task<List<PatientDto.Response>?> GetByCuil(string cuilPatient)
    {
        var patientsFounds = await _patientRepository.GetByCuil(cuilPatient);
        if (patientsFounds == null || !(patientsFounds.Count > 0))
            throw new NullException($"No hay pacientes que coincidan con el cuil {cuilPatient} registrados.");
        return patientsFounds.Select(pr => new PatientDto.Response(pr.Id, pr.Cuil!.Value!, pr.Name!, pr.LastName!,
                    pr.Email!, pr.BirthDate, pr.Phone, pr.Domicilie!.Street!, pr.Domicilie.Number, pr.Domicilie.Locality!,
                    new AffiliateDto.Response(pr.Affiliate?.SocialWork?.Name, pr.Affiliate?.AffiliateNumber)))
                    .ToList();
    }
    public async Task<List<PatientDto.Response>?> GetAll()
    {
        var patients = await _patientRepository.GetAll();
        if (patients == null || patients.Count == 0) throw new NullException("No hay pacientes registrados.");
        return patients.Select(p => new PatientDto.Response(p.Id, p.Cuil?.Value ?? string.Empty, p.Name ?? string.Empty, p.LastName ?? string.Empty,
            p.Email ?? string.Empty, p.BirthDate, p.Phone, p.Domicilie?.Street ?? string.Empty, p.Domicilie?.Number ?? 0, p.Domicilie?.Locality ?? string.Empty,
            new AffiliateDto.Response(p.Affiliate?.SocialWork?.Name, p.Affiliate?.AffiliateNumber))).ToList();
    }

    public async Task<PatientDto.Response?> GetById(int id)
    {
        var patient = await _patientRepository.GetById(id);
        if (patient == null) return null;
        return new PatientDto.Response(patient.Id, patient.Cuil?.Value ?? string.Empty, patient.Name ?? string.Empty, patient.LastName ?? string.Empty,
            patient.Email ?? string.Empty, patient.BirthDate, patient.Phone, patient.Domicilie?.Street ?? string.Empty, patient.Domicilie?.Number ?? 0, patient.Domicilie?.Locality ?? string.Empty,
            new AffiliateDto.Response(patient.Affiliate?.SocialWork?.Name, patient.Affiliate?.AffiliateNumber));
    }

    public async Task<List<PatientDto.Response>> GetByDni(string dni)
    {
        var patientsFounds = await _patientRepository.GetAll();

        var listDni = patientsFounds?.Where(p =>
        {
            if (string.IsNullOrWhiteSpace(p.Cuil!.Value))
                return false;

            var parts = p.Cuil.Value.Split('-');
            if (parts.Length != 3)
                return false;

            return parts[1] == dni;
        }).ToList() ?? new List<Patient>();

        if (listDni.Count == 0)
            return new List<PatientDto.Response>();

        return listDni
        .Select(p => new PatientDto.Response(
            p.Id,
            p.Cuil!.Value!,
            p.Name!,
            p.LastName!,
            p.Email!,
            p.BirthDate,
            p.Phone,
            p.Domicilie!.Street!,
            p.Domicilie.Number,
            p.Domicilie.Locality!,
             new AffiliateDto.Response(p.Affiliate?.SocialWork?.Name
             , p.Affiliate?.AffiliateNumber)
    ))
    .ToList();
    }
}
