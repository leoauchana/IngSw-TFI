using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "All")]
    [HttpGet("getByCuil/{cuilPatient}")]
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
    [Authorize(Policy = "All")]
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll()
    {
        var patientsList = await _patientsService.GetAll();
        if (patientsList == null) return BadRequest("Hubo un error al obtener los pacientes.");
        return Ok(new
        {
            Message = "Pacientes registrados.",
            patientsList
        });
    }
    [Authorize(Policy = "All")]
    [HttpPost]
    public async Task<IActionResult> AddPatient([FromBody] PatientDto.RequestPatient patientDto)
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
    [Authorize(Policy = "All")]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? dni = null)
    {
        List<PatientDto.Response>? patientsList = null;
        if (string.IsNullOrEmpty(dni))
        {
            patientsList = await _patientsService.GetAll();
        }
        else
        {
            patientsList = await _patientsService.GetByDni(dni);
        }
        if (patientsList == null) return BadRequest("Hubo un error al obtener los pacientes.");
        return Ok(patientsList);
    }

    //// Nuevo endpoint: GET /api/patients/{id}
    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetById(int id)
    //{
    //    var patient = await _patientsService.GetById(id);
    //    if (patient == null) return NotFound();
    //    return Ok(patient);
    //}
}
