using TodoApi.Application.DTOs;
using TodoApi.Infrastructure.Services;
using TodoApi.UnitTests.Application.Services;

namespace TodoApi.UnitTests.Application.Services;

public class TarefaServiceTests
{
    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveRetornarTarefaCriada()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var dto = new CriarTarefaDto
        {
            Titulo = "Estudar C#",
            Descricao = "Terminar a API"
        };

        var resultado = await service.CriarAsync(dto);

        Assert.NotEqual(Guid.Empty, resultado.Id);
        Assert.Equal("Estudar C#", resultado.Titulo);
        Assert.Equal("Terminar a API", resultado.Descricao);
        Assert.False(resultado.Concluida);
        Assert.Null(resultado.ConcluidaEm);

        var tarefaNoBanco = await context.Tarefas.FindAsync(resultado.Id);
        Assert.NotNull(tarefaNoBanco);
    }

    [Fact]
    public async Task CriarAsync_ComTituloVazio_DeveLancarExcecao()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var dto = new CriarTarefaDto { Titulo = "" };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(dto));
    }

    [Fact]
    public async Task ListarAsync_QuandoNaoHaTarefas_DeveRetornarListaVazia()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);

        var resultado = await service.ListarAsync();

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarTodasAsTarefas()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);

        await service.CriarAsync(new CriarTarefaDto { Titulo = "Tarefa 1" });
        await service.CriarAsync(new CriarTarefaDto { Titulo = "Tarefa 2" });
        await service.CriarAsync(new CriarTarefaDto { Titulo = "Tarefa 3" });

        var resultado = await service.ListarAsync();

        Assert.Equal(3, resultado.Count());
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoExiste_DeveRetornarTarefa()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Estudar C#" });

        var resultado = await service.ObterPorIdAsync(criada.Id);

        Assert.NotNull(resultado);
        Assert.Equal(criada.Id, resultado!.Id);
        Assert.Equal("Estudar C#", resultado.Titulo);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);

        var resultado = await service.ObterPorIdAsync(Guid.NewGuid());

        Assert.Null(resultado);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoExiste_DeveAtualizarDados()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Antigo" });

        var resultado = await service.AtualizarAsync(criada.Id, new AtualizarTarefaDto
        {
            Titulo = "Novo",
            Descricao = "Descrição nova"
        });

        Assert.NotNull(resultado);
        Assert.Equal("Novo", resultado!.Titulo);
        Assert.Equal("Descrição nova", resultado.Descricao);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);

        var resultado = await service.AtualizarAsync(Guid.NewGuid(), new AtualizarTarefaDto
        {
            Titulo = "Qualquer"
        });

        Assert.Null(resultado);
    }

    [Fact]
    public async Task DeletarAsync_QuandoExiste_DeveRetornarTrueERemover()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Para deletar" });

        var resultado = await service.DeletarAsync(criada.Id);

        Assert.True(resultado);
        var noBanco = await context.Tarefas.FindAsync(criada.Id);
        Assert.Null(noBanco);
    }

    [Fact]
    public async Task DeletarAsync_QuandoNaoExiste_DeveRetornarFalse()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);

        var resultado = await service.DeletarAsync(Guid.NewGuid());

        Assert.False(resultado);
    }

    [Fact]
    public async Task ConcluirAsync_QuandoExiste_DeveMarcarComoConcluida()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Para concluir" });

        var resultado = await service.ConcluirAsync(criada.Id);

        Assert.NotNull(resultado);
        Assert.True(resultado!.Concluida);
        Assert.NotNull(resultado.ConcluidaEm);
    }

    [Fact]
    public async Task ReabrirAsync_QuandoExisteConcluida_DeveReabrir()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Para reabrir" });
        await service.ConcluirAsync(criada.Id);

        var resultado = await service.ReabrirAsync(criada.Id);

        Assert.NotNull(resultado);
        Assert.False(resultado!.Concluida);
        Assert.Null(resultado.ConcluidaEm);
    }
}