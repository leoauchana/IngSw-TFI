using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;

namespace IngSw_Tfi.Application.DTOs;

public class IncomeDto
{
    public record Request(string report, EmergencyLevel emergencyLevel, float temperature, float frecyencyCardiac, 
        float frecuencyRespiratory, float frecuencySystolic, float frecuencyDiastolic, string cuilPatient);
    public record Response(Patient patient);
}
