using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;

namespace IngSw_Bdd.StepDefinitions
{
    [Binding]
    public class ModeloDeUrgenciasStepDefinitions
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientsService _patientsService;
        private readonly IIncomesService _incomesService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISocialWorkRepository _socialWorkRepository;
        private Nurse? _nurse;
        private Patient? _patient;
        private Exception? _exceptionExpected;
        private string? _newIdPatient;
        public ModeloDeUrgenciasStepDefinitions(IIncomeRepository incomeRepository, IIncomesService incomesService,
            IPatientRepository patientRepository, IPatientsService patientsService,
            IEmployeeRepository employeeRepository, ISocialWorkRepository socialWorkRepository)
        {
            _incomeRepository = incomeRepository;
            _incomesService = incomesService;
            _patientRepository = patientRepository;
            _patientsService = patientsService;
            _employeeRepository = employeeRepository;
            _socialWorkRepository = socialWorkRepository;
        }

        //Scenary 1
        [Given("que la siguiente enfermera esta registrada:")]
        public async Task GivenQueLaSiguienteEnfermeraEstaRegistrada(DataTable dataTable)
        {
            _nurse = dataTable.CreateSet<Nurse>().FirstOrDefault();
            if (_nurse == null)
                throw new NullReferenceException("No se obtuvieron los datos de la enferemra registrada");
            await _employeeRepository.Register(_nurse);
        }
        [Given("que estan registrados los siguientes pacientes:")]
        public void GivenQueEstanRegistradosLosSiguientesPacientes(DataTable dataTable)
        {
            foreach (var row in dataTable.Rows)
            {
                var patient = new Patient
                {
                    Id = Guid.TryParse(row["Id"], out var id) ? id : Guid.NewGuid(),
                    Cuil = Cuil.Create(row["Cuil"]),
                    LastName = row["LastName"],
                    Name = row["Name"],
                };
                _patientRepository.AddPatient(patient);
            }
        }
        [When("ingreso a urgencias al siguiente paciente:")]
        public async Task WhenIngresoAUrgenciasAlSiguientePaciente(DataTable dataTable)
        {
            _exceptionExpected = null;
            var patientData = dataTable.Rows.FirstOrDefault();
            if (patientData == null)
                throw new NullReferenceException("No se obtuvieron los datos del ingreso del paciente");
            var idPatient = patientData["Id"];
            var cuilPatient = patientData!["Cuil"];
            var temperature = float.Parse(patientData["Temperatura"]);
            var report = patientData["Informe"];
            var frequencyCardiac = float.Parse(patientData["Frecuencia Cardiaca"]);
            var frequencyRespiratory = float.Parse(patientData["Frecuencia Respiratoria"]);
            EmergencyLevel? level = Enum.TryParse<EmergencyLevel>(
                patientData["Nivel de Emergencia"], true, out var parsedLevel)
                ? parsedLevel
                : null;
            var (frequencySystolic, frequensyDiastolic) = (patientData["Tension Arterial"].Split('/') is var p) ? (float.Parse(p[0]), float.Parse(p[1])) : (0, 0);
            var newIncome = new IncomeDto.RequestT(level, frequencyCardiac, frequensyDiastolic, frequencyRespiratory,
                frequencySystolic, idPatient, report, temperature);
            try
            {
                var incomeDto = await _incomesService.AddIncome(_nurse!.Id.ToString(), newIncome);
            }
            catch (Exception e)
            {
                _exceptionExpected = e;
            }
        }
        [Then("La lista de espera esta ordenada por cuil de la siguiente manera:")]
        public void ThenLaListaDeEsperaEstaOrdenadaPorCuilDeLaSiguienteManera(DataTable dataTable)
        {
            var expectedCuil = dataTable.Rows.FirstOrDefault();
            if (expectedCuil == null)
                throw new NullReferenceException("No se obtuvo el cuil esperado en la cola de espera");
            var incomesList =  _incomesService.GetAllEarrings();
            if (incomesList!.Count <= 0)
                throw new NullReferenceException("No se obtuvo la lista de ingresos");
            var cuilPendiente = incomesList!.FirstOrDefault()!.paciente.cuilPatient;

            Assert.Equal(expectedCuil["Cuil"], cuilPendiente);
        }
        // Scenary 2
        [Given("que no existe el paciente registrado")]
        public void GivenQueNoExisteElPacienteRegistrado()
        {
            _patient = null;
        }
        [Given("que estan registradas las siguientes obras sociales:")]
        public void GivenQueEstanRegistradasLasSiguientesObrasSociales(DataTable dataTable)
        {
            var socialWorks = dataTable.Rows;
            if (socialWorks == null)
                throw new NullReferenceException("No se obtuvieron los datos de las obras sociales a registrar");
            foreach (var row in dataTable.Rows)
            {
                var socialWork = new SocialWork
                {
                    Id = Guid.TryParse(row["Id"], out var id) ? id : Guid.NewGuid(),
                    Name = row["Name"]
                };
                _socialWorkRepository.AddSocialWork(socialWork);
            }
        }
        [When("registro al paciente a urgencias con los siguientes datos:")]
        public async Task WhenRegistroAlPacienteAUrgenciasConLosSiguientesDatos(DataTable dataTable)
        {
            var patientData = dataTable.Rows.FirstOrDefault();
            if (patientData == null)
                throw new NullReferenceException("No se obtuvieron los datos del paciente a registrar");

            var cuil = patientData["Cuil"];
            var name = patientData["Name"];
            var lastName = patientData["LastName"];
            var email = patientData["Email"];
            var phone = patientData["Phone"];

            var birthDate = DateTime.TryParse(patientData["BirthDate"], out var birth)
                ? birth
                : DateTime.UtcNow;

            var street = patientData["Street"];
            var number = int.TryParse(patientData["Number"], out var num) ? num : 0;
            var locality = patientData["Locality"];

            var affiliateNumber = patientData["AffiliateNumber"];

            var idSocialWork = patientData["IdSocialWork"];

            var newPatient = new PatientDto.Request(
                cuil,
                name,
                lastName,
                email ?? string.Empty,
                birthDate,
                phone ?? string.Empty,
                street ?? string.Empty,
                number,
                locality ?? string.Empty,
                idSocialWork,
                affiliateNumber ?? string.Empty
            );
            var newPatientDto = await _patientsService.AddPatient(newPatient);
            _newIdPatient = newPatientDto!.id;
        }
        [When("ingreso a urgencias el nuevo paciente registrado:")]
        public async Task WhenIngresoAUrgenciasElNuevoPacienteRegistrado(DataTable dataTable)
        {
            _exceptionExpected = null;
            var patientData = dataTable.Rows.FirstOrDefault();
            if (patientData == null)
                throw new NullReferenceException("No se obtuvieron los datos del ingreso del paciente");
            var idPatient = _newIdPatient;
            var cuilPatient = patientData!["Cuil"];
            var temperature = float.Parse(patientData["Temperatura"]);
            var report = patientData["Informe"];
            var frequencyCardiac = float.Parse(patientData["Frecuencia Cardiaca"]);
            var frequencyRespiratory = float.Parse(patientData["Frecuencia Respiratoria"]);
            EmergencyLevel level = Enum.TryParse<EmergencyLevel>(
                patientData["Nivel de Emergencia"], true, out var parsedLevel)
                ? parsedLevel
                : 0;
            var (frequencySystolic, frequensyDiastolic) = (patientData["Tension Arterial"].Split('/') is var p) ? (float.Parse(p[0]), float.Parse(p[1])) : (0, 0);
            var newIncome = new IncomeDto.RequestT(level, frequencyCardiac, frequensyDiastolic, frequencyRespiratory,
                frequencySystolic, idPatient, report, temperature);
            try
            {
                var incomeDto = await _incomesService.AddIncome(_nurse!.Id.ToString(), newIncome);
            }
            catch (Exception e)
            {
                _exceptionExpected = e;
            }
        }
        // Scenary 3
        [Then("se informa la falta del dato mandatario {string}")]
        public void ThenSeInformaLaFaltaDelDatoMandatario(string message)
        {
            Assert.Equal(message, _exceptionExpected!.Message);
        }
        [Then("La lista de espera no contendrá el cuil:")]
        public async Task ThenLaListaDeEsperaNoContendraElCuil(DataTable dataTable)
        {
            var expectedCuil = dataTable.Rows.FirstOrDefault();
            if (expectedCuil == null)
                throw new NullReferenceException("No se obtuvo el cuil esperado en la cola de espera");
            var incomesList = await _incomesService.GetAll();
            if (incomesList == null)
                throw new NullReferenceException("La lista de ingreso es nula");
            Assert.False(incomesList!.Any(i => i.paciente.cuilPatient == expectedCuil["Cuil"]));
        }
        // Scenary 4 
        [Then("se informa que la frecuencia respiratorio se cargo de forma incorrecta {string}")]
        public void ThenSeInformaQueLaFrecuenciaRespiratorioSeCargoDeFormaIncorrecta(string message)
        {
            Assert.Equal(message, _exceptionExpected!.Message);
        }
        //Scenary 5
        [Given("que la lista de espera actual ordenada por nivel es:")]
        public void GivenQueLaListaDeEsperaActualOrdenadaPorNivelEs(DataTable dataTable)
        {
            var patientsList = dataTable.Rows;
            if (patientsList == null)
                throw new NullReferenceException("No se obtuvieron los datos de la lista de espera actual");
            foreach (var patientRow in patientsList)
            {
                var cuilPatient = patientRow!["Id"];
                var temperature = float.Parse(patientRow["Temperatura"]);
                var report = patientRow["Informe"];
                var frequencyCardiac = float.Parse(patientRow["Frecuencia Cardiaca"]);
                var frequencyRespiratory = float.Parse(patientRow["Frecuencia Respiratoria"]);
                EmergencyLevel level = Enum.TryParse<EmergencyLevel>(
                    patientRow["Nivel de Emergencia"], true, out var parsedLevel)
                    ? parsedLevel
                    : 0;
                var (frequencySystolic, frequensyDiastolic) = (patientRow["Tension Arterial"].Split('/') is var p)
                    ? (float.Parse(p[0]), float.Parse(p[1]))
                    : (0, 0);
                var newIncome = new IncomeDto.RequestT(level, frequencyCardiac, frequensyDiastolic, frequencyRespiratory,
                frequencySystolic, cuilPatient, report, temperature);
                _incomesService.AddIncome(_nurse!.Id.ToString(), newIncome);
            }
        }

        [Then("La lista de espera esta ordenada por cuil considerando la prioridad de la siguiente manera:")]
        public void ThenLaListaDeEsperaEstaOrdenadaPorCuilConsiderandoLaPrioridadDeLaSiguienteManera(DataTable dataTable)
        {
            var cuilsListExpected = dataTable.Rows
                                .Select(r => r["Cuil"].ToString()!)
                                .ToList();
            if (cuilsListExpected == null)
                throw new NullReferenceException("No se obtuvo los cuil ordenados en la cola de espera");
            var incomes =  _incomesService.GetAllEarrings()!;
            if (incomes == null)
                throw new NullReferenceException("La lista de ingreso es nula");
            var cuilsEarrings = incomes
                                .Select(i => i.paciente!.cuilPatient)
                                .ToList();
            Assert.Equal(cuilsListExpected, cuilsEarrings!);
        }

    }
}
