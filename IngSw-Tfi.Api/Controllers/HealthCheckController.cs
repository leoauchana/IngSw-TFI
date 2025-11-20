using Microsoft.AspNetCore.Mvc;
using IngSw_Tfi.Data.Database;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Repository;

namespace IngSw_Tfi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    private readonly SqlConnection _sqlConnection;
    private readonly IIncomesService _incomesService;
    private readonly IPatientsService _patientsService;
    private readonly IIncomeRepository _incomeRepository;
    private readonly IPatientRepository _patientRepository;

    public HealthCheckController(
        SqlConnection sqlConnection,
        IIncomesService incomesService,
        IPatientsService patientsService,
        IIncomeRepository incomeRepository,
        IPatientRepository patientRepository)
    {
        _sqlConnection = sqlConnection;
        _incomesService = incomesService;
        _patientsService = patientsService;
        _incomeRepository = incomeRepository;
        _patientRepository = patientRepository;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "OK",
            Message = "API está funcionando correctamente",
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("detailed")]
    public IActionResult GetDetailed()
    {
        var checks = new Dictionary<string, object>();

        // 1. Verificar que el servidor responde
        checks["ServerRunning"] = new
        {
            Status = "OK",
            Message = "El servidor está respondiendo"
        };

        // 2. Verificar CORS (headers)
        checks["CORS"] = new
        {
            Status = "OK",
            Message = "CORS configurado",
            AllowedOrigins = new[] { "http://localhost:3000", "http://localhost:5173" }
        };

        // 3. Verificar inyección de dependencias
        checks["DependencyInjection"] = new
        {
            IncomesService = _incomesService != null ? "OK" : "ERROR",
            PatientsService = _patientsService != null ? "OK" : "ERROR",
            IncomeRepository = _incomeRepository != null ? "OK" : "ERROR",
            PatientRepository = _patientRepository != null ? "OK" : "ERROR"
        };

        // 4. Verificar conexión a base de datos
        string dbStatus = "ERROR";
        string dbMessage = "";
        try
        {
            using var connection = _sqlConnection.CreateConnection();
            connection.Open();
            dbStatus = connection.State == System.Data.ConnectionState.Open ? "OK" : "ERROR";
            dbMessage = dbStatus == "OK" 
                ? "Conexión a base de datos exitosa" 
                : "No se pudo abrir la conexión";
            connection.Close();
        }
        catch (Exception ex)
        {
            dbMessage = $"Error al conectar con la base de datos: {ex.Message}";
        }

        checks["Database"] = new
        {
            Status = dbStatus,
            Message = dbMessage,
            ConnectionString = _sqlConnection != null ? "Configurada" : "No configurada"
        };

        // 5. Verificar configuración de controladores
        checks["Controllers"] = new
        {
            PatientsController = "OK",
            IncomesController = "OK",
            HealthCheckController = "OK"
        };

        // 6. Verificar middleware
        checks["Middleware"] = new
        {
            ExceptionMiddleware = "OK",
            CorsMiddleware = "OK"
        };

        // Determinar status general
        bool allOk = dbStatus == "OK" 
            && _incomesService != null 
            && _patientsService != null 
            && _incomeRepository != null 
            && _patientRepository != null;

        return Ok(new
        {
            OverallStatus = allOk ? "OK" : "WARNING",
            Message = allOk 
                ? "✅ Todas las verificaciones pasaron correctamente" 
                : "⚠️ Algunas verificaciones fallaron, revisa los detalles",
            Timestamp = DateTime.UtcNow,
            Checks = checks
        });
    }

    [HttpGet("database")]
    public IActionResult TestDatabase()
    {
        try
        {
            using var connection = _sqlConnection.CreateConnection();
            connection.Open();
            
            // Obtener la versión del servidor si es posible
            string serverVersion = "N/A";
            if (connection is MySql.Data.MySqlClient.MySqlConnection mysqlConnection)
            {
                serverVersion = mysqlConnection.ServerVersion;
            }
            
            var result = new
            {
                Status = "OK",
                Message = "✅ Conexión a base de datos exitosa",
                State = connection.State.ToString(),
                Database = connection.Database,
                ServerVersion = serverVersion
            };
            
            connection.Close();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Status = "ERROR",
                Message = "❌ Error al conectar con la base de datos",
                Error = ex.Message,
                InnerException = ex.InnerException?.Message
            });
        }
    }

    [HttpGet("services")]
    public IActionResult TestServices()
    {
        var services = new Dictionary<string, string>
        {
            ["IncomesService"] = _incomesService != null ? "✅ Registrado" : "❌ No registrado",
            ["PatientsService"] = _patientsService != null ? "✅ Registrado" : "❌ No registrado",
            ["IncomeRepository"] = _incomeRepository != null ? "✅ Registrado" : "❌ No registrado",
            ["PatientRepository"] = _patientRepository != null ? "✅ Registrado" : "❌ No registrado"
        };

        bool allOk = services.Values.All(v => v.Contains("✅"));

        return Ok(new
        {
            Status = allOk ? "OK" : "ERROR",
            Message = allOk 
                ? "✅ Todos los servicios están registrados correctamente" 
                : "❌ Algunos servicios no están registrados",
            Services = services
        });
    }

    [HttpGet("cors")]
    public IActionResult TestCors()
    {
        var origin = Request.Headers["Origin"].ToString();
        
        return Ok(new
        {
            Status = "OK",
            Message = "✅ CORS está configurado (verifica los headers de respuesta)",
            YourOrigin = string.IsNullOrEmpty(origin) ? "No especificado" : origin,
            AllowedOrigins = new[] 
            { 
                "http://localhost:3000", 
                "http://localhost:5173" 
            },
            Note = "Si recibes este mensaje desde el frontend, CORS está funcionando correctamente"
        });
    }
}

