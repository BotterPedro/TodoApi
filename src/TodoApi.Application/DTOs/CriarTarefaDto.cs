namespace TodoApi.Application.DTOs;

public class CriarTarefaDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}