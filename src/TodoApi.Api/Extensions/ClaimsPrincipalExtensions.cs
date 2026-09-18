using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TodoApi.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid ObterUsuarioId(this ClaimsPrincipal user)
    {
        var valor = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(valor) || !Guid.TryParse(valor, out var id))
            throw new UnauthorizedAccessException("Token não contém identificação válida de usuário.");

        return id;
    }
}