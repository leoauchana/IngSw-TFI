using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static IngSw_Tfi.Api.Controllers.IncomesController;

namespace IngSw_Tfi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttentionController : ControllerBase
{
    private readonly IAttentionService _attentionService;
    public AttentionController(IAttentionService attentionService)
    {
        _attentionService = attentionService;
    }
    [HttpPost]
    public async Task<IActionResult> AddAttention([FromBody] AttentionDto.Request newIncome)
    {
        var idUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (idUser == null) return BadRequest("No se pudo obtener el ID del empleado.");
        var incomeRegistered = await _attentionService.AddAttention(idUser, newIncome);
        if (incomeRegistered == null) return BadRequest();
        return Ok(new
        {
            Message = "Ingreso registrado con éxito",
            incomeRegistered
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIncomeStatus(string id, [FromBody] UpdateStatusRequest request)
    {
            var result = await _attentionService.UpdateIncomeStatus(id, request.estado);
            if (result == null) return NotFound(new { message = "Ingreso no encontrado" });
            return Ok(new { Message = "Estado actualizado correctamente", data = result });
    }
}
