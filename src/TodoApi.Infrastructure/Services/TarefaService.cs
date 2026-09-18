using Microsoft.EntityFrameworkCore;
using TodoApi.Application.DTOs;
using TodoApi.Application.Interfaces;
using TodoApi.Domain.Entities;
using TodoApi.Infrastructure.Data;

namespace TodoApi.Infrastructure.Services;

public class TarefaService : ITarefaService
{
    private readonly AppDbContext _context;

    public TarefaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TarefaDto>> ListarAsync(Guid usuarioId)
    {
        var tarefas = await _context.Tarefas
            .AsNoTracking()
            .Where(t => t.UsuarioId == usuarioId)
            .OrderByDescending(t => t.CriadaEm)
            .ToListAsync();

        return tarefas.Select(ToDto);
    }

    public async Task<TarefaDto?> ObterPorIdAsync(Guid id, Guid usuarioId)
    {
        var tarefa = await _context.Tarefas
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

        return tarefa is null ? null : ToDto(tarefa);
    }

    public async Task<TarefaDto> CriarAsync(CriarTarefaDto dto, Guid usuarioId)
    {
        var tarefa = new Tarefa(dto.Titulo, dto.Descricao, usuarioId);
        _context.Tarefas.Add(tarefa);
        await _context.SaveChangesAsync();
        return ToDto(tarefa);
    }

    public async Task<TarefaDto?> AtualizarAsync(Guid id, AtualizarTarefaDto dto, Guid usuarioId)
    {
        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);
        if (tarefa is null) return null;

        tarefa.Atualizar(dto.Titulo, dto.Descricao);
        await _context.SaveChangesAsync();
        return ToDto(tarefa);
    }

    public async Task<bool> DeletarAsync(Guid id, Guid usuarioId)
    {
        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);
        if (tarefa is null) return false;

        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TarefaDto?> ConcluirAsync(Guid id, Guid usuarioId)
    {
        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);
        if (tarefa is null) return null;

        tarefa.Concluir();
        await _context.SaveChangesAsync();
        return ToDto(tarefa);
    }

    public async Task<TarefaDto?> ReabrirAsync(Guid id, Guid usuarioId)
    {
        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);
        if (tarefa is null) return null;

        tarefa.Reabrir();
        await _context.SaveChangesAsync();
        return ToDto(tarefa);
    }

    private static TarefaDto ToDto(Tarefa tarefa) => new()
    {
        Id = tarefa.Id,
        Titulo = tarefa.Titulo,
        Descricao = tarefa.Descricao,
        Concluida = tarefa.Concluida,
        CriadaEm = tarefa.CriadaEm,
        ConcluidaEm = tarefa.ConcluidaEm
    };
}