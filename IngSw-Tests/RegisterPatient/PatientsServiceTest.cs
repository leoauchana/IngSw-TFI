using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Services;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Interfaces;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;
using NSubstitute;

namespace IngSw_Tests.RegisterPatient;

public class PatientsServiceTest
{
    private readonly IPatientRepository _patientsRepository;
    private readonly PatientsService _patientsService;
    private readonly ISocialWorkServiceApi _socialWorkServiceApi;
    public PatientsServiceTest()
    {
        _patientsRepository = Substitute.For<IPatientRepository>();
        _socialWorkServiceApi = Substitute.For<ISocialWorkServiceApi>();
        _patientsService = new PatientsService(_patientsRepository, _socialWorkServiceApi);
    }
    [Fact]
    public async Task AddPatient_WhenTheHealthcareSystemExistsWithSocialWorkExisting_ShouldCreateThePatient()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );
        _socialWorkServiceApi.ExistingSocialWork("OSPE")
            .Returns(Task.FromResult(true));
        _socialWorkServiceApi.IsAffiliated("4798540152")
        .Returns(Task.FromResult(true));
        // Act
        var result = await _patientsService.AddPatient(patientDto);

        // Assert
        await _patientsRepository.Received(1).AddPatient(Arg.Any<Patient>());
        await _socialWorkServiceApi.Received(1).ExistingSocialWork(Arg.Any<string>());
        await _socialWorkServiceApi.Received(1).IsAffiliated(Arg.Any<string>());
        Assert.NotNull(result);
        Assert.Equal(patientDto.cuilPatient, result.cuilPatient);
        Assert.Equal(patientDto.namePatient, result.namePatient);
        Assert.Equal(patientDto.lastNamePatient, result.lastNamePatient);
    }
    [Fact]
    public async Task AddPatient_WhenTheHealthcareSystemExistsWithoutSocialWork_ShouldCreateThePatient()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: null,
            affiliateNumber: null
        );

        // Act
        var result = await _patientsService.AddPatient(patientDto);

        // Assert
        await _patientsRepository.Received(1).AddPatient(Arg.Any<Patient>());
        Assert.NotNull(result);
        Assert.Equal(patientDto.cuilPatient, result.cuilPatient);
        Assert.Equal(patientDto.namePatient, result.namePatient);
        Assert.Equal(patientDto.lastNamePatient, result.lastNamePatient);
    }
    [Fact]
    public async Task AddPatient_WhenTheHealthcareSystemExistsWithSocialWorkInexisting_ShouldNotCreateThePatient()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "Subsidio",
            affiliateNumber: "4798540152"
        );
        _socialWorkServiceApi.ExistingSocialWork("Subsidio")
            .Returns(Task.FromResult(false));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessConflicException>(
            () => _patientsService.AddPatient(patientDto)
        );
        Assert.Equal("La obra social no existe, por lo tanto no se puede registrar al paciente.", exception.Message);
        await _socialWorkServiceApi.Received(1).ExistingSocialWork(Arg.Any<string>());
        await _socialWorkServiceApi.Received(0).IsAffiliated(Arg.Any<string>());
        await _patientsRepository.Received(0).AddPatient(Arg.Any<Patient>());
    }
    [Fact]
    public async Task AddPatient_WhenTheHealthcareSystemExistsWithSocialWorkExistingButWitouthAffiliation_ShouldNotCreateThePatient()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "Subsidio",
            affiliateNumber: "4798540152"
        );
        _socialWorkServiceApi.ExistingSocialWork("Subsidio")
            .Returns(Task.FromResult(true));
        _socialWorkServiceApi.IsAffiliated("4798540152")
            .Returns(Task.FromResult(false));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessConflicException>(
            () => _patientsService.AddPatient(patientDto)
        );
        Assert.Equal("El paciente no es afiliado de la obra social, por lo tanto no se puede registrar al paciente.", exception.Message);
        await _socialWorkServiceApi.Received(1).ExistingSocialWork(Arg.Any<string>());
        await _socialWorkServiceApi.Received(1).IsAffiliated(Arg.Any<string>());
        await _patientsRepository.Received(0).AddPatient(Arg.Any<Patient>());
    }
    [Fact]
    public async Task AddPatient_WhenSocialWorkOrAffiliateNumberIsMissing_ShouldThrowArgumentException()
    {
        // Arrange

        // Caso 1: Falta el número de afiliado, pero se indica la obra social
        var patientDtoMissingAffiliate = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "Subsidio",
            affiliateNumber: null
        );
        // Caso 2: Falta la obra social, pero se indica el número de afiliado
        var patientDtoMissingSocialWork = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: null,
            affiliateNumber: "4798540152"
        );

        // Act & Assert

        var exception1 = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDtoMissingAffiliate)
        );
        Assert.Equal("Si se ingresa la obra social, también debe ingresarse el número de afiliado (y viceversa).", exception1.Message);

        var exception2 = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDtoMissingSocialWork)
        );
        Assert.Equal("Si se ingresa la obra social, también debe ingresarse el número de afiliado (y viceversa).", exception2.Message);

        await _socialWorkServiceApi.Received(0).ExistingSocialWork(Arg.Any<string>());
        await _socialWorkServiceApi.Received(0).IsAffiliated(Arg.Any<string>());
        await _patientsRepository.Received(0).AddPatient(Arg.Any<Patient>());
    }
    [Fact]
    public async Task AddPatient_WhenCuilIsNotValid_ThenShouldThrowExceptionAndNotCreateThePatient()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "45750673",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );
        _socialWorkServiceApi.ExistingSocialWork("OSPE")
        .Returns(Task.FromResult(true));
        _socialWorkServiceApi.IsAffiliated("4798540152")
        .Returns(Task.FromResult(true));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
        );

        Assert.Equal("CUIL con formato inválido.", exception.Message);

        await _patientsRepository.DidNotReceive().AddPatient(Arg.Any<Patient>());
    }
    [Fact]
    public async Task AddPatient_WhenCuilIsNull_ThenShouldThrowExceptionAndNotCreateThePatient()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: null!,
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );
        _socialWorkServiceApi.ExistingSocialWork("OSPE")
        .Returns(Task.FromResult(true));
        _socialWorkServiceApi.IsAffiliated("4798540152")
        .Returns(Task.FromResult(true));
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
        );

        Assert.Equal("CUIL no puede ser vacío.", exception.Message);

        await _patientsRepository.DidNotReceive().AddPatient(Arg.Any<Patient>());
    }
    [Fact]
    public async Task AddPatient_WhenCuilIsWhiteSpace_ThenShouldThrowExceptionAndNotCreateThePatient()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "   ",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );
        _socialWorkServiceApi.ExistingSocialWork("OSPE")
        .Returns(Task.FromResult(true));
        _socialWorkServiceApi.IsAffiliated("4798540152")
        .Returns(Task.FromResult(true));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
        );
        Assert.Equal("CUIL no puede ser vacío.", exception.Message);
        await _patientsRepository.DidNotReceive().AddPatient(Arg.Any<Patient>());
    }
    [Fact]
    public async Task AddPatient_WhenPatientAlreadyExists_ThenShouldThrowExceptionAndNotCreateThePatient()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        var existingPatient = new Patient
        {
            Cuil = Cuil.Create("20-45750673-8"),
            Name = "Lautaro",
            LastName = "Lopez",
            Email = "lautalopez@gmail.com"
        };

        _patientsRepository.GetByCuil(patientDto.cuilPatient)!.Returns(Task.FromResult(new List<Patient> { existingPatient }));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessConflicException>(
            () => _patientsService.AddPatient(patientDto)
        );
        Assert.Equal($"El paciente de cuil {patientDto.cuilPatient} ya se encuentra registrado", exception.Message);
        await _patientsRepository.DidNotReceive().AddPatient(Arg.Any<Patient>());
    }
    [Fact]
    public async Task GetByCuil_WhenPatientsExistWithMatchingCuil_ShouldReturnAllMatchingPatients()
    {
        // Arrange
        string cuilReceived = "20-45758";

        var patientsFromRepository = new List<Patient>
        {
            new Patient
            {
                Cuil = Cuil.Create("20-45758331-8"),
                Name = "Lautaro",
                LastName = "Lopez",
                Email = "lautalopez@gmail.com",
                Domicilie = new Domicilie
                {
                    Number = 324,
                    Street = "Jujuy",
                    Locality = "San Miguel"
                }
            },
            new Patient
            {
                Cuil = Cuil.Create("20-43758621-4"),
                Name = "Lucia",
                LastName = "Perez",
                Email = "luciaperez@gmail.com",
                Domicilie = new Domicilie
                {
                    Number = 356,
                    Street = "Avenue Nine Of July",
                    Locality = "CABA"
                },
            }
        };

        _patientsRepository.GetByCuil(cuilReceived)
               .Returns(Task.FromResult<List<Patient>?>(patientsFromRepository));

        // Act
        var patientsFound = await _patientsService.GetByCuil(cuilReceived);

        // Assert
        await _patientsRepository.Received(1).GetByCuil(cuilReceived);

        Assert.NotNull(patientsFound);
        Assert.Equal(2, patientsFound.Count);
        // Comprobamos propiedades de los DTOs resultantes
        Assert.Equal(patientsFromRepository[0].Cuil!.Value, patientsFound[0].cuilPatient);
        Assert.Equal(patientsFromRepository[1].Cuil!.Value, patientsFound[1].cuilPatient);
        Assert.Equal(patientsFromRepository[0].Name, patientsFound[0].namePatient);
        Assert.Equal(patientsFromRepository[1].Name, patientsFound[1].namePatient);
    }
    [Fact]
    public async Task GetByCuil_WhenExistsAUniqueMatchingCuil_ShouldReturnThePatient()
    {
        // Arrange
        string cuilReceived = "20-457583";

        var patientsFromRepository = new List<Patient>
        {
            new Patient
            {
                Cuil = Cuil.Create("20-45758331-8"),
                Name = "Lautaro",
                LastName = "Lopez",
                Email = "lautalopez@gmail.com",
                Domicilie = new Domicilie
                {
                    Number = 324,
                    Street = "Jujuy",
                    Locality = "San Miguel"
                }
            }
        };

        _patientsRepository.GetByCuil(cuilReceived)
               .Returns(Task.FromResult<List<Patient>?>(patientsFromRepository));

        // Act
        var patientsFound = await _patientsService.GetByCuil(cuilReceived);

        // Assert
        await _patientsRepository.Received(1).GetByCuil(cuilReceived);

        Assert.NotNull(patientsFound);
        Assert.Single(patientsFound);
        // Comprobamos propiedades de los DTOs resultantes
        Assert.Equal(patientsFromRepository[0].Cuil!.Value, patientsFound[0].cuilPatient);
        Assert.Equal(patientsFromRepository[0].Name, patientsFound[0].namePatient);
    }
    [Fact]
    public async Task GetByCuil_WhenNoPatientsFound_ShouldThrowNullException()
    {
        // Arrange
        string cuilReceived = "20-45758";

        _patientsRepository.GetByCuil(cuilReceived)!.Returns(Task.FromResult<List<Patient>?>(null));

        //Act & Arrange
        var exception = await Assert.ThrowsAsync<NullException>(
            () => _patientsService.GetByCuil(cuilReceived)
            );
        await _patientsRepository.Received(1).GetByCuil(cuilReceived);
        Assert.Equal($"No hay pacientes que coincidan con el cuil {cuilReceived} registrados.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenLastNameIsOmitted_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Apellido' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenLastNameIsWhiteSpace_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "  ",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Apellido' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenLastNameIsNull_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: null!,
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Apellido' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenNameIsOmitted_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Nombre' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenNameIsWhiteSpace_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "   ",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Nombre' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenNameIsNull_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: null!,
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Nombre' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenStreetIsOmitted_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Calle' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenStreetIsWhiteSpace_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "   ",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Calle' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenStreetIsNull_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: null!,
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Calle' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenNumberIsOmitted_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 0,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Número' no puede ser omitido o exceder el límite permitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenNumberIsNegative_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: -356,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Número' no puede ser omitido o exceder el límite permitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenNumberIsTooHigh_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 10000,
            localityDomicilie: "CABA",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Número' no puede ser omitido o exceder el límite permitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenLocalityIsOmitted_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Localidad' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenLocalityIsWhiteSpace_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "   ",
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Localidad' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenLocalityIsNull_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: null!,
            idSocialWork: "OSPE",
            affiliateNumber: "4798540152"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Localidad' no puede ser omitido.", exception.Message);
    }
}
