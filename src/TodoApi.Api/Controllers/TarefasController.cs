using Microsoft.AspNetCore.Mvc;
using TodoApi.Application.DTOs;
using TodoApi.Application.Interfaces;

namespace TodoApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarefasController : ControllerBase
{
    private readonly ITarefaService _service;

    public TarefasController(ITarefaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TarefaDto>>> Listar()
    {
        var tarefas = await _service.ListarAsync();
        return Ok(tarefas);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TarefaDto>> ObterPorId(Guid id)
    {
        var tarefa = await _service.ObterPorIdAsync(id);
        if (tarefa is null) return NotFound();
        return Ok(tarefa);
    }

    [HttpPost]
    public async Task<ActionResult<TarefaDto>> Criar([FromBody] CriarTarefaDto dto)
    {
        var tarefa = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TarefaDto>> Atualizar(Guid id, [FromBody] AtualizarTarefaDto dto)
    {
        var tarefa = await _service.AtualizarAsync(id, dto);
        if (tarefa is null) return NotFound();
        return Ok(tarefa);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var deletou = await _service.DeletarAsync(id);
        if (!deletou) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id:guid}/concluir")]
    public async Task<ActionResult<TarefaDto>> Concluir(Guid id)
    {
        var tarefa = await _service.ConcluirAsync(id);
        if (tarefa is null) return NotFound();
        return Ok(tarefa);
    }

    [HttpPatch("{id:guid}/reabrir")]
    public async Task<ActionResult<TarefaDto>> Reabrir(Guid id)
    {
        var tarefa = await _service.ReabrirAsync(id);
        if (tarefa is null) return NotFound();
        return Ok(tarefa);
    }
}