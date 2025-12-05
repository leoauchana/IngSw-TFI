using IngSw_Tfi.Application.DTOs;
using IngSw_Tfi.Application.Exceptions;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Application.Services;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Exception;
using IngSw_Tfi.Domain.Interfaces;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace IngSw_Tests.Login;

public class AuthServiceTest
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly AuthService _authService;
    public AuthServiceTest()
    {
        _employeeRepository = Substitute.For<IEmployeeRepository>();

        var settings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "ReplaceWithAStrongSecretKeyChangeInProduction" },
            { "Jwt:Issuer", "IngSwTfi.Api" },
            { "Jwt:Audience", "IngSwTfi.Client" },
            { "Jwt:ExpiresMinutes", "60" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();

        _authService = new AuthService(_employeeRepository, configuration);
    }
    [Fact]
    public async Task Login_WhenYouEnterTheCorrectEmailAndPassword_ThenYouLogIn_ShouldGetTheEmployee()
    {
        // Arrange

        var userDto = new UserDto.Request("ramirobrito@gmail.com", "bocateamo");
        var employeeFound = new Employee
        {
            Name = "Ramiro",
            LastName = "Brito",
            Cuil = Cuil.Create("20-42365986-7"),
            PhoneNumber = "381754963",
            Email = "ramirobrito@gmail.com",
            Registration = "LO78Q",
            User = new User
            {
                Email = "ramirobrito@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("bocateamo"),
            }
        };
        _employeeRepository.GetByEmail("ramirobrito@gmail.com")!
            .Returns(Task.FromResult(employeeFound));

        // Act

        var result = await _authService.Login(userDto);

        // Assert

        await _employeeRepository.Received(1).GetByEmail(Arg.Any<string>());
        Assert.NotNull(result);
        Assert.Equal(employeeFound.Email, result.email);
        Assert.Equal(employeeFound.Name, result.name);
        Assert.Equal(employeeFound.LastName, result.lastName);
    }
    [Fact]
    public async Task Login_WhenEnteredInvalidEmail_ShouldThrowEntityNotFoundException()
    {
        // Arrange

        var userDto = new UserDto.Request("ramirobrito@gmail.com", "bocateamo");
        
        _employeeRepository.GetByEmail(userDto.email!)!
            .Returns(Task.FromResult<Employee?>(null));

        // Act & Assert

        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _authService.Login(userDto)
        );
        Assert.Equal("Usuario o contraseña incorrecto.", exception.Message);
        await _employeeRepository.Received(1).GetByEmail(Arg.Any<string>());

    }
    [Fact]
    public async Task Login_WhenEnteredInvalidPassword_ShouldThrowEntityNotFoundException()
    {
        // Arrange

        var userDto = new UserDto.Request("ramirobrito@gmail.com", "riverteamo");
        var employeeFound = new Employee
        {
            Name = "Ramiro",
            LastName = "Brito",
            Cuil = Cuil.Create("20-42365986-7"),
            PhoneNumber = "381754963",
            Email = "ramirobrito@gmail.com",
            Registration = "LO78Q",
            User = new User
            {
                Email = "ramirobrito@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("bocateamo"),
            }
        };
        _employeeRepository.GetByEmail("ramirobrito@gmail.com")!
            .Returns(Task.FromResult(employeeFound));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _authService.Login(userDto)
        );
        Assert.Equal("Usuario o contraseña incorrecto.", exception.Message);
        await _employeeRepository.Received(1).GetByEmail(Arg.Any<string>());
    }
    [Fact]
    public async Task Login_WhenEnteredEmptyEmail_ShouldThrowArgumentException()
    {
        // Arrange

        var userDto = new UserDto.Request("", "riverteamo");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Login(userDto)
        );
        Assert.Equal("Debe ingresar correctamente los datos", exception.Message);
        await _employeeRepository.Received(0).GetByEmail(Arg.Any<string>());
    }
    [Fact]
    public async Task Login_WhenEnteredEmptyPassword_ShouldThrowArgumentException()
    {
        // Arrange

        var userDto = new UserDto.Request("ramirobrito@gmail.com", "");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Login(userDto)
        );

        Assert.Equal("Debe ingresar correctamente los datos", exception.Message);
        await _employeeRepository.Received(0).GetByEmail(Arg.Any<string>());
    }
    [Fact]
    public async Task Register_WhenTheFormIsCompleteCorrectly_ThenShouldCreateUserAndEmployee()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "Nurse"
            );

        Employee? employeeCaptured = null;

        _employeeRepository
            .Register(Arg.Do<Employee>(x => employeeCaptured = x))!
            .Returns(Task.FromResult<Employee>(new Nurse
            {
                Id = Guid.NewGuid(),
                Name = "Ramiro",
                LastName = "Brito",
                Email = "ramirobrito@gmail.com",
                PhoneNumber = "381754963",
                Cuil = Cuil.Create("20-45750673-8"),
                Registration = "LO78Q",
                User = new User
                {
                    Email = "ramirobrito@gmail.com",
                    Password = "HASHED-PASS"
                }
            }));

        //Act
        var result = await _authService.Register(userDto);

        //Assert
        await _employeeRepository.Received(1).Register(Arg.Any<Employee>());
        Assert.NotNull(result);
        Assert.Equal(userDto.email, result.email);
        Assert.Equal(userDto.cuil, result.cuil);
        Assert.Equal(userDto.licence, result.licence);
        Assert.Equal(userDto.typeEmployee, result.typeEmployee);
    }
    [Fact]
    public async Task Register_WhenPasswordsDontMatching_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "bocateamo1",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("Las contraseñas no coinciden.", exception.Message);
    }
    [Fact]
    public async Task Register_WhenEmailIsEmpty_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Email' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task Register_WhenEmailFormatIsNotValid_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("Email con formato inválido.", exception.Message);
    }
    //[Fact]
    //public async Task Register_WhenEmailIsNull_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: null!,
    //        password: "bocateamo",
    //        confirmPassword: "bocateamo",
    //        name: "Ramiro",
    //        lastName: "Brito",
    //        cuil: "20-45750673-8",
    //        licence: "LO78Q",
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Email' no puede ser omitido.", exception.Message);
    //}
    [Fact]
    public async Task Register_WhenPasswordIsEmpty_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Contraseña' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task Register_WhenPasswordDoNotHave8Chars_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "123",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("La contraseña debe poseer un minimo de 8 caracteres de largo.", exception.Message);
    }
    //[Fact]
    //public async Task Register_WhenPasswordIsNull_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: null!,
    //        confirmPassword: "bocateamo",
    //        name: "Ramiro",
    //        lastName: "Brito",
    //        cuil: "20-45750673-8",
    //        licence: "LO78Q",
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Contraseña' no puede ser omitido.", exception.Message);
    //}
    [Fact]
    public async Task Register_WhenConfirmPasswordIsEmpty_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Confirmacion' no puede ser omitido.", exception.Message);
    }
    //[Fact]
    //public async Task Register_WhenConfirmPasswordIsWhiteSpace_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: "   ",
    //        name: "Ramiro",
    //        lastName: "Brito",
    //        cuil: "20-45750673-8",
    //        licence: "LO78Q",
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Confirmacion' no puede ser omitido.", exception.Message);
    //}
    //[Fact]
    //public async Task Register_WhenConfirmPasswordIsNull_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: null!,
    //        name: "Ramiro",
    //        lastName: "Brito",
    //        cuil: "20-45750673-8",
    //        licence: "LO78Q",
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Confirmacion' no puede ser omitido.", exception.Message);
    //}
    [Fact]
    public async Task Register_WhenNameIsEmpty_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Nombre' no puede ser omitido.", exception.Message);
    }
    //[Fact]
    //public async Task Register_WhenNameIsWhiteSpace_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: "bocateamo",
    //        name: "   ",
    //        lastName: "Brito",
    //        cuil: "20-45750673-8",
    //        licence: "LO78Q",
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Nombre' no puede ser omitido.", exception.Message);
    //}
    //[Fact]
    //public async Task Register_WhenNameIsNull_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: "bocateamo",
    //        name: null!,
    //        lastName: "Brito",
    //        cuil: "20-45750673-8",
    //        licence: "LO78Q",
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Nombre' no puede ser omitido.", exception.Message);
    //}
    [Fact]
    public async Task Register_WhenLastNameIsEmpty_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Apellido' no puede ser omitido.", exception.Message);
    }
    //[Fact]
    //public async Task Register_WhenLastNameIsWhiteSpace_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: "bocateamo",
    //        name: "Ramiro",
    //        lastName: "   ",
    //        cuil: "20-45750673-8",
    //        licence: "LO78Q",
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Apellido' no puede ser omitido.", exception.Message);
    //}
    //[Fact]
    //public async Task Register_WhenLastNameIsNull_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: "bocateamo",
    //        name: "Ramiro",
    //        lastName: null!,
    //        cuil: "20-45750673-8",
    //        licence: "LO78Q",
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Apellido' no puede ser omitido.", exception.Message);
    //}
    [Fact]
    public async Task Register_WhenCuilIsNotValid_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "2048556782",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("CUIL con formato inválido.", exception.Message);
    }
    [Fact]
    public async Task Register_WhenCuilIsEmpty_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Cuil' no puede ser omitido.", exception.Message);
    }
    //[Fact]
    //public async Task Register_WhenCuilIsWhiteSpace_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: "bocateamo",
    //        name: "Ramiro",
    //        lastName: "Brito",
    //        cuil: "   ",
    //        licence: "LO78Q",
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Cuil' no puede ser omitido.", exception.Message);
    //}
    //[Fact]
    //public async Task Register_WhenCuilIsNull_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: "bocateamo",
    //        name: "Ramiro",
    //        lastName: "Brito",
    //        cuil: null!,
    //        licence: "LO78Q",
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Cuil' no puede ser omitido.", exception.Message);
    //}
    [Fact]
    public async Task Register_WhenLicenceIsEmpty_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "",
            phoneNumber: "381754963",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Licencia' no puede ser omitido.", exception.Message);
    }
    //[Fact]
    //public async Task Register_WhenLicenceIsWhiteSpace_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: "bocateamo",
    //        name: "Ramiro",
    //        lastName: "Brito",
    //        cuil: "20-45750673-8",
    //        licence: "   ",
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Licencia' no puede ser omitido.", exception.Message);
    //}
    //[Fact]
    //public async Task Register_WhenLicenceIsNull_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: "bocateamo",
    //        name: "Ramiro",
    //        lastName: "Brito",
    //        cuil: "20-45750673-8",
    //        licence: null!,
    //        phoneNumber: "381754963",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Licencia' no puede ser omitido.", exception.Message);
    //}
    [Fact]
    public async Task Register_WhenPhoneIsEmpty_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "",
            typeEmployee: "nurse"
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Numero de Telefono' no puede ser omitido.", exception.Message);
    }
    //[Fact]
    //public async Task Register_WhenPhoneIsWhiteSpace_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: "bocateamo",
    //        name: "Ramiro",
    //        lastName: "Brito",
    //        cuil: "20-45750673-8",
    //        licence: "LO78Q",
    //        phoneNumber: "  ",
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Numero de Telefono' no puede ser omitido.", exception.Message);
    //}
    //[Fact]
    //public async Task Register_WhenPhoneIsNull_ThenShouldArgumentException()
    //{
    //    // Arrange
    //    var userDto = new UserDto.RequestRegister(
    //        email: "ramirobrito@gmail.com",
    //        password: "bocateamo",
    //        confirmPassword: "bocateamo",
    //        name: "Ramiro",
    //        lastName: "Brito",
    //        cuil: "20-45750673-8",
    //        licence: "LO78Q",
    //        phoneNumber: null!,
    //        typeEmployee: "nurse"
    //        );

    //    //Act
    //    var exception = await Assert.ThrowsAsync<ArgumentException>(
    //        () => _authService.Register(userDto)
    //    );

    //    //Assert
    //    Assert.NotNull(exception);
    //    Assert.Equal("El campo 'Numero de Telefono' no puede ser omitido.", exception.Message);
    //}
    [Fact]
    public async Task Register_WhenEmployeeIsEmpty_ThenShouldArgumentException()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: ""
            );

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.Register(userDto)
        );

        //Assert
        Assert.NotNull(exception);
        Assert.Equal("El campo 'Tipo de Empleado' no puede ser omitido.", exception.Message);
    }
    [Fact]
    public async Task Register_WhenTypeEmployeeIsNurse_ThenShouldCreateNurse()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "Nurse"
            );
        Employee employeeCreated = new Nurse()
        {
            Name = "Ramiro",
            LastName = "Brito",
            Cuil = Cuil.Create("20-45750673-8"),
            PhoneNumber = "381754963",
            Email = "ramirobrito@gmail.com",
            Registration = "LO78Q",
            User = new User
            {
                Email = "ramirobrito@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("bocateamo"),
            }
        };
        _employeeRepository.Register(Arg.Any<Employee>())!
            .Returns(Task.FromResult(employeeCreated));

        //Act
        var result = await _authService.Register(userDto);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.typeEmployee, result.typeEmployee);
    }
    [Fact]
    public async Task Register_WhenTypeEmployeeIsDoctor_ThenShouldCreateDoctor()
    {
        // Arrange
        var userDto = new UserDto.RequestRegister(
            email: "ramirobrito@gmail.com",
            password: "bocateamo",
            confirmPassword: "bocateamo",
            name: "Ramiro",
            lastName: "Brito",
            cuil: "20-45750673-8",
            licence: "LO78Q",
            phoneNumber: "381754963",
            typeEmployee: "Doctor"
            );
        Employee employeeCreated = new Doctor()
        {
            Name = "Ramiro",
            LastName = "Brito",
            Cuil = Cuil.Create("20-45750673-8"),
            PhoneNumber = "381754963",
            Email = "ramirobrito@gmail.com",
            Registration = "LO78Q",
            User = new User
            {
                Email = "ramirobrito@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("bocateamo"),
            }
        };
        _employeeRepository.Register(Arg.Any<Employee>())!
            .Returns(Task.FromResult(employeeCreated));

        //Act
        var result = await _authService.Register(userDto);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.typeEmployee, result.typeEmployee);
    }
    [Fact]
    public async Task Login_WhenYouLoginSuccessfully_ThenTokenGenerator_ShouldCreateValidToken()
    {
        // Arrange
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("ReplaceWithAStrongSecretKeyChangeInProduction");
        var validationParams = new TokenValidationParameters
        {
            ValidIssuer = "IngSwTfi.Api",
            ValidAudience = "IngSwTfi.Client",
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };
        var userDto = new UserDto.Request("ramirobrito@gmail.com", "bocateamo");
        var employeeFound = new Employee
        {
            Name = "Ramiro",
            LastName = "Brito",
            Cuil = Cuil.Create("20-42365986-7"),
            PhoneNumber = "381754963",
            Email = "ramirobrito@gmail.com",
            Registration = "LO78Q",
            User = new User
            {
                Email = "ramirobrito@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("bocateamo"),
            }
        };
        _employeeRepository.GetByEmail(userDto.email!)!
            .Returns(Task.FromResult(employeeFound));

        // Act

        var result = await _authService.Login(userDto);

        // Assert

        await _employeeRepository.Received(1).GetByEmail(Arg.Any<string>());
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.token));
        Assert.Equal(3, result.token.Split('.').Length);
        Assert.Equal(result.id, handler.
            ValidateToken(result.token, validationParams, out _).
            FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal(result.name, handler.
            ValidateToken(result.token, validationParams, out _).
            FindFirst(ClaimTypes.Name)?.Value);
        Assert.Equal(result.typeEmployee, handler.
            ValidateToken(result.token, validationParams, out _).
            FindFirst(ClaimTypes.Role)?.Value);
    }
}