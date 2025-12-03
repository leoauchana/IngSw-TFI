using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IngSw_Tfi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IncomesController : ControllerBase
{
    private readonly IIncomesService _incomesService;
    public IncomesController(IIncomesService incomesService)
    {
        _incomesService = incomesService;
    }

    // GET /api/incomes
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var listIncomes = await _incomesService.GetAll();
        return Ok(listIncomes ?? new List<IncomeDto.Response>());
    }

    [HttpGet("getAllEarrings")]
    public IActionResult GetAllEarrings()
    {
        var listIncomes =  _incomesService.GetAllEarrings();
        return Ok(listIncomes);
    }

    // GET /api/incomes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var incomeFound = await _incomesService.GetById(id);
        if (incomeFound == null) return NotFound();
        return Ok(incomeFound);
    }

    // POST /api/incomes
    [HttpPost("add")]
    public async Task<IActionResult> AddIncome([FromBody] IncomeDto.RequestT newIncome)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return BadRequest("No se puedo obtener el ID del empleado.");
        var incomeRegistered = await _incomesService.AddIncome(userId, newIncome);
        if (incomeRegistered == null) return BadRequest();
        return Ok(new
        {
            Message = "Ingreso registrado con éxito",
            incomeRegistered
        });
    }
}