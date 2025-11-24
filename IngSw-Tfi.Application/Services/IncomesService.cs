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

    public async Task<IncomeDto.ResponseTest> AddIncome(IncomeDto.Request incomeData)
    {
        //string email = string.Empty;
        var patientfound = await _patientRepository.GetByCuil(incomeData.cuilPatient);
        if (patientfound == null) throw new EntityNotFoundException($"El paciente de cuil {incomeData.cuilPatient} no está registrado.");
        //var nurseFound = await _employeeRepository.GetByEmail(email);
        //if (nurseFound == null) throw new EntityNotFoundException($"El enfermero autenticado de email {email} no se existe.");
        var newIncome = new Income
        {
            EmergencyLevel = incomeData.emergencyLevel,
            IncomeStatus = IncomeStatus.EARRING,
            Patient = patientfound.FirstOrDefault(),
            //Nurse = (Nurse)nurseFound,
            Report = incomeData.report,
            IncomeDate = DateTime.Now,
            Temperature = incomeData.temperature,
            FrecuencyCardiac = new FrecuencyCardiac(incomeData.frecuencyCardiac),
            FrecuencyRespiratory = new FrecuencyRespiratory(incomeData.frecuencyRespiratory),
            BloodPressure = new BloodPressure
            {
                FrecuencyDiastolic = new FrecuencyDiastolic(incomeData.frecuencyDiastolic),
                FrecuencySystolic = new FrecuencySystolic(incomeData.frecuencySystolic)
            }
        };
        var levelAssigned = EmergencyLevelExtensions.ObtenerNivel(newIncome.EmergencyLevel);
        _priorityQueueService.Enqueue(newIncome);
        await _incomeRepository.Add(newIncome);
        return new IncomeDto.ResponseTest(newIncome.Report, newIncome.EmergencyLevel, DateOnly.FromDateTime(newIncome.IncomeDate), levelAssigned.MaximumDuration,
            newIncome.IncomeStatus.ToString()!, newIncome.Temperature, newIncome.FrecuencyCardiac.Value, newIncome.FrecuencyRespiratory.Value, 
            newIncome.BloodPressure.FrecuencySystolic.Value, newIncome.BloodPressure.FrecuencyDiastolic.Value,
            new PatientDto.ResponseIncome(newIncome.Patient!.Cuil!.Value, newIncome.Patient!.Name!, newIncome.Patient.LastName!));
    }
    public List<IncomeDto.ResponseTest> GetAllEarrings()
    {
        var priorityQueue = _priorityQueueService.GetAll();
        return priorityQueue.Select(i => new IncomeDto.ResponseTest(i.Report, i.EmergencyLevel, DateOnly.FromDateTime(i.IncomeDate),
            EmergencyLevelExtensions.ObtenerNivel(i.EmergencyLevel).MaximumDuration,
            i.IncomeStatus.ToString()!, i.Temperature, i.FrecuencyCardiac.Value, i.FrecuencyRespiratory.Value,
            i.BloodPressure.FrecuencySystolic.Value, i.BloodPressure.FrecuencyDiastolic.Value,
            new PatientDto.ResponseIncome(i.Patient!.Cuil!.Value, i.Patient!.Name!, i.Patient.LastName!)))
            .ToList();
    }
    public Task<List<IncomeDto.Response>> GetById(int idIncome)
    {
        throw new NotImplementedException();
    }
}

