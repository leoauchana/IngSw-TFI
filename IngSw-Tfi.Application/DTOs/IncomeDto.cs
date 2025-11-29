using IngSw_Tfi.Domain.Enums;
using System.Text.Json;

namespace IngSw_Tfi.Application.DTOs;

public class IncomeDto
{
    public record Request(
        string report,
        EmergencyLevel emergencyLevel,
        float temperature,
        float frecyencyCardiac,
        float frecuencyRespiratory,
        float frecuencySystolic,
        float frecuencyDiastolic,
        JsonElement patient,
        JsonElement nurse
    );
    // Response contiene el DTO anidado de paciente y datos de la admisión
    public record Response(
        string id,
        PatientDto.Response paciente,
        DateTime fechaIngreso,
        EmergencyLevelDto nivelEmergencia,
        StatusDto estado,
        float? temperature,
        float? heartRate,
        float? respiratoryRate,
        float? systolicRate,
        float? diastolicRate,
        string? report,
        NurseDto? enfermera
    );

    public record EmergencyLevelDto(int id, string label);
    public record StatusDto(string id, string label);
    public record NurseDto(string id, string nombre, string apellido, string? matricula);
}
