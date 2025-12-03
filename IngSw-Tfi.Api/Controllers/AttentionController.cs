using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Application.Services;
using Microsoft.AspNetCore.Mvc;
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
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIncomeStatus(string id, [FromBody] UpdateStatusRequest request)
    {
            var result = await _attentionService.UpdateIncomeStatus(id, request.estado);
            if (result == null) return NotFound(new { message = "Ingreso no encontrado" });
            return Ok(new { Message = "Estado actualizado correctamente", data = result });
    }
}
