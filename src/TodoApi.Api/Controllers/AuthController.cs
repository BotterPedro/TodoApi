using Microsoft.AspNetCore.Mvc;
using TodoApi.Application.DTOs;
using TodoApi.Application.Interfaces;

namespace TodoApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<TokenDto>> Registrar([FromBody] RegistrarUsuarioDto dto)
    {
        var resultado = await _service.RegistrarAsync(dto);
        return Ok(resultado);
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto dto)
    {
        var resultado = await _service.LoginAsync(dto);

        if (resultado is null)
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

        return Ok(resultado);
    }
}