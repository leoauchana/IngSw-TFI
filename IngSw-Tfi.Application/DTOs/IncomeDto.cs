using IngSw_Tfi.Domain.Enums;

namespace IngSw_Tfi.Application.DTOs;

public class IncomeDto
{
    public record Request(
        string report,
        EmergencyLevel emergencyLevel,
        float temperature,
        float frecuencyCardiac,
        float frecuencyRespiratory,
        float frecuencySystolic,
        float frecuencyDiastolic,
        string idPatient,
        string idNurse
    );
    public record RequestT(
        string report,
        EmergencyLevel emergencyLevel,
        float temperature,
        float frecuencyCardiac,
        float frecuencyRespiratory,
        float frecuencySystolic,
        float frecuencyDiastolic,
        string idPatient
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
        EmployeeDto.NurseResponse? enfermera
    );

    public record ResponseShirt(
        string id,
        DateTime fechaIngreso,
        EmergencyLevelDto nivelEmergencia,
        StatusDto estado,
        float? temperature,
        float? heartRate,
        float? respiratoryRate,
        float? systolicRate,
        float? diastolicRate,
        string? report,
        EmployeeDto.NurseResponse? enfermera);

    public record UpdateStatusRequest(string estado);
    public record EmergencyLevelDto(int id, string label);
    public record StatusDto(string id, string label);
}
