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
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPriorityQueueService _priorityQueueService;
    public IncomesService(IIncomeRepository incomeRepository, IPatientRepository patientRepository, 
        IEmployeeRepository employeeRepository, IPriorityQueueService priorityQueueService)
    {
        _incomeRepository = incomeRepository;
        _patientRepository = patientRepository;
        _employeeRepository = employeeRepository;
        _priorityQueueService = priorityQueueService;
    }

    public async Task<IncomeDto.Response> AddIncome(IncomeDto.Request incomeData)
    {
        string email = string.Empty;
        var patientFound = await _patientRepository.GetByCuil(incomeData.cuilPatient);
        if (patientFound == null) throw new EntityNotFoundException($"El paciente de cuil {incomeData.cuilPatient} no está registrado.");
        var nurseFound = await _employeeRepository.GetByEmail(email);
        if (nurseFound == null) throw new EntityNotFoundException($"El enfermero autenticado de email {email} no se existe.");
        var newIncome = new Income
        {
            EmergencyLevel = incomeData.emergencyLevel,
            IncomeStatus = IncomeStatus.EARRING,
            Patient = patientFound.FirstOrDefault(),
            Nurse = (Nurse)nurseFound,
            Description = incomeData.report,
            FrecuencyCardiac = new FrecuencyCardiac(incomeData.frecyencyCardiac),
            FrecuencyRespiratory = new FrecuencyRespiratory(incomeData.frecyencyCardiac),
            BloodPressure = new BloodPressure
            {
                FrecuencyDiastolic = new FrecuencyDiastolic(incomeData.frecuencyDiastolic),
                FrecuencySystolic = new FrecuencySystolic(incomeData.frecuencySystolic)
            }
        };
        _priorityQueueService.Enqueue(newIncome);
        await _incomeRepository.Add(newIncome);
        return new IncomeDto.Response(newIncome.Patient!);
    }

    public List<IncomeDto.Response> GetAllEarrings()
    {
        var priorityQueue = _priorityQueueService.GetAll();
        return priorityQueue.Select(p => new IncomeDto.Response(p.Patient!)).ToList();
    }

    public Task<List<IncomeDto.Response>> GetById(int idIncome)
    {
        throw new NotImplementedException();
    }
}
