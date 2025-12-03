namespace IngSw_Tfi.Application.DTOs;

public class SocialWorkDto
{
    public record Validate(string name, int memberNumber);
    public record Response(Guid id, string name);
}
