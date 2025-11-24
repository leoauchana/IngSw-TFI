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
    public async Task<IActionResult> Login(UserDto.Request userData)
    {
        var userFound = await _authService.Login(userData);
        if (userFound == null) return BadRequest("Hubo un error al autenticar el usuario.");
        return Ok(new
        {
            Message = "Inicio de sesión exitoso.",
            userFound
        });
    }
}
