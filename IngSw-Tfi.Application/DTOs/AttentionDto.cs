namespace IngSw_Tfi.Application.DTOs;

public class AttentionDto
{
    public record Request(string idIncome, string report);
    public record Response();
}
