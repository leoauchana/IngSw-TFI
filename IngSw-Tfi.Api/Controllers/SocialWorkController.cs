using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Data.DAOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IngSw_Tfi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SocialWorkController : ControllerBase
{
    private readonly ISocialWorkService _socialWorkService;

    public SocialWorkController(ISocialWorkService socialWorkService)
    {
        _socialWorkService = socialWorkService;
    }
    [Authorize(Policy = "Nurse")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var socialsWork = await _socialWorkService.GetAll();
        if (socialsWork == null) return BadRequest("Hubo un error al obtener las obras sociales.");
        return Ok(socialsWork);
    }
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateInsuranceAndMember1([FromBody] SocialWorkDto.Validate socialData)
    {
        var result = await _socialWorkService.ValidateInsuranceAndMember(socialData);
        if (!result)
        {
            return Ok(new
            {
                isValid = false,
                message = "El paciente no es afiliado de la obra social."
            });
        }
        return Ok(new
        {
            isValid = true,
            message = "El paciente es afiliado de la obra social."
        });
    }
}
