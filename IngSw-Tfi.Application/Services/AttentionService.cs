using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Tfi.Application.Services;

public class AttentionService : IAttentionService
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IPriorityQueueService _priorityQueueService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAttentionRepository _attentionRepository;
    public AttentionService(IIncomeRepository incomeRepository, IPriorityQueueService priorityQueueService,
        IEmployeeRepository employeeRepository, IAttentionRepository attentionRepository)
    {
        _incomeRepository = incomeRepository;
        _priorityQueueService = priorityQueueService;
        _employeeRepository = employeeRepository;
        _attentionRepository = attentionRepository;
    }

    public async Task<AttentionDto.Response?> AddAttention(string idDoctor, AttentionDto.Request newAttention)
    {
        var doctorFound = await _employeeRepository.GetById(idDoctor);
        if (doctorFound == null) throw new EntityNotFoundException("No se encontró al doctor autenticado.");
        var incomeFound = await _incomeRepository.GetById(newAttention.idIncome);
        if (incomeFound.IncomeStatus != IncomeStatus.IN_PROCESS) throw new BusinessConflicException("El ingreso no estaba siendo atendido.");
        if (incomeFound == null) throw new EntityNotFoundException("No se encontró el ingreso asociado a la atención.");
        var attention = new Attention
        {
            Report = newAttention.report,
            Income = incomeFound,
            Doctor = (Doctor)doctorFound
        };
        await _attentionRepository.AddAttention(attention);
        await _incomeRepository.UpdateStatus(Guid.Parse(newAttention.idIncome), IncomeStatus.FINISHED);
        return new AttentionDto.Response();
    }

    public async Task<IncomeDto.Response?> UpdateIncomeStatus(string incomeId, string newStatus)
    {
        if (!Guid.TryParse(incomeId, out var guid)) throw new ArgumentException("ID de ingreso inválido");

        var allIncomes = await _incomeRepository.GetAll();
        var income = allIncomes?.FirstOrDefault(i => i.Id == guid);

        if (income == null) throw new EntityNotFoundException($"El ingreso de id {incomeId} no se encontró.");

        IncomeStatus status = newStatus switch
        {
            "PENDIENTE" => IncomeStatus.EARRING,
            "EN_PROCESO" => IncomeStatus.IN_PROCESS,
            "FINALIZADO" => IncomeStatus.FINISHED,
            _ => throw new BusinessConflicException($"Estado '{newStatus}' no válido")
        };

        var queueIncome = _priorityQueueService.Dequeue();
        if (queueIncome == null) throw new BusinessConflicException("Hubo un error al obtener el ingreso siguiente de la cola de espera.");
        income.IncomeStatus = status;
        queueIncome.IncomeStatus = status;
        await _incomeRepository.UpdateStatus(income.Id, status);
        return MapToDto(queueIncome);
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

        IncomeDto.NurseDto? nurseDto = null;
        if (income.Nurse != null)
        {
            nurseDto = new IncomeDto.NurseDto(
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
