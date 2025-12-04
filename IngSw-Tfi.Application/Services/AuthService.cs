using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;

namespace IngSw_Tfi.Application.Services;

public class AuthService : IAuthService
{
private readonly IEmployeeRepository _employeeRepository;
    private readonly IConfiguration _configuration;
    public AuthService(IEmployeeRepository employeeRepository, IConfiguration configuration)
    {
        _employeeRepository = employeeRepository;
        _configuration = configuration;
    }
    public async Task<UserDto.Response?> Login(UserDto.RequestUser? userData)
    {
        if (string.IsNullOrWhiteSpace(userData!.email) || string.IsNullOrWhiteSpace(userData!.password)) throw new ArgumentException("Debe ingresar correctamente los datos");
        var employeeFound = await _employeeRepository.GetByEmail(userData.email);
        if (employeeFound == null || !VerifyPassword(userData!.password!, employeeFound.User!.Password!)) throw new EntityNotFoundException("Usuario o contraseña incorrecto.");
        var token = TokenGenerator(employeeFound);
        return new UserDto.Response
        (
            employeeFound.Id.ToString() ?? string.Empty,
            employeeFound.Email!,
            employeeFound.Name!,
            employeeFound.LastName!,
            employeeFound.Cuil?.Value ?? string.Empty,
            employeeFound.Registration ?? string.Empty,
            employeeFound.PhoneNumber ?? string.Empty,
            GetRoleName(employeeFound),
            token
        );
    }
    public async Task<UserDto.Response?> Register(UserDto.RequestRegister? userData)
    {
        ValidateRegisterData(userData!);

        Employee newEmployee = CreateEmployee(userData!);

        var employeeRegistered = await _employeeRepository.Register(newEmployee);

        if (employeeRegistered == null)
            return null;

        return MapToResponse(employeeRegistered);
    }
    private string TokenGenerator(Employee user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.User!.Id.ToString() ?? string.Empty),
            new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
            new Claim(ClaimTypes.Role, user.GetType().Name ?? string.Empty)
        };

        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
        var issuer = _configuration["Jwt:Issuer"] ?? string.Empty;
        var audience = _configuration["Jwt:Audience"] ?? string.Empty;
        var expiresMinutesStr = _configuration["Jwt:ExpiresMinutes"];
        var expiresMinutes = int.TryParse(expiresMinutesStr, out var m) ? m : 60;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
    private bool VerifyPassword(string passwordInput, string hashedPassword) => BCrypt.Net.BCrypt.Verify(passwordInput, hashedPassword);
    
    private string GetRoleName(Employee employee)
    {
        return employee switch
        {
            Doctor => "Doctor",
            Nurse => "Enfermera",
            _ => employee.GetType().Name
        };
    }
    private void ValidateRegisterData(UserDto.RequestRegister userData)
    {
        var campos = new Dictionary<string, object?>
    {
        { "Email",              userData.email },
        { "Contraseña",         userData.password },
        { "Confirmacion",       userData.confirmPassword },
        { "Nombre",             userData.name },
        { "Apellido",           userData.lastName },
        { "Cuil",               userData.cuil },
        { "Licencia",           userData.licence },
        { "Numero de Telefono", userData.phoneNumber },
        { "Tipo de Empleado",   userData.typeEmployee }
    };

        foreach (var campo in campos)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(campo.Value)))
                throw new ArgumentException($"El campo '{campo.Key}' no puede ser omitido.");
        }

        if (userData.password != userData.confirmPassword)
            throw new ArgumentException("Las contraseñas no coinciden.");
    }
    private Employee CreateEmployee(UserDto.RequestRegister userData)
    {
        var passwordHashed = BCrypt.Net.BCrypt.HashPassword(userData.password);

        var baseEmployee = new
        {
            userData.name,
            userData.lastName,
            userData.email,
            cuil = Cuil.Create(userData.cuil),
            userData.phoneNumber,
            userData.licence
        };

        return userData.typeEmployee.Equals("doctor", StringComparison.OrdinalIgnoreCase)
            ? new Doctor
            {
                Name = baseEmployee.name,
                LastName = baseEmployee.lastName,
                Cuil = baseEmployee.cuil,
                Email = baseEmployee.email,
                PhoneNumber = baseEmployee.phoneNumber,
                Registration = baseEmployee.licence,
                User = new User
                {
                    Email = userData.email,
                    Password = passwordHashed
                }
            }
            : new Nurse
            {
                Name = baseEmployee.name,
                LastName = baseEmployee.lastName,
                Cuil = baseEmployee.cuil,
                Email = baseEmployee.email,
                PhoneNumber = baseEmployee.phoneNumber,
                Registration = baseEmployee.licence,
                User = new User
                {
                    Email = userData.email,
                    Password = passwordHashed
                }
            };
    }
    private UserDto.Response MapToResponse(Employee employee)
    {
        return new UserDto.Response(
            employee.Id.ToString(),
            employee.Email!,
            employee.Name!,
            employee.LastName!,
            employee.Cuil!.Value!,
            employee.Registration!,
            employee.PhoneNumber!,
            employee.GetType().Name,//GetRoleName(employee),
            string.Empty
        );
    }
}
