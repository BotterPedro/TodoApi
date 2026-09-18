using TodoApi.Application.DTOs;

namespace TodoApi.Application.Interfaces;

public interface IAuthService
{
    Task<TokenDto> RegistrarAsync(RegistrarUsuarioDto dto);
    Task<TokenDto?> LoginAsync(LoginDto dto);
}