using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;
using Org.BouncyCastle.Crypto.Generators;

namespace IngSw_Tfi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IEmployeeRepository _employeeRepository;
    //private readonly IConfiguration _configuration;
    public AuthService(IEmployeeRepository employeeRepository/*, IConfiguration configuration*/)
    {
        _employeeRepository = employeeRepository;
        // _configuration = configuration;
    }
    public async Task<UserDto.Response?> Login(UserDto.Request? userData)
    {
        if (string.IsNullOrWhiteSpace(userData!.email) || string.IsNullOrWhiteSpace(userData!.password)) throw new ArgumentException("Debe ingresar correctamente los datos");
        var employeeFound = await _employeeRepository.GetByEmail(userData.email);
        if (employeeFound == null || !VerifyPassword(userData!.password!, employeeFound.User!.Password!)) throw new EntityNotFoundException("Usuario o contraseña incorrecto.");
        return new UserDto.Response
        (
            employeeFound.Email!,
            employeeFound.Name!,
            employeeFound.LastName!,
            employeeFound.Registration!,
            employeeFound.PhoneNumber!,
            employeeFound.GetType().Name,
            ""
        // TokenGenerator(userFound)
        );
    }
    public async Task<UserDto.Response?> Register(UserDto.RequestRegister? userData)
    {
        var campos = new Dictionary<string, object?>
        {
            { "Email",              userData?.email },
            { "Contraseña",         userData?.password },
            { "Confirmacion",       userData?.confirmPassword },
            { "Nombre",             userData?.name },
            { "Apellido",           userData?.lastName },
            { "Cuil",               userData?.cuil },
            { "Licencia",           userData?.licence },
            { "Numero de Telefono", userData?.phoneNumber },
            { "Tipo de Empleado",   userData?.typeEmployee }
        };
        foreach (var campo in campos)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(campo.Value)))
                throw new ArgumentException($"El campo '{campo.Key}' no puede ser omitido.");
        }
        if (userData!.password != userData.confirmPassword)
            throw new ArgumentException("Las contraseñas no coinciden.");

        Employee newEmployee = userData.typeEmployee.ToLower().Equals("doctor") ? new Doctor()
        {
            Name = userData.name,
            LastName = userData.lastName,
            Cuil = Cuil.Create(userData.cuil),
            Email = userData.email,
            PhoneNumber = userData.phoneNumber,
            Registration = userData.licence,
            User = new User
            {
                Email = userData.email,
                Password = BCrypt.Net.BCrypt.HashPassword(userData!.password)
            }
        } : new Nurse()
        {
            Name = userData.name,
            LastName = userData.lastName,
            Cuil = Cuil.Create(userData.cuil),
            Email = userData.email,
            PhoneNumber = userData.phoneNumber,
            User = new User
            {
                Email = userData.email,
                Password = BCrypt.Net.BCrypt.HashPassword(userData!.password)
            }
        };

        var employeeRegistered = await _employeeRepository.Register(newEmployee);

        return employeeRegistered != null ? new UserDto.Response
        (
            employeeRegistered.Email!,
            employeeRegistered.Name!,
            employeeRegistered.LastName!,
            employeeRegistered.Cuil!.Value!,
            employeeRegistered.Registration!,
            employeeRegistered.PhoneNumber!,
            employeeRegistered.GetType().Name
        ) : null;
    }
    //private string TokenGenerator(User user)
    //{
    //    var userClaims = new[]
    //    {
    //        new Claim(ClaimTypes.NameIdentifier, user.Employee!.Id.ToString()!),
    //        new Claim(ClaimTypes.Name, user.Employee!.Name!),
    //        new Claim(ClaimTypes.Role, user.Employee.GetType().Name!)
    //    };

    //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
    //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

    //    var jwtConfig = new JwtSecurityToken(
    //        claims: userClaims,
    //        expires: DateTime.UtcNow.AddMinutes(10),
    //        signingCredentials: credentials
    //        );
    //    return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
    //}
    private bool VerifyPassword(string passwordInput, string hashedPassword) => BCrypt.Net.BCrypt.Verify(passwordInput, hashedPassword);
}
