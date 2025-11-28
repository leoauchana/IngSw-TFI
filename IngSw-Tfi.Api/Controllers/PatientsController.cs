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
    // Nuevo endpoint: GET /api/patients
    // Soporta filtrado opcional por DNI mediante query parameter ?dni=12345678
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? dni = null)
    {
        var list = await _patientsService.GetAll();
        
        // Si no se proporciona DNI, devolver todos los pacientes
        if (string.IsNullOrWhiteSpace(dni))
        {
            return Ok(list ?? new List<PatientDto.Response>());
        }
        
        // Filtrar por DNI extraído del CUIL (formato: XX-DNI-X)
        var filtered = list?.Where(p =>
        {
            if (string.IsNullOrWhiteSpace(p.cuilPatient))
                return false;
                
            var parts = p.cuilPatient.Split('-');
            if (parts.Length != 3)
                return false;
                
            return parts[1] == dni;
        }).ToList() ?? new List<PatientDto.Response>();
        
        return Ok(filtered);
    }

    // Nuevo endpoint: GET /api/patients/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var patient = await _patientsService.GetById(id);
        if (patient == null) return NotFound();
        return Ok(patient);
    }
}
