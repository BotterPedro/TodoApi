using System.Net;
using System.Net.Http.Json;
using TodoApi.Application.DTOs;

namespace TodoApi.IntegrationTests;

public class TarefasControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TarefasControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_ComDadosValidos_DeveRetornar201ComTarefa()
    {
        
        var dto = new CriarTarefaDto
        {
            Titulo = "Estudar C#",
            Descricao = "Terminar a API"
        };
       
        var response = await _client.PostAsJsonAsync("/api/tarefas", dto);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var tarefaCriada = await response.Content.ReadFromJsonAsync<TarefaDto>();
        Assert.NotNull(tarefaCriada);
        Assert.Equal("Estudar C#", tarefaCriada!.Titulo);
        Assert.False(tarefaCriada.Concluida);
    }

    [Fact]
    public async Task Post_ComTituloVazio_DeveRetornar400()
    {
        var dto = new CriarTarefaDto { Titulo = "" };

        var response = await _client.PostAsJsonAsync("/api/tarefas", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task Get_QuandoNaoHaTarefas_DeveRetornar200ComListaVazia()
    {
        var response = await _client.GetAsync("/api/tarefas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tarefas = await response.Content.ReadFromJsonAsync<List<TarefaDto>>();
        Assert.NotNull(tarefas);
        Assert.Empty(tarefas!);
    }

    [Fact]
    public async Task Get_AposCriarTarefa_DeveRetornarListaComUmaTarefa()
    {
        await _client.PostAsJsonAsync("/api/tarefas", new CriarTarefaDto { Titulo = "Tarefa 1" });
        await _client.PostAsJsonAsync("/api/tarefas", new CriarTarefaDto { Titulo = "Tarefa 2" });

        var response = await _client.GetAsync("/api/tarefas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tarefas = await response.Content.ReadFromJsonAsync<List<TarefaDto>>();
        Assert.NotNull(tarefas);
        Assert.Equal(2, tarefas!.Count);
    }

    [Fact]
    public async Task GetPorId_QuandoExiste_DeveRetornar200ComTarefa()
    {
        var criarResponse = await _client.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Estudar C#" });
        var criada = await criarResponse.Content.ReadFromJsonAsync<TarefaDto>();

        var response = await _client.GetAsync($"/api/tarefas/{criada!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tarefa = await response.Content.ReadFromJsonAsync<TarefaDto>();
        Assert.NotNull(tarefa);
        Assert.Equal(criada.Id, tarefa!.Id);
    }

    [Fact]
    public async Task GetPorId_QuandoNaoExiste_DeveRetornar404()
    {
        var response = await _client.GetAsync($"/api/tarefas/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_QuandoExiste_DeveRetornar200ComDadosAtualizados()
    {
        var criarResponse = await _client.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Antigo" });
        var criada = await criarResponse.Content.ReadFromJsonAsync<TarefaDto>();

        var atualizarDto = new AtualizarTarefaDto
        {
            Titulo = "Novo título",
            Descricao = "Nova descrição"
        };

        var response = await _client.PutAsJsonAsync($"/api/tarefas/{criada!.Id}", atualizarDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tarefa = await response.Content.ReadFromJsonAsync<TarefaDto>();
        Assert.NotNull(tarefa);
        Assert.Equal("Novo título", tarefa!.Titulo);
        Assert.Equal("Nova descrição", tarefa.Descricao);
    }

    [Fact]
    public async Task Put_QuandoNaoExiste_DeveRetornar404()
    {
        var response = await _client.PutAsJsonAsync($"/api/tarefas/{Guid.NewGuid()}",
            new AtualizarTarefaDto { Titulo = "Qualquer" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_QuandoExiste_DeveRetornar204ESumir()
    {
        var criarResponse = await _client.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Para deletar" });
        var criada = await criarResponse.Content.ReadFromJsonAsync<TarefaDto>();

        var response = await _client.DeleteAsync($"/api/tarefas/{criada!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var buscarResponse = await _client.GetAsync($"/api/tarefas/{criada.Id}");
        Assert.Equal(HttpStatusCode.NotFound, buscarResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_QuandoNaoExiste_DeveRetornar404()
    {
        var response = await _client.DeleteAsync($"/api/tarefas/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchConcluir_QuandoExiste_DeveMarcarComoConcluida()
    {
        var criarResponse = await _client.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Para concluir" });
        var criada = await criarResponse.Content.ReadFromJsonAsync<TarefaDto>();

        var response = await _client.PatchAsync($"/api/tarefas/{criada!.Id}/concluir", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tarefa = await response.Content.ReadFromJsonAsync<TarefaDto>();
        Assert.NotNull(tarefa);
        Assert.True(tarefa!.Concluida);
        Assert.NotNull(tarefa.ConcluidaEm);
    }

    [Fact]
    public async Task PatchReabrir_QuandoConcluida_DeveReabrir()
    {
        var criarResponse = await _client.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Para reabrir" });
        var criada = await criarResponse.Content.ReadFromJsonAsync<TarefaDto>();
        await _client.PatchAsync($"/api/tarefas/{criada!.Id}/concluir", null);

        var response = await _client.PatchAsync($"/api/tarefas/{criada.Id}/reabrir", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tarefa = await response.Content.ReadFromJsonAsync<TarefaDto>();
        Assert.NotNull(tarefa);
        Assert.False(tarefa!.Concluida);
        Assert.Null(tarefa.ConcluidaEm);
    }
}