using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;

namespace IngSw_Tfi.Application.Services;

public class IncomesService : IIncomesService
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IPriorityQueueService _priorityQueueService;
    private readonly IEmployeeRepository _employeeRepository;

    public IncomesService(IIncomeRepository incomeRepository, IPatientRepository patientRepository,
        IPriorityQueueService priorityQueueService, IEmployeeRepository employeeRepository)
    {
        _incomeRepository = incomeRepository;
        _patientRepository = patientRepository;
        _priorityQueueService = priorityQueueService;
        _employeeRepository = employeeRepository;
    }
    public async Task<IncomeDto.Response?> AddIncome(string idUser, IncomeDto.RequestT newIncome)
    {
        // Verificar que existe la enfermera registrada
        var nurseFound = await _employeeRepository.GetById(idUser);
        if (nurseFound == null) throw new EntityNotFoundException($"No se encontro el empleado autenticado de id {idUser}");
        // Verificar que exista el paciente registrado
        var patientFound = await _patientRepository.GetByGuid(newIncome.idPatient);
        if (patientFound == null) throw new EntityNotFoundException($"No se encontró ningún paciente con cuil {newIncome.idPatient}");
        // Verificar que el paciente no tenga un ingreso activo
        // var hasActive = _priorityQueueService.HasActiveIncome(Guid.Parse(newIncome.idPatient));
        var hasActive = await _incomeRepository.HasActiveIncomeByPatient(newIncome.idPatient);
        Console.WriteLine(hasActive);
        if (hasActive) throw new BusinessConflicException("El paciente ya tiene un ingreso activo.");

        var argentinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Buenos_Aires");
        var argentinaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, argentinaTimeZone);

        var income = new Income
        {
            Description = newIncome.report,
            EmergencyLevel = newIncome.emergencyLevel,
            IncomeDate = argentinaTime,
            Temperature = newIncome.temperature,
            FrequencyCardiac = new FrecuencyCardiac(newIncome.frecuencyCardiac),
            FrequencyRespiratory = new FrecuencyRespiratory(newIncome.frecuencyRespiratory),
            BloodPressure = new BloodPressure(new FrecuencySystolic(newIncome.frecuencySystolic),
                            new FrecuencyDiastolic(newIncome.frecuencyDiastolic)),
            IncomeStatus = IncomeStatus.EARRING,
            Patient = patientFound,
            Nurse = (Nurse)nurseFound
        };
        await _incomeRepository.AddIncome(income);
        _priorityQueueService.Enqueue(income);
        return MapToDto(income);
    }
    public List<IncomeDto.Response>? GetAllEarrings()
    {
        var incomesEarrings = _priorityQueueService.GetAll();
        if (incomesEarrings == null || !incomesEarrings.Any()) return new List<IncomeDto.Response>();
        return incomesEarrings.Select(MapToDto).ToList();
    }
    public async Task<IncomeDto.Response?> GetById(string idIncome)
    {
        var income = await _incomeRepository.GetById(idIncome);
        if (income == null) return null;
        return MapToDto(income);
    }
    public async Task<List<IncomeDto.Response>?> GetAll()
    {
        var incomesEarring = _priorityQueueService.GetAll() ?? new List<Income>();
        var incomes = await _incomeRepository.GetAll();
        if (incomes == null || incomes.Count == 0) return new List<IncomeDto.Response>();
        var allIncomes = incomesEarring
        .Concat(incomes)
        .GroupBy(i => i.Id)
        .Select(g => g.First())
        .ToList();
        return incomes.Select(MapToDto).ToList();
    }
    private IncomeDto.Response MapToDto(Income income)
    {
        var patientDto = new PatientDto.Response(
            income.Patient?.Id ?? Guid.Empty,
            income.Patient?.Cuil?.Value ?? string.Empty,
            income.Patient?.Name ?? string.Empty,
            income.Patient?.LastName ?? string.Empty,
            income.Patient?.Email ?? string.Empty,
            income.Patient?.BirthDate ?? DateTime.MinValue,
            income.Patient?.Phone,
            income.Patient?.Domicilie?.Street ?? string.Empty,
            income.Patient?.Domicilie?.Number ?? 0,
            income.Patient?.Domicilie?.Locality ?? string.Empty,
            new AffiliateDto.Response(
                income.Patient?.Affiliate?.SocialWork?.Name,
                income.Patient?.Affiliate?.AffiliateNumber
            )
        );
        var levelId = (int)income.EmergencyLevel + 1;
        var levelLabel = income.EmergencyLevel switch
        {
            EmergencyLevel.CRITICAL => "Crítica",
            EmergencyLevel.EMERGENCY => "Emergencia",
            EmergencyLevel.URGENCY => "Urgencia",
            EmergencyLevel.URGENCY_MINOR => "Urgencia menor",
            EmergencyLevel.WITHOUT_URGENCY => "Sin urgencia",
            _ => "Desconocido"
        };

        var status = income.IncomeStatus ?? IncomeStatus.EARRING;
        var statusId = status.ToString();
        var statusLabel = status switch
        {
            IncomeStatus.EARRING => "Pendiente",
            IncomeStatus.IN_PROCESS => "En Proceso",
            IncomeStatus.FINISHED => "Finalizado",
            _ => "Pendiente"
        };

        EmployeeDto.NurseResponse? nurseDto = null;
        if (income.Nurse != null)
        {
            nurseDto = new EmployeeDto.NurseResponse(
                income.Nurse.Id.ToString() ?? string.Empty,
                income.Nurse.Name ?? string.Empty,
                income.Nurse.LastName ?? string.Empty,
                income.Nurse.Registration
            );
        }

        return new IncomeDto.Response(
            income.Id.ToString(),
            patientDto,
            income.IncomeDate ?? DateTime.MinValue,
            new IncomeDto.EmergencyLevelDto(levelId, levelLabel),
            new IncomeDto.StatusDto(statusId, statusLabel),
            income.Temperature,
            income.FrequencyCardiac?.Value ?? 0,
            income.FrequencyRespiratory?.Value ?? 0,
            income.BloodPressure?.FrecuencySystolic?.Value ?? 0,
            income.BloodPressure?.FrecuencyDiastolic?.Value ?? 0,
            income.Description,
            nurseDto
        );
    }
}

