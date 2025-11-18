using IngSw_Tfi.Domain.Enums;

namespace IngSw_Tfi.Application.DTOs;

public class IncomeDto
{
    public record Request(string report, EmergencyLevel emergencyLevel, float temperature, float frecyencyCardiac, 
        float frecuencyRespiratory, float frecuencySystolic, float frecuencyDiastolic);
    public record Response(PatientDto patient);
}
