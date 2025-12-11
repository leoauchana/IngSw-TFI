using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Application.Services;
using IngSw_Tfi.Data.DAOs;
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
    private readonly ISocialWorkService _socialWorkServiceAffiliate;
    private readonly ISocialWorkRepository _socialWorkRepository;
    public PatientsServiceTest()
    {
        _patientsRepository = Substitute.For<IPatientRepository>();
        _socialWorkServiceApi = Substitute.For<ISocialWorkServiceApi>();
        _socialWorkServiceAffiliate = Substitute.For<ISocialWorkService>();
        _socialWorkRepository = Substitute.For<ISocialWorkRepository>();
        _patientsService = new PatientsService(_patientsRepository, _socialWorkRepository);
    }
    [Fact]
    public async Task AddPatient_WhenTheHealthcareExists_ShouldCreateThePatient()
    {
        // Arrange
        var HealthCare = new SocialWork
        {
            Name = "OSPE",
        };
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "3814050905",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "CA50EF74-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "123456"
        );
        _socialWorkRepository.ExistingSocialWork(patientDto.idSocialWork!)!
            .Returns(Task.FromResult(HealthCare));
        _socialWorkRepository.ValidateInsuranceAndMember(Arg.Any<string>(), Arg.Any<string>()).
            Returns(true);

        // Act
        var result = await _patientsService.AddPatient(patientDto);

        // Assert
        await _patientsRepository.Received(1).AddPatient(Arg.Any<Patient>());
        await _socialWorkRepository.Received(1).ExistingSocialWork(Arg.Any<string>());
        _socialWorkRepository.Received(1).ValidateInsuranceAndMember(Arg.Any<string>(), Arg.Any<string>());
        Assert.NotNull(result);
        Assert.Equal(patientDto.cuilPatient, result.cuilPatient);
        Assert.Equal(patientDto.namePatient, result.namePatient);
        Assert.Equal(patientDto.lastNamePatient, result.lastNamePatient);
    }
    [Fact]
    public async Task AddPatient_WhenTheHealthcareIsNull_ShouldCreateThePatient()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "3814050905",
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
    public async Task AddPatient_WhenTheHealthcareDoesNotExist_ShouldNotCreateThePatient()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "3814050905",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "KO55BD63-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "7f0e47c2-59c4-4e2c-afdc-bb1631a12045"
        );
        _socialWorkRepository.ExistingSocialWork(patientDto.idSocialWork!)!
            .Returns(Task.FromResult<SocialWork?>(null));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessConflicException>(
            () => _patientsService.AddPatient(patientDto)
        );
        Assert.Equal("La obra social no existe, por lo tanto no se puede registrar al paciente.", exception.Message);
        await _socialWorkRepository.Received(1).ExistingSocialWork(Arg.Any<string>());
        await _patientsRepository.Received(0).AddPatient(Arg.Any<Patient>());
    }
    [Fact]
    public async Task AddPatient_WhenPatientIsNotAffiliated_ShouldThrowArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "3814050905",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "CA50EF74-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "123456"
        );
        var socialWorkFound = new SocialWork();

        _socialWorkRepository.ExistingSocialWork(patientDto.idSocialWork!)!
            .Returns(Task.FromResult<SocialWork?>(socialWorkFound));
        _socialWorkRepository.ValidateInsuranceAndMember(Arg.Any<string>(), Arg.Any<string>()).
            Returns(false);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
        );

        //Assert
        await _socialWorkRepository.Received(1).ExistingSocialWork(Arg.Any<string>());
        _socialWorkRepository.Received(1).ValidateInsuranceAndMember(Arg.Any<string>(), Arg.Any<string>());
        Assert.Equal("No se pudo registrar el paciente dado que no esta afiliado a la obra social.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenCuilIsNotValid_ThenShouldThrowExceptionAndNotCreateThePatient()
    {
        // Arrange
        var HealthCare = new SocialWork
        {
            Name = "OSPE",
        };

        var patientDto = new PatientDto.Request(
            cuilPatient: "45750673",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "3814050905",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "CA50EF74-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "1234656"
        );
        _socialWorkRepository.ExistingSocialWork(patientDto.idSocialWork!)!
        .Returns(Task.FromResult(HealthCare));
        _socialWorkRepository.ValidateInsuranceAndMember(Arg.Any<string>(), Arg.Any<string>()).
            Returns(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
        );

        Assert.Equal("CUIL con formato inválido.", exception.Message);
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
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "3814050905",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "CA50EF74-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "7f0e47c2-59c4-4e2c-afdc-bb1631a12045"
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
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "3814050905",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "CA50EF74-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "7f0e47c2-59c4-4e2c-afdc-bb1631a12045"
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
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "3814050905",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "CA50EF74-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "7f0e47c2-59c4-4e2c-afdc-bb1631a12045"
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
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "3814050905",
            streetDomicilie: "",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "CA50EF74-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "7f0e47c2-59c4-4e2c-afdc-bb1631a12045"
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
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "3814050905",
            numberDomicilie: 0,
            localityDomicilie: "CABA",
            idSocialWork: "CA50EF74-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "7f0e47c2-59c4-4e2c-afdc-bb1631a12045"
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
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "3814050905",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "",
            idSocialWork: "CA50EF74-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "7f0e47c2-59c4-4e2c-afdc-bb1631a12045"
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
    public async Task AddPatient_WhenBirthDateIsNotValid_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            birthDate: DateTime.MinValue,
            phone: "3814050905",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "CA50EF74-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "7f0e47c2-59c4-4e2c-afdc-bb1631a12045"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Fecha de nacimiento' es inválido o está fuera del rango permitido.", exception.Message);
    }
    [Fact]
    public async Task AddPatient_WhenPhoneIsWhiteSpace_ShouldArgumentException()
    {
        // Arrange
        var patientDto = new PatientDto.Request(
            cuilPatient: "20-45750673-8",
            namePatient: "Lautaro",
            lastNamePatient: "Lopez",
            email: "lautalopez@gmail.com",
            birthDate: new DateTime(2001, 09, 17, 13, 30, 0),
            phone: "   ",
            streetDomicilie: "Avenue Nine Of July",
            numberDomicilie: 356,
            localityDomicilie: "CABA",
            idSocialWork: "CA50EF74-40AC-471E-8397-A3B214FD5B8F",
            affiliateNumber: "7f0e47c2-59c4-4e2c-afdc-bb1631a12045"
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _patientsService.AddPatient(patientDto)
            );

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Telefono' no puede ser omitido.", exception.Message);
    }
}