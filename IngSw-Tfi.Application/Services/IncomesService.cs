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
    private readonly IngSw_Tfi.Data.Database.SqlConnection _sqlConnection;

    public IncomesService(IIncomeRepository incomeRepository, IPatientRepository patientRepository, IngSw_Tfi.Data.Database.SqlConnection sqlConnection)
    {
        _incomeRepository = incomeRepository;
        _patientRepository = patientRepository;
        _sqlConnection = sqlConnection;
    }

    public Task<IncomeDto.Response?> AddIncome(IncomeDto.Request newIncome)
    {
        // Usar zona horaria de Argentina (UTC-3)
        var argentinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Buenos_Aires");
        var argentinaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, argentinaTimeZone);
        
        var income = new Income
        {
            Description = newIncome.report,
            EmergencyLevel = newIncome.emergencyLevel,
            IncomeDate = argentinaTime,
            Temperature = newIncome.temperature,
            FrequencyCardiac = newIncome.frecyencyCardiac,
            FrequencyRespiratory = newIncome.frecuencyRespiratory,
            SystolicRate = newIncome.frecuencySystolic,
            DiastolicRate = newIncome.frecuencyDiastolic,
            IncomeStatus = IncomeStatus.EARRING
        };

        var t = Task.Run(async () =>
        {
            try
            {
                // Resolve patient logic... (Keeping our robust implementation)
                if (newIncome.patient.ValueKind != System.Text.Json.JsonValueKind.Undefined && newIncome.patient.ValueKind != System.Text.Json.JsonValueKind.Null)
                {
                    var p = newIncome.patient;
                    if (p.TryGetProperty("id", out var pid) && pid.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var idStr = pid.GetString();
                        if (!string.IsNullOrEmpty(idStr))
                        {
                            var patient = await _patientRepository.GetByGuid(idStr);
                            if (patient != null) income.Patient = patient;
                        }
                    }
                    if (income.Patient == null && p.TryGetProperty("cuil", out var pcuil) && pcuil.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var cuil = pcuil.GetString();
                        if (!string.IsNullOrEmpty(cuil))
                        {
                            var found = await _patientRepository.GetByCuil(cuil);
                            if (found != null && found.Count > 0) income.Patient = found.First();
                        }
                    }
                    // Fallback create patient logic...
                    if (income.Patient == null && (p.TryGetProperty("nombre", out var pnom) || p.TryGetProperty("name", out var pname)))
                    {
                        var newPatient = new Domain.Entities.Patient
                        {
                            Id = Guid.NewGuid(),
                            Name = p.TryGetProperty("nombre", out var pn) ? pn.GetString() ?? string.Empty : (p.TryGetProperty("name", out var pn2) ? pn2.GetString() ?? string.Empty : string.Empty),
                            LastName = p.TryGetProperty("apellido", out var pa) ? pa.GetString() ?? string.Empty : string.Empty,
                            Email = p.TryGetProperty("email", out var pe) ? pe.GetString() ?? string.Empty : string.Empty,
                            Domicilie = new Domain.Entities.Domicilie { Street = "", Number = 0, Locality = "" }
                        };
                        await _patientRepository.AddPatient(newPatient);
                        income.Patient = newPatient;
                    }
                }

                // Resolve nurse logic...
                if (newIncome.nurse.ValueKind != System.Text.Json.JsonValueKind.Undefined && newIncome.nurse.ValueKind != System.Text.Json.JsonValueKind.Null)
                {
                    var n = newIncome.nurse;
                    string? idStr = null;
                    if (n.ValueKind == System.Text.Json.JsonValueKind.String) idStr = n.GetString();
                    else if (n.TryGetProperty("id", out var nid) && nid.ValueKind == System.Text.Json.JsonValueKind.String) idStr = nid.GetString();

                    if (!string.IsNullOrEmpty(idStr) && Guid.TryParse(idStr, out var g))
                    {
                        income.Nurse = new Domain.Entities.Nurse { Id = g };
                    }
                }

                // Transactional insert
                using (var conn = (MySql.Data.MySqlClient.MySqlConnection)_sqlConnection.CreateConnection())
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        try
                        {
                            // Validar que el paciente no tenga ya un ingreso activo
                            if (income.Patient != null)
                            {
                                var existingActiveIncomes = await _incomeRepository.GetAllEarrings();
                                var hasActiveIncome = existingActiveIncomes?.Any(i => 
                                    i.Patient?.Id == income.Patient.Id && 
                                    (i.IncomeStatus == null || i.IncomeStatus == Domain.Enums.IncomeStatus.EARRING || i.IncomeStatus == Domain.Enums.IncomeStatus.IN_PROCESS)
                                ) ?? false;

                                if (hasActiveIncome)
                                {
                                    throw new BusinessConflicException($"El paciente {income.Patient.Name} {income.Patient.LastName} ya tiene un ingreso activo.");
                                }
                            }

                            await ((IngSw_Tfi.Data.Repositories.IncomeRepository)_incomeRepository).Add(income, conn, tx);
                            tx.Commit();
                        }
                        catch
                        {
                            try { tx.Rollback(); } catch { }
                            throw;
                        }
                    }
                }
                return MapToDto(income);
            }
            catch (Exception)
            {
                throw;
            }
        });
        return t;
    }

    public async Task<List<IncomeDto.Response>?> GetAllEarrings()
    {
        var incomesEarrings = await _incomeRepository.GetAllEarrings();
        if (incomesEarrings == null || incomesEarrings.Count == 0) return new List<IncomeDto.Response>();
        return incomesEarrings.Select(MapToDto).ToList();
    }

    public async Task<IncomeDto.Response?> GetById(int idIncome)
    {
        var income = await _incomeRepository.GetById(idIncome);
        if (income == null) return null;
        return MapToDto(income);
    }

    public async Task<List<IncomeDto.Response>?> GetAll()
    {
        var incomes = await _incomeRepository.GetAll();
        if (incomes == null || incomes.Count == 0) return new List<IncomeDto.Response>();
        return incomes.Select(MapToDto).ToList();
    }

    public async Task<IncomeDto.Response?> UpdateIncomeStatus(string incomeId, string newStatus)
    {
        if (!Guid.TryParse(incomeId, out var guid)) throw new ArgumentException("ID de ingreso inválido");

        var allIncomes = await _incomeRepository.GetAll();
        var income = allIncomes?.FirstOrDefault(i => i.Id == guid);

        if (income == null) return null;

        Domain.Enums.IncomeStatus status = newStatus switch
        {
            "PENDIENTE" => Domain.Enums.IncomeStatus.EARRING,
            "EN_PROCESO" => Domain.Enums.IncomeStatus.IN_PROCESS,
            "FINALIZADO" => Domain.Enums.IncomeStatus.FINISHED,
            _ => throw new ArgumentException($"Estado '{newStatus}' no válido")
        };

        income.IncomeStatus = status;
        await _incomeRepository.UpdateStatus(income.Id ?? Guid.Empty, status);

        return MapToDto(income);
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
            income.Patient?.Domicilie?.Locality ?? string.Empty
        );

        var levelId = (int)income.EmergencyLevel + 1;
        var levelLabel = income.EmergencyLevel switch
        {
            Domain.Enums.EmergencyLevel.CRITICAL => "Crítica",
            Domain.Enums.EmergencyLevel.EMERGENCY => "Emergencia",
            Domain.Enums.EmergencyLevel.URGENCY => "Urgencia",
            Domain.Enums.EmergencyLevel.URGENCY_MINOR => "Urgencia menor",
            Domain.Enums.EmergencyLevel.WITHOUT_URGENCY => "Sin urgencia",
            _ => "Desconocido"
        };

        var status = income.IncomeStatus ?? Domain.Enums.IncomeStatus.EARRING;
        var statusId = status.ToString();
        var statusLabel = status switch
        {
            Domain.Enums.IncomeStatus.EARRING => "Pendiente",
            Domain.Enums.IncomeStatus.IN_PROCESS => "En Proceso",
            Domain.Enums.IncomeStatus.FINISHED => "Finalizado",
            _ => "Pendiente"
        };

        IncomeDto.NurseDto? nurseDto = null;
        if (income.Nurse != null)
        {
            nurseDto = new IncomeDto.NurseDto(
                income.Nurse.Id?.ToString() ?? string.Empty,
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
            income.FrequencyCardiac,
            income.FrequencyRespiratory,
            income.SystolicRate,
            income.DiastolicRate,
            income.Description,
            nurseDto
        );
    }
}
