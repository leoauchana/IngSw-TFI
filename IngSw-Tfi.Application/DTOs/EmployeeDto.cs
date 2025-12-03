namespace IngSw_Tfi.Application.DTOs;

public class EmployeeDto
{
    public record DoctorResponse(string id, string nombre, string apellido, string? matricula);
    public record NurseResponse(string id, string nombre, string apellido, string? matricula);
}
