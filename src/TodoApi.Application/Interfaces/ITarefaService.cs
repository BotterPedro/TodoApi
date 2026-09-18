using TodoApi.Application.DTOs;

namespace TodoApi.Application.Interfaces;

public interface ITarefaService
{
    Task<IEnumerable<TarefaDto>> ListarAsync(Guid usuarioId);
    Task<TarefaDto?> ObterPorIdAsync(Guid id, Guid usuarioId);
    Task<TarefaDto> CriarAsync(CriarTarefaDto dto, Guid usuarioId);
    Task<TarefaDto?> AtualizarAsync(Guid id, AtualizarTarefaDto dto, Guid usuarioId);
    Task<bool> DeletarAsync(Guid id, Guid usuarioId);
    Task<TarefaDto?> ConcluirAsync(Guid id, Guid usuarioId);
    Task<TarefaDto?> ReabrirAsync(Guid id, Guid usuarioId);
}