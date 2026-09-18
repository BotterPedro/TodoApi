using TodoApi.Application.DTOs;
using TodoApi.Infrastructure.Services;
using TodoApi.UnitTests.Application.Services;

namespace TodoApi.UnitTests.Application.Services;

public class TarefaServiceTests
{
    private static readonly Guid UsuarioId = Guid.NewGuid();
    
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

        var resultado = await service.CriarAsync(dto, UsuarioId);

        Assert.NotEqual(Guid.Empty, resultado.Id);
        Assert.Equal("Estudar C#", resultado.Titulo);
        Assert.Equal("Terminar a API", resultado.Descricao);
        Assert.False(resultado.Concluida);
        Assert.Null(resultado.ConcluidaEm);

        var tarefaNoBanco = await context.Tarefas.FindAsync(resultado.Id);
        Assert.NotNull(tarefaNoBanco);
        Assert.Equal(UsuarioId, tarefaNoBanco!.UsuarioId);
    }

    [Fact]
    public async Task CriarAsync_ComTituloVazio_DeveLancarExcecao()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var dto = new CriarTarefaDto { Titulo = "" };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(dto, UsuarioId));
    }

    [Fact]
    public async Task ListarAsync_QuandoNaoHaTarefas_DeveRetornarListaVazia()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);

        var resultado = await service.ListarAsync(UsuarioId);

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarTodasAsTarefasDoUsuario()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);

        await service.CriarAsync(new CriarTarefaDto { Titulo = "Tarefa 1" }, UsuarioId);
        await service.CriarAsync(new CriarTarefaDto { Titulo = "Tarefa 2" }, UsuarioId);
        await service.CriarAsync(new CriarTarefaDto { Titulo = "Tarefa 3" }, UsuarioId);

        var resultado = await service.ListarAsync(UsuarioId);

        Assert.Equal(3, resultado.Count());
    }

    [Fact]
    public async Task ListarAsync_NaoDeveRetornarTarefasDeOutroUsuario()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var outroUsuarioId = Guid.NewGuid();

        await service.CriarAsync(new CriarTarefaDto { Titulo = "Minha" }, UsuarioId);
        await service.CriarAsync(new CriarTarefaDto { Titulo = "Do outro" }, outroUsuarioId);

        var resultado = await service.ListarAsync(UsuarioId);

        Assert.Single(resultado);
        Assert.Equal("Minha", resultado.First().Titulo);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoExisteEProprio_DeveRetornarTarefa()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Estudar C#" }, UsuarioId);

        var resultado = await service.ObterPorIdAsync(criada.Id, UsuarioId);

        Assert.NotNull(resultado);
        Assert.Equal(criada.Id, resultado!.Id);
        Assert.Equal("Estudar C#", resultado.Titulo);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);

        var resultado = await service.ObterPorIdAsync(Guid.NewGuid(), UsuarioId);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoTarefaDeOutroUsuario_DeveRetornarNull()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var outroUsuarioId = Guid.NewGuid();
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Do outro" }, outroUsuarioId);

        var resultado = await service.ObterPorIdAsync(criada.Id, UsuarioId);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoExisteEProprio_DeveAtualizarDados()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Antigo" }, UsuarioId);

        var resultado = await service.AtualizarAsync(criada.Id, new AtualizarTarefaDto
        {
            Titulo = "Novo",
            Descricao = "Descrição nova"
        }, UsuarioId);

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
        }, UsuarioId);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoTarefaDeOutroUsuario_DeveRetornarNull()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var outroUsuarioId = Guid.NewGuid();
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Do outro" }, outroUsuarioId);

        var resultado = await service.AtualizarAsync(criada.Id, new AtualizarTarefaDto
        {
            Titulo = "Tentativa de invasão"
        }, UsuarioId);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task DeletarAsync_QuandoExisteEProprio_DeveRetornarTrueERemover()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Para deletar" }, UsuarioId);

        var resultado = await service.DeletarAsync(criada.Id, UsuarioId);

        Assert.True(resultado);
        var noBanco = await context.Tarefas.FindAsync(criada.Id);
        Assert.Null(noBanco);
    }

    [Fact]
    public async Task DeletarAsync_QuandoNaoExiste_DeveRetornarFalse()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);

        var resultado = await service.DeletarAsync(Guid.NewGuid(), UsuarioId);

        Assert.False(resultado);
    }

    [Fact]
    public async Task DeletarAsync_QuandoTarefaDeOutroUsuario_DeveRetornarFalse()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var outroUsuarioId = Guid.NewGuid();
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Do outro" }, outroUsuarioId);

        var resultado = await service.DeletarAsync(criada.Id, UsuarioId);

        Assert.False(resultado);
        var aindaExiste = await context.Tarefas.FindAsync(criada.Id);
        Assert.NotNull(aindaExiste);
    }

    [Fact]
    public async Task ConcluirAsync_QuandoExisteEProprio_DeveMarcarComoConcluida()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Para concluir" }, UsuarioId);

        var resultado = await service.ConcluirAsync(criada.Id, UsuarioId);

        Assert.NotNull(resultado);
        Assert.True(resultado!.Concluida);
        Assert.NotNull(resultado.ConcluidaEm);
    }

    [Fact]
    public async Task ReabrirAsync_QuandoExisteEProprio_DeveReabrir()
    {
        using var context = DbContextFactoryHelper.Criar();
        var service = new TarefaService(context);
        var criada = await service.CriarAsync(new CriarTarefaDto { Titulo = "Para reabrir" }, UsuarioId);
        await service.ConcluirAsync(criada.Id, UsuarioId);

        var resultado = await service.ReabrirAsync(criada.Id, UsuarioId);

        Assert.NotNull(resultado);
        Assert.False(resultado!.Concluida);
        Assert.Null(resultado.ConcluidaEm);
    }
}