using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IngSw_Tfi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto.Request credentials)
    {
            var user = await _authService.Login(credentials);
            if (user == null) return Unauthorized(new { message = "Usuario o contraseña inválidos" });
            return Ok(user);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto.RequestRegister newUser)
    {
            var registered = await _authService.Register(newUser);
            if (registered == null) return BadRequest(new { message = "No se pudo registrar el usuario." });
            return Ok(registered);
}


