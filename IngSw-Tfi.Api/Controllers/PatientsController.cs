using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IngSw_Tfi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController : ControllerBase
{
    private readonly IPatientsService _patientsService;
    public PatientsController(IPatientsService patientsService)
    {
        _patientsService = patientsService;
    }
    [HttpGet("getByCuil{cuilPatient}")]
    public async Task<IActionResult> GetByCuil(string cuilPatient)
    {
        var patientsFound = await _patientsService.GetByCuil(cuilPatient);
        if (patientsFound == null) return BadRequest("Hubo un error al obtener los pacientes.");
        return Ok(new
        {
            Message = "Pacientes encontrados.",
            patientsFound
        });
    } 
    [HttpPost]
    public async Task<IActionResult> AddPatient([FromBody] PatientDto.Request patientDto)
    {
        var newPatient = await _patientsService.AddPatient(patientDto);
        if (newPatient == null) 
            return BadRequest($"Hubo un error al registrar al paciente de cuil {patientDto.cuilPatient}.");
        return Ok(new
        {
            Message = "Paciente registrado con éxito.",
            newPatient
        });
    }
}
