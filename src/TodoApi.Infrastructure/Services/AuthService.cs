using Microsoft.EntityFrameworkCore;
using TodoApi.Application.DTOs;
using TodoApi.Application.Interfaces;
using TodoApi.Domain.Entities;
using TodoApi.Infrastructure.Data;

namespace TodoApi.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthService(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<TokenDto> RegistrarAsync(RegistrarUsuarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("E-mail é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Senha) || dto.Senha.Length < 6)
            throw new ArgumentException("Senha deve ter pelo menos 6 caracteres.");

        var emailNormalizado = dto.Email.Trim().ToLowerInvariant();

        var usuarioExistente = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == emailNormalizado);

        if (usuarioExistente is not null)
            throw new ArgumentException("Já existe um usuário com esse e-mail.");

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        var usuario = new Usuario(dto.Nome, dto.Email, senhaHash);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var (token, expiraEm) = _jwtService.GerarToken(usuario);

        return new TokenDto
        {
            Token = token,
            Nome = usuario.Nome,
            Email = usuario.Email,
            ExpiraEm = expiraEm
        };
    }

    public async Task<TokenDto?> LoginAsync(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
            return null;

        var emailNormalizado = dto.Email.Trim().ToLowerInvariant();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == emailNormalizado);

        if (usuario is null)
            return null;

        var senhaValida = BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash);
        if (!senhaValida)
            return null;

        var (token, expiraEm) = _jwtService.GerarToken(usuario);

        return new TokenDto
        {
            Token = token,
            Nome = usuario.Nome,
            Email = usuario.Email,
            ExpiraEm = expiraEm
        };
    }
}