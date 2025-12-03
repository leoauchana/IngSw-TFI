using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Data.DAOs;
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

    //[HttpGet]
    //public async Task<IActionResult> GetAll1()
    //{
    //    try
    //    {
    //        var results = await _healthInsuranceDao.GetAll();
    //        var insurances = results.Select(r => new
    //        {
    //            id = r.GetValueOrDefault("id_health_insurance")?.ToString(),
    //            name = r.GetValueOrDefault("name")?.ToString(),
    //            memberNumber = Convert.ToInt32(r.GetValueOrDefault("member_number") ?? 0)
    //        }).ToList();

    //        return Ok(insurances);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, new { message = "Error al obtener obras sociales", error = ex.Message });
    //    }
    //}

    //[HttpPost("validate1")]
    //public async Task<IActionResult> ValidateInsuranceAndMember1([FromBody] ValidationRequest request)
    //{
    //    try
    //    {
    //        if (string.IsNullOrWhiteSpace(request.Name) || request.MemberNumber <= 0)
    //        {
    //            return BadRequest(new { message = "Nombre de obra social y número de afiliado son requeridos" });
    //        }

    //        var result = await _healthInsuranceDao.GetByNameAndMemberNumber(request.Name, request.MemberNumber);

    //        if (result == null)
    //        {
    //            return Ok(new
    //            {
    //                isValid = false,
    //                message = "La obra social y el número de afiliado no coinciden"
    //            });
    //        }

    //        return Ok(new
    //        {
    //            isValid = true,
    //            id = result.GetValueOrDefault("id_health_insurance")?.ToString()
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, new { message = "Error al validar datos", error = ex.Message });
    //    }
    //}

    //public class ValidationRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public int MemberNumber { get; set; }
    //}
}
