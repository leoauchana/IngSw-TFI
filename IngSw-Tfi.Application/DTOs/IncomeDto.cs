using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;
using static IngSw_Tfi.Application.DTOs.PatientDto;

namespace IngSw_Tfi.Application.DTOs;

public class IncomeDto
{
    public record Request(string report, EmergencyLevel emergencyLevel, float temperature, float frecuencyCardiac, 
        float frecuencyRespiratory, float frecuencySystolic, float frecuencyDiastolic, string cuilPatient);
    public record Response(Patient patient, EmergencyLevel emergencyLevel, DateOnly incomeDate, TimeSpan timeWait);

    public record ResponseTest(string report, EmergencyLevel emergencyLevel, DateOnly incomeDate, TimeSpan timeWait,
        string status, float temperature, float frecuencyCardiac, float frecuencyRespiratory, float frecuencySystolic, 
        float frecuencyDiastolic, ResponseIncome patient);
}
