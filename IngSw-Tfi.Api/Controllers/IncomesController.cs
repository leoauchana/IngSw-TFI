using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var listIncomes = await _incomesService.GetAll();
        return Ok(new
        {
            Message = "Ingresos registrados",
            listIncomes
        });
    }
    [HttpGet("getById{idIncome}")]
    public async Task<IActionResult> GetById(int idIncome)
    {
        var incomeFound = await _incomesService.GetById(idIncome);
        return Ok(new
        {
            Message= "Ingreso encontrado",
            incomeFound
        });
    }
    public async Task<IActionResult> AddIncome([FromBody] IncomeDto.Request newIncome)
    {
        var incomeRegistered = await _incomesService.AddIncome(newIncome);
        return Ok(new
        {
            Message = "Ingreso registrado con éxito",
            incomeRegistered
        });
    }
}