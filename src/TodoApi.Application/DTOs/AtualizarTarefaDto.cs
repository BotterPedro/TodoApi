namespace TodoApi.Application.DTOs;

public class AtualizarTarefaDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}