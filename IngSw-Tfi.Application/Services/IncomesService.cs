using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;

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
        var income = new Income
        {
            Description = newIncome.report,
            EmergencyLevel = newIncome.emergencyLevel,
            IncomeDate = DateTime.UtcNow,
            Temperature = newIncome.temperature,
            FrequencyCardiac = newIncome.frecyencyCardiac,
            FrequencyRespiratory = newIncome.frecuencyRespiratory,
            SystolicRate = newIncome.frecuencySystolic,
            DiastolicRate = newIncome.frecuencyDiastolic,
            IncomeStatus = null
        };

        var t = Task.Run(async () =>
        {
            // Resolve patient: patient JsonElement may contain id, cuil or dni or full object
            try
            {
                if (newIncome.patient.ValueKind != System.Text.Json.JsonValueKind.Undefined && newIncome.patient.ValueKind != System.Text.Json.JsonValueKind.Null)
                {
                    var p = newIncome.patient;
                    if (p.TryGetProperty("id", out var pid) && pid.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var idStr = pid.GetString();
                        if (!string.IsNullOrEmpty(idStr))
                        {
                            // Try find by GUID id
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
                    if (income.Patient == null && p.TryGetProperty("dni", out var pdni) && (pdni.ValueKind == System.Text.Json.JsonValueKind.String || pdni.ValueKind == System.Text.Json.JsonValueKind.Number))
                    {
                        var dni = pdni.GetString();
                        if (!string.IsNullOrEmpty(dni))
                        {
                            // Attempt to find by cuil-like pattern or by dni in patient table (not implemented); fallback: create patient
                            var newPatient = new Domain.Entities.Patient
                            {
                                Id = Guid.NewGuid(),
                                Name = p.TryGetProperty("nombre", out var pnom) ? pnom.GetString() ?? string.Empty : string.Empty,
                                LastName = p.TryGetProperty("apellido", out var pap) ? pap.GetString() ?? string.Empty : string.Empty,
                                Email = p.TryGetProperty("email", out var pmail) ? pmail.GetString() ?? string.Empty : string.Empty,
                                Domicilie = new Domain.Entities.Domicilie { Street = p.TryGetProperty("street", out var pst) ? pst.GetString() ?? string.Empty : string.Empty, Number = 0, Locality = "" }
                            };
                            await _patientRepository.AddPatient(newPatient);
                            income.Patient = newPatient;
                        }
                    }
                    // If still null, and patient json has name/email, create patient
                    if (income.Patient == null && (p.TryGetProperty("nombre", out var pnom2) || p.TryGetProperty("name", out var pname2)))
                    {
                        var newPatient = new Domain.Entities.Patient
                        {
                            Id = Guid.NewGuid(),
                            Name = p.TryGetProperty("nombre", out var pn) ? pn.GetString() ?? (p.TryGetProperty("name", out var pn2) ? pn2.GetString() ?? string.Empty : string.Empty) : (p.TryGetProperty("name", out var pn3) ? pn3.GetString() ?? string.Empty : string.Empty),
                            LastName = p.TryGetProperty("apellido", out var pa) ? pa.GetString() ?? string.Empty : string.Empty,
                            Email = p.TryGetProperty("email", out var pe) ? pe.GetString() ?? string.Empty : string.Empty,
                            Domicilie = new Domain.Entities.Domicilie { Street = "", Number = 0, Locality = "" }
                        };
                        await _patientRepository.AddPatient(newPatient);
                        income.Patient = newPatient;
                    }
                }

                // Resolve nurse if provided
                if (newIncome.nurse.ValueKind != System.Text.Json.JsonValueKind.Undefined && newIncome.nurse.ValueKind != System.Text.Json.JsonValueKind.Null)
                {
                    var n = newIncome.nurse;
                    if (n.TryGetProperty("id", out var nid) && nid.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var idStr = nid.GetString();
                        if (!string.IsNullOrEmpty(idStr))
                        {
                            income.Nurse = new Domain.Entities.Nurse { Id = Guid.TryParse(idStr, out var g) ? g : Guid.NewGuid() };
                        }
                    }
                }

                // Use a single DB connection + transaction for create patient (if needed) + insert income
                using (var conn = (MySql.Data.MySqlClient.MySqlConnection)_sqlConnection.CreateConnection())
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        try
                        {
                            // If patient not resolved yet, try non-transactional lookups first
                            if (income.Patient == null && newIncome.patient.ValueKind != System.Text.Json.JsonValueKind.Null && newIncome.patient.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                            {
                                var p = newIncome.patient;
                                if (p.TryGetProperty("id", out var pid) && pid.ValueKind == System.Text.Json.JsonValueKind.String)
                                {
                                    var idStr = pid.GetString();
                                    if (!string.IsNullOrEmpty(idStr))
                                    {
                                        var found = await _patientRepository.GetByGuid(idStr);
                                        if (found != null) income.Patient = found;
                                    }
                                }
                                if (income.Patient == null && p.TryGetProperty("cuil", out var pcuil) && pcuil.ValueKind == System.Text.Json.JsonValueKind.String)
                                {
                                    var cuil = pcuil.GetString();
                                    var foundList = await _patientRepository.GetByCuil(cuil);
                                    if (foundList != null && foundList.Count > 0) income.Patient = foundList.First();
                                }
                            }

                            // If still no patient, create within transaction
                            if (income.Patient == null && newIncome.patient.ValueKind != System.Text.Json.JsonValueKind.Null && newIncome.patient.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                            {
                                var p = newIncome.patient;
                                var newPatient = new Domain.Entities.Patient
                                {
                                    Id = Guid.NewGuid(),
                                    Name = p.TryGetProperty("nombre", out var pnom) ? pnom.GetString() ?? string.Empty : (p.TryGetProperty("name", out var pname) ? pname.GetString() ?? string.Empty : string.Empty),
                                    LastName = p.TryGetProperty("apellido", out var pap) ? pap.GetString() ?? string.Empty : string.Empty,
                                    Email = p.TryGetProperty("email", out var pmail) ? pmail.GetString() ?? string.Empty : string.Empty,
                                    Domicilie = new Domain.Entities.Domicilie { Street = p.TryGetProperty("street", out var pst) ? pst.GetString() ?? string.Empty : string.Empty, Number = 0, Locality = "" }
                                };
                                await ((IngSw_Tfi.Data.Repositories.PatientRepository)_patientRepository).AddPatient(newPatient, conn, tx);
                                income.Patient = newPatient;
                            }

                            // Insert income using same connection/transaction
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
                var emptyPatient = new PatientDto.Response(income.Patient?.Id ?? Guid.Empty, income.Patient?.Cuil?.Value ?? string.Empty, income.Patient?.Name ?? string.Empty, income.Patient?.LastName ?? string.Empty, income.Patient?.Email ?? string.Empty, income.Patient?.Domicilie?.Street ?? string.Empty, income.Patient?.Domicilie?.Number ?? 0, income.Patient?.Domicilie?.Locality ?? string.Empty);
                return new IncomeDto.Response(emptyPatient);
            }
            catch (Exception ex)
            {
                // Bubble up exception (handled by middleware)
                throw;
            }
        });
        return t;
    }

    public async Task<List<IncomeDto.Response>?> GetAllEarrings()
    {
        var incomesEarrings = await _incomeRepository.GetAllEarrings();
        if (incomesEarrings == null || incomesEarrings.Count == 0) return new List<IncomeDto.Response>();
        var result = incomesEarrings.Select(i =>
        {
            var patientDto = new PatientDto.Response(i.Patient?.Id ?? Guid.Empty, i.Patient?.Cuil?.Value ?? string.Empty, i.Patient?.Name ?? string.Empty, i.Patient?.LastName ?? string.Empty,
                i.Patient?.Email ?? string.Empty, i.Patient?.Domicilie?.Street ?? string.Empty, i.Patient?.Domicilie?.Number ?? 0, i.Patient?.Domicilie?.Locality ?? string.Empty);
            return new IncomeDto.Response(patientDto);
        }).ToList();
        return result;
    }

    public async Task<IncomeDto.Response?> GetById(int idIncome)
    {
        var income = await _incomeRepository.GetById(idIncome);
        if (income == null) return null;
        var patientDto = new PatientDto.Response(income.Patient?.Id ?? Guid.Empty, income.Patient?.Cuil?.Value ?? string.Empty, income.Patient?.Name ?? string.Empty, income.Patient?.LastName ?? string.Empty,
            income.Patient?.Email ?? string.Empty, income.Patient?.Domicilie?.Street ?? string.Empty, income.Patient?.Domicilie?.Number ?? 0, income.Patient?.Domicilie?.Locality ?? string.Empty);
        return new IncomeDto.Response(patientDto);
    }
    public async Task<List<IncomeDto.Response>?> GetAll()
    {
        var incomes = await _incomeRepository.GetAllEarrings();
        if (incomes == null || incomes.Count == 0) return new List<IncomeDto.Response>();
        return incomes.Select(i =>
        {
            var patientDto = new PatientDto.Response(i.Patient?.Id ?? Guid.Empty, i.Patient?.Cuil?.Value ?? string.Empty, i.Patient?.Name ?? string.Empty, i.Patient?.LastName ?? string.Empty,
                i.Patient?.Email ?? string.Empty, i.Patient?.Domicilie?.Street ?? string.Empty, i.Patient?.Domicilie?.Number ?? 0, i.Patient?.Domicilie?.Locality ?? string.Empty);
            return new IncomeDto.Response(patientDto);
        }).ToList();
    }
}
