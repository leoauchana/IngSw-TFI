using IngSw_Tfi.Application.DTOs;

namespace IngSw_Tfi.Application.Interfaces;

public interface IAuthService
{
    Task<UserDto.Response?> Login(UserDto.RequestUser userDto);
    Task<UserDto.Response?> Register(UserDto.RequestRegister userDto);
}
