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
    // Response contiene el DTO anidado de paciente (PatientDto.Response)
    public record Response(PatientDto.Response patient);
}
