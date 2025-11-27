namespace IngSw_Tfi.Application.DTOs;

public class UserDto
{
    public record Request(string? email, string? password);
    // Response ahora incluye el id del empleado y el token JWT como último campo (puede ser vacío si no se genera)
    public record Response(string id, string email, string name, string lastName, string cuil,
        string licence, string phoneNumber, string typeEmployee, string token);
    public record RequestRegister(string email, string password, string confirmPassword, string name, string lastName, string cuil,
        string licence, string phoneNumber, string typeEmployee);
}
