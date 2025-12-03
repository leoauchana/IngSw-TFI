using static IngSw_Tfi.Application.DTOs.IncomeDto;

namespace IngSw_Tfi.Application.DTOs;

public class AttentionDto
{
    public record Request(string idIncome, string report);
    public record Response(
    string id,
    IncomeDto.Response income,
    EmployeeDto.DoctorResponse doctor,
    string report);
}
